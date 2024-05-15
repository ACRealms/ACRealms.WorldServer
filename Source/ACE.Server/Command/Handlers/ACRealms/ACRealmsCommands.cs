using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity.Actions;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Physics.Common;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ACE.Server.Command.Handlers
{
    public static class ACRealmsCommands
    {
        [CommandHandler("telerealm", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0, "Teleports the current player to another realm.")]
        public static void HandleMoveRealm(ISession session, params string[] parameters)
        {
            if (parameters.Length < 1)
                return;
            if (!ushort.TryParse(parameters[0], out var realmid))
                return;

            var pos = session.Player.Location;
            var newpos = new InstancedPosition(pos, InstancedPosition.InstanceIDFromVars(realmid, 0, false));

            session.Player.Teleport(newpos);
            var positionMessage = new GameMessageSystemChat($"Teleporting to realm {realmid}.", ChatMessageType.Broadcast);
            session.Network.EnqueueSend(positionMessage);
        }

        [CommandHandler("zoneinfo", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Lists all properties for the current realm.")]
        public static void HandleZoneInfo(ISession session, params string[] parameters)
        {
            session.Network.EnqueueSend(new GameMessageSystemChat($"\n{session.Player.CurrentLandblock.RealmRuleset.DebugOutputString()}", ChatMessageType.System));
        }

        [CommandHandler("exitinstance", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleExitInstance(ISession session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("exitinst", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleExitInst(ISession session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("exiti", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleExitI(ISession session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("leaveinstance", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleLeaveInstance(ISession session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("leaveinst", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleLeaveInst(ISession session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("leavei", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleLeaveI(ISession session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("hideout", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Recalls to your hideout.")]
        public static void HandleHideout(ISession session, params string[] parameters)
        {
            if (session?.Player?.HomeRealm == null)
                return;
            if (!Managers.RealmManager.GetRealm(session.Player.HomeRealm).StandardRules.GetProperty(RealmPropertyBool.HideoutEnabled))
            {
                session.Network.EnqueueSend(new GameMessageSystemChat($"Your home realm has not enabled hideouts.", ChatMessageType.Broadcast));
                return;
            }
            
            session.Player.HandleActionTeleToHideout();
        }

        // Requires IsDuelingRealm and HomeRealm to be set
        [CommandHandler("rebuff", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0,
            "Buffs you with all beneficial spells. Only usable in certain realms.")]
        public static void HandleRebuff(ISession session, params string[] parameters)
        {
            var player = session.Player;
            var realm = RealmManager.GetRealm(player.HomeRealm);
            if (realm == null) return;
            if (!realm.StandardRules.GetProperty(RealmPropertyBool.IsDuelingRealm)) return;
            var ts = player.GetProperty(PropertyInt.LastRebuffTimestamp);
            if (ts != null)
            {
                var timesince = (int)Time.GetUnixTime() - ts.Value;
                if (timesince < 180)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You may use this command again in {180 - timesince}s.", ChatMessageType.Broadcast));
                    return;
                }
            }
            player.SetProperty(PropertyInt.LastRebuffTimestamp, (int)Time.GetUnixTime());
            player.CreateSentinelBuffPlayers(new Player[] { player }, true);
        }

        [CommandHandler("duels", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0,
         "Recalls you to the duel staging area.")]
        public static void HandleRecallDuels(ISession session, params string[] parameters)
        {
            if (RealmManager.DuelRealm == null)
                return;
            var player = session.Player;
            if (!player.ValidatePlayerRealmPosition(DuelRealmHelpers.GetDuelingAreaDrop()))
                return;

            if (player.PKTimerActive)
            {
                session.Network.EnqueueSend(new GameEventWeenieError(session, WeenieError.YouHaveBeenInPKBattleTooRecently));
                return;
            }

            if (player.RecallsDisabled)
            {
                session.Network.EnqueueSend(new GameEventWeenieError(session, WeenieError.ExitTrainingAcademyToUseCommand));
                return;
            }

            if (player.TooBusyToRecall)
            {
                session.Network.EnqueueSend(new GameEventWeenieError(session, WeenieError.YoureTooBusy));
                return;
            }

            if (player.CombatMode != CombatMode.NonCombat)
            {
                // this should be handled by a different thing, probably a function that forces player into peacemode
                var updateCombatMode = new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.CombatMode, (int)CombatMode.NonCombat);
                player.SetCombatMode(CombatMode.NonCombat);
                session.Network.EnqueueSend(updateCombatMode);
            }

            player.EnqueueBroadcast(new GameMessageSystemChat($"{player.Name} is recalling to the duel staging area.", ChatMessageType.Recall), Player.LocalBroadcastRange, ChatMessageType.Recall);

            player.SendMotionAsCommands(MotionCommand.MarketplaceRecall, MotionStance.NonCombat);

            var startPos = new InstancedPosition(player.Location);

            // TODO: (OptimShi): Actual animation length is longer than in retail. 18.4s
            // float mpAnimationLength = MotionTable.GetAnimationLength((uint)MotionTableId, MotionCommand.MarketplaceRecall);
            // mpChain.AddDelaySeconds(mpAnimationLength);
            ActionChain mpChain = new ActionChain();
            mpChain.AddDelaySeconds(5);

            // Then do teleport
            player.IsBusy = true;
            mpChain.AddAction(player, () =>
            {
                player.IsBusy = false;
                var endPos = new InstancedPosition(player.Location);
                if (startPos.SquaredDistanceTo(endPos) > Player.RecallMoveThresholdSq)
                {
                    session.Network.EnqueueSend(new GameEventWeenieError(session, WeenieError.YouHaveMovedTooFar));
                    return;
                }

                player.Teleport(DuelRealmHelpers.GetDuelingAreaDrop());
            });

            // Set the chain to run
            mpChain.EnqueueChain();
        }

        [CommandHandler("reload-all-landblocks", AccessLevel.Admin, CommandHandlerFlag.None, 0, "Reloads all landblocks currently loaded.")]
        public static void HandleReloadAllLandblocks(ISession session, params string[] parameters)
        {
            ActionChain lbResetChain = new ActionChain();
            var lbs = LandblockManager.GetLoadedLandblocks().Select(x => (id: x.Id, instance: x.Instance));
            var enumerator = lbs.GetEnumerator();

            ActionEventDelegate resetLandblockAction = null;
            resetLandblockAction = new ActionEventDelegate(() =>
            {
                if (!enumerator.MoveNext())
                    return;
                if (LandblockManager.IsLoaded(enumerator.Current.id, enumerator.Current.instance))
                {
                    var lb = LandblockManager.GetLandblockUnsafe(enumerator.Current.id, enumerator.Current.instance);
                    if (lb != null)
                    {
                        if (session?.Player?.CurrentLandblock != lb)
                            CommandHandlerHelper.WriteOutputInfo(session, $"Reloading 0x{lb.LongId:X16}", ChatMessageType.Broadcast);
                        lb.Reload();
                    }
                }
                lbResetChain.AddDelayForOneTick();
                lbResetChain.AddAction(WorldManager.ActionQueue, resetLandblockAction);
            });
            lbResetChain.AddAction(WorldManager.ActionQueue, resetLandblockAction);
            lbResetChain.EnqueueChain();
        }

        [CommandHandler("compile-ruleset", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 1, "Gives a diagnostic trace of a ruleset compilation for the current landblock",
            "{ full | landblock | ephemeral-new | ephemeral-cached | all } ")]
        public static void HandleCompileRuleset(ISession session, params string[] parameters)
        {
            string type = parameters[0];

            Ruleset ruleset = null;
            switch (type)
            {
                case "landblock":
                    ruleset = AppliedRuleset.MakeRerolledRuleset(session.Player.RealmRuleset.Template, trace: true);
                    break;
                case "ephemeral-new":
                    if (!session.Player.CurrentLandblock.IsEphemeral)
                    {
                        session.Network.EnqueueSend(new GameMessageSystemChat($"The current landblock is not ephemeral.", ChatMessageType.Broadcast));
                        return;
                    }
                    ruleset = AppliedRuleset.MakeRerolledRuleset(session.Player.CurrentLandblock.InnerRealmInfo.RulesetTemplate.RebuildTemplateWithTrace(), true);
                    break;
                case "ephemeral-cached":
                    if (!session.Player.CurrentLandblock.IsEphemeral)
                    {
                        session.Network.EnqueueSend(new GameMessageSystemChat($"The current landblock is not ephemeral.", ChatMessageType.Broadcast));
                        return;
                    }
                    ruleset = AppliedRuleset.MakeRerolledRuleset(session.Player.RealmRuleset.Template, trace: true);
                    break;
                case "full":
                    RulesetTemplate template;
                    if (!session.Player.CurrentLandblock.IsEphemeral)
                        template = RealmManager.BuildRuleset(session.Player.RealmRuleset.Realm, trace: true);
                    else
                        template = session.Player.CurrentLandblock.InnerRealmInfo.RulesetTemplate.RebuildTemplateWithTrace();
                    ruleset = AppliedRuleset.MakeRerolledRuleset(template, true);
                    break;
                case "all":
                    HandleCompileRuleset(session, "landblock");
                    if (session.Player.CurrentLandblock.IsEphemeral)
                    {
                        HandleCompileRuleset(session, "ephemeral-cached");
                        HandleCompileRuleset(session, "ephemeral-new");
                    }
                    HandleCompileRuleset(session, "full");
                    return;
                default:
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Unknown compilation type.", ChatMessageType.Broadcast));
                    return;
            }

            var filename = $"compile-ruleset-output-{session.Player.Name}-{type}.txt";
            File.WriteAllLines(filename, ruleset.TraceLog);
            session.Network.EnqueueSend(new GameMessageSystemChat($"Logged compilation output to {filename}", ChatMessageType.Broadcast));
        }
    }
}
