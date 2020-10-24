using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity.Actions;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Command.Handlers
{
    public static class CustomCommands
    {
        [CommandHandler("telerealm", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0, "Teleports the current player to another realm.")]
        public static void HandleMoveRealm(Session session, params string[] parameters)
        {
            if (parameters.Length < 1)
                return;
            if (!ushort.TryParse(parameters[0], out var realmid))
                return;

            var pos = session.Player.GetPosition(PositionType.Location);
            var newpos = new Position(pos);
            newpos.SetToDefaultRealmInstance(realmid);

            session.Player.Teleport(newpos);
            var positionMessage = new GameMessageSystemChat($"Teleporting to realm {realmid}.", ChatMessageType.Broadcast);
            session.Network.EnqueueSend(positionMessage);
        }

        [CommandHandler("zoneinfo", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Lists all properties for the current realm.")]
        public static void HandleZoneInfo(Session session, params string[] parameters)
        {
            session.Network.EnqueueSend(new GameMessageSystemChat($"\n{session.Player.CurrentLandblock.RealmRuleset.DebugOutputString()}", ChatMessageType.System));
        }

        [CommandHandler("exitinstance", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleExitInstance(Session session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("exitinst", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleExitInst(Session session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }


        [CommandHandler("exiti", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleExitI(Session session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("leaveinstance", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleLeaveInstance(Session session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("leaveinst", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleLeaveInst(Session session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("leavei", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Leaves the current instance, if the player is currently in one.")]
        public static void HandleLeaveI(Session session, params string[] parameters)
        {
            session.Player.ExitInstance();
        }

        [CommandHandler("hideout", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Recalls to your hideout.")]
        public static void HandleHideout(Session session, params string[] parameters)
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

        [CommandHandler("rebuff", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0,
            "Buffs you with all beneficial spells. Only usable in certain realms.")]
        public static void HandleRebuff(Session session, params string[] parameters)
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
        public static void HandleRecallDuels(Session session, params string[] parameters)
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

            var startPos = new Position(player.Location);

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
                var endPos = new Position(player.Location);
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
    }
}
