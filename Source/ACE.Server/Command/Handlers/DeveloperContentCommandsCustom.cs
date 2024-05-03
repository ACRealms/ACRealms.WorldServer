using ACE.Server.Network;
using System;
using System.IO;
using System.Linq;

namespace ACE.Server.Command.Handlers.Processors
{
    public partial class DeveloperContentCommands
    {
        public static void ImportSQLWeenieWrapped(ISession session, string param, string param2)
        {
            DirectoryInfo di = VerifyContentFolder(session);
            if (!di.Exists) return;

            var sep = Path.DirectorySeparatorChar;

            var prefix = param + " ";

            var sql_folder = $"{di.FullName}{sep}sql{sep}weenies{sep}";

            if (param.Equals("folder", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(param2))
            {
                if (param2.Contains(".."))
                {
                    CommandHandlerHelper.WriteOutputInfo(session, $"Path may not contain the sequence '..'");
                    return;
                }
                sql_folder = $"{sql_folder}{param2}{sep}";
                prefix = "";
            }
            else if (param.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "";
            }

            di = new DirectoryInfo(sql_folder);
            if (!di.Exists)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Couldn't find folder: {di.FullName}");
                return;
            }

            var files = di.Exists ? di.GetFiles($"{prefix}*.sql", SearchOption.AllDirectories) : null;

            if (files == null || files.Length == 0)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Couldn't find {sql_folder}{prefix}*.sql");
                return;
            }

            foreach (var file in files)
                ImportSQLWeenie(session, Path.GetDirectoryName(file.FullName) + Path.DirectorySeparatorChar, file.Name);
        }
    }
}
