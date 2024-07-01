using ACE.Entity.ACRealms;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.WorldObjects;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{

    public abstract record CanonicalCharacterName(string Name, string RealmName)
        : CanonicalName<IPlayer, CanonicalCharacterName>(Name)
    {
        /// <summary>
        /// Gets the name displayed to players sharing the same home realm
        /// </summary>
        public abstract string DisplayNameSameRealm { get; protected init; }

        /// <summary>
        /// Gets the name displayed to players who do not have the same home realm of this character
        /// </summary>
        public abstract string DisplayNameOtherRealm { get; protected init; }

        internal static CanonicalCharacterName FromPlayer(IPlayer player)
            => Make(player.Name, player.HomeRealmIDRaw, player.Account.AccessLevel > 1);

        internal static CanonicalCharacterName Make(string name, int? rawRealmId, bool isAdmin)
        {
            string realmName = RealmManager.GetDisplayNameForAnyRawRealmId(rawRealmId);
            return Make(name, realmName, isAdmin);
        }

        internal static CanonicalCharacterName Make(string name, string realmName, bool isAdmin)
        {
            if (isAdmin)
                return new AdminCharacterName(name, realmName);
            else if (Common.ACRealms.ACRealmsConfigManager.Config.CharacterCreationOptions.CharacterNamesUniquePerHomeRealm)
                return new ShardedCharacterName(name, realmName);
            else
                return new BasicCharacterName(name);
        }
    }


    public record AdminCharacterName(string Name, string RealmName)
    : CanonicalCharacterName(Name, RealmName)
    {
        public override string DisplayNameSameRealm { get; protected init; } = $"+{Name}";
        public override string DisplayNameOtherRealm { get; protected init; } = $"+{Name}";
    }

    public record BasicCharacterName(string Name)
        : CanonicalCharacterName(Name, null)
    {
        public override string DisplayNameSameRealm { get; protected init; } = Name;
        public override string DisplayNameOtherRealm { get; protected init; } = Name;
    }

    public record ShardedCharacterName(string Name, string RealmName)
    : CanonicalCharacterName(Name, RealmName)
    {
        public override string DisplayNameSameRealm { get; protected init; } = Name;
        public override string DisplayNameOtherRealm { get; protected init; } = $"{Name} [{RealmName}]";
    }

    [Flags]
    public enum CharacterNameResolverOptions
    {
        None = 0,
        AlwaysRevealAmbiguousMatches = 0x1,
        RequireExactlyOneMatch = 0x2,
        RequireSameHomeRealm = 0x4,
        RequireOnlinePlayer = 0x8,
        RequireShardedNameInvocation = 0x10,
        ResolveWithHomeRealm = 0x20,
        ResolveWithOnlineStatus = 0x40,
        NeverResolveAmbiguousMatches = 0x80,
        IncludePendingDeletedCharactersInResults = 0x100,

        ResolveAmbiguous = ResolveWithHomeRealm | ResolveWithOnlineStatus,
        MatchOne = RequireExactlyOneMatch | ResolveAmbiguous,
        MatchOneOnline = MatchOne | RequireOnlinePlayer,
        MatchOneHomeRealm = MatchOne | RequireSameHomeRealm,
        MatchAll = NeverResolveAmbiguousMatches | AlwaysRevealAmbiguousMatches,
        MatchAllOnline = MatchAll | RequireOnlinePlayer,
    }

    public record CharacterNameResolverContext(string CommandName, string PassedName, ISession RequestingSession, CharacterNameResolverOptions Options)
        : CanonicalResolverContext<IPlayer, CanonicalCharacterName>
    {
        public override CanonicalCharacterName CanonicalName { get; } = BuildCanonicalNameFromArgs(PassedName);
        private static CanonicalCharacterName BuildCanonicalNameFromArgs(string passedName)
        {
            if (!Common.ACRealms.ACRealmsConfigManager.Config.CharacterCreationOptions.CharacterNamesUniquePerHomeRealm)
                return new BasicCharacterName(passedName);

            CanonicalCharacterName nameArg;
            var match = Regex.Match(passedName, @"(.*) \[(.*)\]");
            if (match.Success)
            {
                var basicName = match.Groups[0].Value;
                var realmName = match.Groups[1].Value;
                nameArg = new ShardedCharacterName(basicName, realmName);
            }
            else
                nameArg = new BasicCharacterName(passedName);

            return nameArg;
        }

        public override bool TryResolve(out IEnumerable<IPlayer> players)
            => PlayerManager.CanonicalStore.TryResolve(this, out players);
    }

    public interface INameResolvableCanonicalStore<TKey, TEntity, TName, TContext>
        where TEntity : ICanonicallyResolvable<TEntity, TName>
        where TKey : IEquatable<TKey>
        where TName : CanonicalName<TEntity, TName>
        where TContext : CanonicalResolverContext<TEntity, TName>
    {
        IReadOnlyDictionary<TKey, TEntity> PrimaryStore { get; }
        IEnumerable<TKey> GetPrimaryKeysMatchingLabel(string label);
        bool TryResolve(TContext context, out IEnumerable<TEntity> results);
    }

    public class CanonicalCharacterNameStore
        : INameResolvableCanonicalStore<ulong, IPlayer, CanonicalCharacterName, CharacterNameResolverContext>
    {
        private ConcurrentDictionary<ulong, IPlayer> _primaryStore = new ConcurrentDictionary<ulong, IPlayer>();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Keys shall not be removed after being inserted.<para/>
        /// You may cast this to its native ConcurrentDictionary type, but I strongly recommend against it
        /// </summary>
        public IReadOnlyDictionary<ulong, IPlayer> PrimaryStore => _primaryStore;

        public CanonicalCharacterNameStore(ConcurrentDictionary<ulong, IPlayer> primaryStore)
        {
            _primaryStore = primaryStore;
        }

        public IEnumerable<ulong> GetPrimaryKeysMatchingLabel(string label)
            => PlayerManager.GetPlayerGuidsByBasicName(label);

        public bool TryResolveOnline(CharacterNameResolverContext context, out IEnumerable<Player> results)
        {
            var options = context.Options;
            options |= CharacterNameResolverOptions.RequireOnlinePlayer;

            var success = TryResolve(context, out var resultsInvariant, options);
            results = resultsInvariant.Cast<Player>();
            return success;
        }

        public bool TryResolve(CharacterNameResolverContext context, out IEnumerable<IPlayer> results) =>
            TryResolve(context, out results, context.Options);

        private bool? TryResolve(CharacterNameResolverContext context, out IEnumerable<IPlayer> results, CharacterNameResolverOptions options, IPlayer[] narrowedList)
        {
            if (narrowedList.Length == 0)
            {
                results = [];
                return false;
            }
            else if (narrowedList.Length == 1)
            {
                results = narrowedList;
                return true;
            }
            else
            {
                results = narrowedList;

                if (options.HasFlag(CharacterNameResolverOptions.NeverResolveAmbiguousMatches))
                    return null;

                if (options.HasFlag(CharacterNameResolverOptions.ResolveWithOnlineStatus))
                {
                    var narrowed2 = narrowedList.Where(x => x.IsOnline).ToArray();
                    if (narrowed2.Length == 1)
                    {
                        results = narrowed2;
                        return true;
                    }
                }
                if (options.HasFlag(CharacterNameResolverOptions.ResolveWithHomeRealm) && context.RequestingSession != null)
                {
                    var requestSourceHomeRealm = context.RequestingSession.Player.HomeRealm;
                    if (requestSourceHomeRealm != 0)
                    {
                        var narrowed2 = results.Where(x => x.HomeRealm == requestSourceHomeRealm).ToArray();
                        if (narrowed2.Length == 1)
                        {
                            results = narrowed2;
                            return true;
                        }
                        else if (narrowed2.Length > 1 && options.HasFlag(CharacterNameResolverOptions.ResolveWithOnlineStatus))
                        {
                            narrowed2 = narrowed2.Where(x => x.IsOnline).ToArray();
                            if (narrowed2.Length == 1)
                            {
                                results = narrowed2;
                                return true;
                            }    
                        }
                    }
                }
                return null;
            }
        }


        private bool TryResolve(CharacterNameResolverContext context, out IEnumerable<IPlayer> results, CharacterNameResolverOptions options)
        {
            // Try to preserve legacy behavior of requiring exactly one match since multiple characters can't exist with the same name anyway
            if (!Common.ACRealms.ACRealmsConfigManager.Config.CharacterCreationOptions.CharacterNamesUniquePerHomeRealm)
                options |= CharacterNameResolverOptions.RequireExactlyOneMatch;

            // Plussed characters never use their home realm as a contextual resolve
            if (context.RequestingSession == null || context.RequestingSession.Player.Account.AccessLevel > 1)
            {
                options &= ~CharacterNameResolverOptions.RequireSameHomeRealm;
                options &= ~CharacterNameResolverOptions.ResolveWithHomeRealm;
                options |= CharacterNameResolverOptions.AlwaysRevealAmbiguousMatches;
            }

            if (options.HasFlag(CharacterNameResolverOptions.RequireShardedNameInvocation) && !(context.CanonicalName is ShardedCharacterName))
            {
                results = [];
                return false;
            }

            ushort? targetRealmId;
            if (context.CanonicalName is ShardedCharacterName shardedName)
            {
                var realm = RealmManager.GetRealmByName(shardedName.RealmName, includeRulesets: false);
                if (realm == null)
                {
                    results = [];
                    return false;
                }
                else
                    targetRealmId = realm.Realm.Id;
            }
            else if (options.HasFlag(CharacterNameResolverOptions.RequireSameHomeRealm))
                targetRealmId = context.RequestingSession.Player.HomeRealm;
            else
                targetRealmId = null;

            if (targetRealmId == 0)
            {
                results = [];
                return false;
            }

            if (!PlayerManager.DoesBasicNameExist(context.CanonicalName.Name))
            {
                results = [];
                return false;
            }

            var keys = GetPrimaryKeysMatchingLabel(context.CanonicalName.Name);
            if (!keys.Any())
            {
                results = [];
                return false;
            }

            var players = keys.Select(guid => {
                if (PrimaryStore.TryGetValue(guid, out var player))
                    return player;
                else
                    return null;
            });
            players = players.Where(p => p != null);

            if (options.HasFlag(CharacterNameResolverOptions.RequireOnlinePlayer))
                players = players.Where(p => p.IsOnline);

            if (targetRealmId.HasValue)
                players = players.Where(p => p.HomeRealm == targetRealmId);

            players = players.Where(p => !p.IsDeleted);

            if (!options.HasFlag(CharacterNameResolverOptions.IncludePendingDeletedCharactersInResults))
                players = players.Where(p => !p.IsPendingDeletion);

            // From this point on the players list will be a snapshot from the point where ToArray was called
            var maybeResult = TryResolve(context, out results, options, players.ToArray());
            if (maybeResult.HasValue)
                return maybeResult.Value;

            if (!results.Any())
                return false;
            if (results.Count() == 1)
                return true;
            if (options.HasFlag(CharacterNameResolverOptions.RequireExactlyOneMatch))
            {
                if (!options.HasFlag(CharacterNameResolverOptions.AlwaysRevealAmbiguousMatches))
                   results = [];
                return false;
            }
            
            return true;
        }
    }
}
