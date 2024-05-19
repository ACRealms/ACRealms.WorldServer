using ACE.Entity.Enum;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACE.Database.Adapter;
using ACE.Server.Managers;
using log4net;
using ACE.Server.Command.Handlers.Processors;
using System.Runtime.CompilerServices;

namespace ACE.Server.Command.Handlers
{
    public static class RealmDataCommands
    {
        [CommandHandler("import-realms", AccessLevel.Developer, CommandHandlerFlag.None, 0, "Imports all json realms from the Content folder")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void HandleImportRealms(ISession session, params string[] parameters)
        {
            DateTime now = DateTime.Now;
            DirectoryInfo di = DeveloperContentCommands.VerifyContentFolder(session);
            if (!di.Exists) return;

            var sep = Path.DirectorySeparatorChar;

            var realms_index = $"{di.FullName}{sep}json{sep}realms.jsonc";
            var json_folder = $"{di.FullName}{sep}json{sep}realms{sep}";

            try
            {
                var realms = RealmDataHelpers.ImportJsonRealmsFolder(session, json_folder);
                if (realms != null)
                    RealmDataHelpers.ImportJsonRealmsIndex(session, realms_index, realms);
                else
                    throw new InvalidDataException("Could not load realms files");
                session?.Network.EnqueueSend(new GameMessageSystemChat($"Synced {realms.Count} realms in {(DateTime.Now - now).TotalSeconds} seconds.", ChatMessageType.Broadcast));
            }
            catch (Exception ex)
            {
                CommandHandlerHelper.WriteOutputError(session, $"Error: {ex.Message}", ChatMessageType.Broadcast);
                throw;
            }
        }
    }
}
