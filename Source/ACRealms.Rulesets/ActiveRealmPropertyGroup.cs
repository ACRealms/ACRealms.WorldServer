using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    internal interface IRealmPropertyGroup
    {
        RealmPropertyGroupOptions OptionsBase { get; }
    }

    internal interface IRealmPropertyGroup<TVal> : IRealmPropertyGroup
        where TVal : IEquatable<TVal>
    {
        int PropertyKey { get; }
        RealmPropertyGroupOptions<TVal> Options { get; }
        IRealmPropertyGroup<TVal> Parent { get; }
    }

    internal abstract record ActiveRealmPropertyGroup(RulesetCompilationContext ctx, int propertyKey)
    {
        protected RulesetCompilationContext Context { get; } = ctx;
        public int PropertyKey { get; } = propertyKey;
        public RealmPropertyGroupOptions OptionsBase { get; init; }
    }

    /// <summary>
    /// Represents a collection of all properties associated with a single ruleset, for a single key.
    /// There will be more than one item in this collection if multiple scopes are defined for a ruleset with the same property key
    /// </summary>
    internal record ActiveRealmPropertyGroup<TVal>(RulesetCompilationContext ctx, int propertyKey) : ActiveRealmPropertyGroup(ctx, propertyKey), IRealmPropertyGroup<TVal>
        where TVal : IEquatable<TVal>
    {
        IEnumerable<ActiveRealmProperty<TVal>> Properties { get; init; }

        /// <summary>
        /// Gets properties, excluding properties that are impossible to eval and can be safely pruned
        /// </summary>
        IEnumerable<ActiveRealmProperty<TVal>> EffectiveProperties =>
            Properties.TakeWhile(p => !p.Options.Scope.GlobalScope)
                      .Concat(Properties.SkipWhile(p => !p.Options.Scope.GlobalScope).Take(1));

        IRealmPropertyGroup<TVal> IRealmPropertyGroup<TVal>.Parent { get; }
        public ActiveRealmPropertyGroup<TVal> Parent { get; }

        public RealmPropertyGroupOptions<TVal> Options => (RealmPropertyGroupOptions<TVal>)OptionsBase;
        public bool ContainsAtLeast1GlobalScope => Properties.Any(x => x.Options.Scope.GlobalScope);
        public bool ContainsOnlyGlobalScope => Properties.First().Options.Scope.GlobalScope;
        protected void LogTrace(Func<string> message)
        {
            if (!Context.Trace)
                return;
            Context.LogDirect($"   [P][{Options.Name}] (Def:{Options.RulesetName}) {message()}");
        }

        //Clone
        public ActiveRealmPropertyGroup(RulesetCompilationContext ctx, IRealmPropertyGroup<TVal> prop, IRealmPropertyGroup<TVal> parent = null)
            : this(ctx, prop.PropertyKey)
        {
            OptionsBase = prop.Options;
            IEnumerable<AppliedRealmProperty<TVal>> properties;
            if (prop is ActiveRealmPropertyGroup<TVal> p)
                properties = p.Properties;
            else if (prop is TemplatedRealmPropertyGroup<TVal> p2)
                properties = p2.Properties;
            else
                throw new NotImplementedException();

            Properties = properties.Select(p => new ActiveRealmProperty<TVal>(ctx)
            {
                Group = this,
                Options = p.Options,
                PropertyKey = PropertyKey
            });

            TryGetCommonOption(opts => opts.CompositionType, out RealmPropertyCompositionType? compositionType);

            LogTrace(() => $"Cloning from template. CompositionType: {compositionType?.ToString() ?? "[WARN!] Varies with scope" }");

            if (compositionType.HasValue && compositionType == RealmPropertyCompositionType.replace)
            {
                if (parent != null)
                    LogTrace(() => $"Discarding parent: {parent.Options.RulesetName}");
                else if (prop.Parent != null)
                    LogTrace(() => $"Discarding parent (from template parent): {prop.Parent.Options.RulesetName}");
            }
            else
            { 
                if (parent != null)
                {
                    LogTrace(() => $"Setting parent (explicitly passed): {parent.Options.RulesetName}");
                    Parent = new ActiveRealmPropertyGroup<TVal>(ctx, parent, null);
                }
                else if (prop.Parent != null)
                {
                    LogTrace(() => $"Setting parent (from template parent): {prop.Parent.Options.RulesetName}");
                    Parent = new ActiveRealmPropertyGroup<TVal>(ctx, prop.Parent, null);
                }
            }
        }

        /// <summary>
        /// Returns the HardDefaultValue in case of end of evaluation chain without resolution,
        /// Otherwise returns the evaluation result
        // 
        /// Not public because this is not the final result of property fetching, only eval!
        /// Does not do MinMax clamping
        /// Is not aware of Server Properties possibly used as fallbacks (this is a library, not the server itself)
        /// </summary>
        /// <param name="worldCtx"></param>
        /// <param name="val"></param>
        /// <returns>True if the property was successfully evaluated</returns>
        internal bool TryEval(IReadOnlyCollection<(string, RealmProps.Contexts.ICanonicalContextEntity)> worldCtx, out TVal val)
        {
            return EvalImpl(worldCtx, out val, direct: true);
        }

        // This has a very counterintuitive but critical implementation, so we also shouldn't make this public
        // Returns true if we can guarantee that the compose options for the given predicate are a match for all possible world contexts
        // when eval'ing this group
        // This is used during compilation in order to prune calculations that would be dropped from the evaluation chain anyway, saving on performance
        internal bool TrueForAll(Func<RealmPropertyOptions, bool> predicate)
        {
            if (!ContainsAtLeast1GlobalScope)
                return false;

            if (!Properties.Any())
                throw new InvalidOperationException("Unexpected empty property group");

            return EffectiveProperties.Select(p => p.Options).All(predicate);
        }

        // If true, then fetching the common option yielded a result that is true for all scoped variations of this property
        internal bool TryGetCommonOption<TResult>(Func<RealmPropertyOptions, TResult> selector, out TResult result)
        {
            if (!ContainsAtLeast1GlobalScope)
            {
                result = default;
                return false;
            }
            var guessedResults = EffectiveProperties.Select(p => p.Options).Select(selector);
            var nonUniq = guessedResults.Distinct().Skip(1);
            if (nonUniq.Any())
            {
                result = default;
                return false;
            }
            result = guessedResults.First();
            return true;
        }

        private bool EvalImpl(IReadOnlyCollection<(string, RealmProps.Contexts.ICanonicalContextEntity)> worldCtx, out TVal val, bool direct = true, bool scopelessPreEval = false)
        {
            if (scopelessPreEval)
            {
                foreach (var prop in Properties)
                    prop.TryEval(null, out _, true);
                val = default;
                return true;
            }

            foreach (var prop in Properties)
            {
                // Parent eval will be called from the prop itself
                if (prop.TryEval(worldCtx, out val))
                    return true;
            }

            // Scope not matched, parent was not eval'ed
            if (Parent != null)
            {
                return Parent.TryEval(worldCtx, out val);
            }
            else
            {
                // Fallback to HardDefaultValue, flag unsuccessful so the caller can decide whether to use this fallback or not
                val = Options.Prototype.HardDefaultValue;
                return false;
            }
        }

        internal TVal Eval(IReadOnlyCollection<(string, RealmProps.Contexts.ICanonicalContextEntity)> worldCtx, bool direct = true, bool scopelessPreEval = false)
        {
            EvalImpl(worldCtx, out var val, direct: direct, scopelessPreEval: scopelessPreEval);
            return val;
        }

        internal void PreEval()
        {
            EvalImpl(null, out _, scopelessPreEval: true);
        }

        internal string GetAppliedParentChain(StringBuilder sb = null)
        {
            bool direct = sb == null;
            sb ??= new StringBuilder();
            if (!direct) sb.AppendFormat("<-{0}", Options.RulesetName);
            if (Parent != null) Parent.GetAppliedParentChain(sb);
            return direct ? sb.ToString() : null;
        }

    }
}
