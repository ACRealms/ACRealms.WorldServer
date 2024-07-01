using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.Network;

using log4net;

namespace ACE.Server.Command
{
    public static class CommandManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly bool NonInteractiveConsole = Convert.ToBoolean(Environment.GetEnvironmentVariable("ACE_NONINTERACTIVE_CONSOLE"));

        private static Dictionary<string, CommandHandlerInfo> commandHandlers = new Dictionary<string, CommandHandlerInfo>(StringComparer.OrdinalIgnoreCase);

        public static IEnumerable<CommandHandlerInfo> GetCommands()
        {
            return commandHandlers.Select(p => p.Value);
        }

        public static IEnumerable<CommandHandlerInfo> GetCommandByName(string commandname)
        {
            return commandHandlers.Select(p => p.Value).Where(p => p.Attribute.Command == commandname);
        }

        public static CommandHandler GetDelegate(Action<ISession, string[]> handler) => (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), handler.Method);

        public static bool TryAddCommand(MethodInfo handler, string command, AccessLevel access, CommandHandlerFlag flags = CommandHandlerFlag.None, string description = "", string usage = "", bool overrides = true)
        {
            var del = (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), handler);

            var info = new CommandHandlerInfo()
            {
                Attribute = new CommandHandlerAttribute(command, access, flags, description, usage),
                Handler = del,
            };
            return TryAddCommand(info, overrides);
        }

        public static bool TryAddCommand(Action<ISession, string[]> handler, string command, AccessLevel access, CommandHandlerFlag flags = CommandHandlerFlag.None, string description = "", string usage = "", bool overrides = true)
        {
            var del = (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), handler.Method);
            var info = new CommandHandlerInfo()
            {
                Attribute = new CommandHandlerAttribute(command, access, flags, description, usage),
                Handler = del
            };

            if (TryAddCommand(info, overrides))
                return true;

            return false;
        }

        public static bool TryAddCommand(CommandHandlerInfo commandHandler, bool overrides = true)
        {
            if (commandHandler is null)
                return false;

            var command = commandHandler.Attribute.Command;

            //Add if the command doesn't exist
            if (!commandHandlers.ContainsKey(command))
            {
                commandHandlers.Add(command, commandHandler);
                log.Info($"Command created: {command}");
                return true;
            }
            //Update if overriding and the command exists
            else if (overrides)
            {
                log.Info($"Command updated: {command}");
                commandHandlers[command] = commandHandler;
                return true;
            }
            log.Warn($"Failed to add command: {command}");
            return false;
        }

        public static bool TryRemoveCommand(string command)
        {
            if (!commandHandlers.ContainsKey(command))
                return false;

            log.Info($"Removed command: {command}");
            commandHandlers.Remove(command);
            return true;
        }

        public static void Initialize()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attribute in method.GetCustomAttributes<CommandHandlerAttribute>())
                    {
                        var commandHandler = new CommandHandlerInfo()
                        {
                            Handler = (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), method),
                            Attribute = attribute
                        };

                        commandHandlers[attribute.Command] = commandHandler;
                    }
                }
            }
        }

        internal static void InitializeCommandThread()
        {
            if (NonInteractiveConsole)
            {
                log.Info("AC Realms command prompt disabled - Environment.GetEnvironmentVariable(ACE_NONINTERACTIVE_CONSOLE) was true");
                return;
            }

            var thread = new Thread(new ThreadStart(CommandThread));
            thread.Name = "Command Manager";
            thread.IsBackground = true;
            thread.Start();
        }

        static IEnumerator<Task<string>> AsyncConsoleInput()
        {
            var e = loop(); e.MoveNext(); return e;
            IEnumerator<Task<string>> loop()
            {
                while (true) yield return Task.Run(Console.ReadLine);
            }
        }

        static Task<string> ReadLine(this IEnumerator<Task<string>> console)
        {
            if (console.Current.IsCompleted) console.MoveNext();
            return console.Current;
        }

        private static bool ResetConsole = false;
        private static string AsyncConsoleCommand = null;

        internal static void ResetConsoleThread() => ResetConsole = true;
        internal static void WriteAsyncConsoleCommand(string arg) => AsyncConsoleCommand = arg;

        private static void CommandThread()
        {
            ServerManager.FlushConsoleLogBuffers();
            Console.WriteLine("");
            Console.WriteLine("AC Realms command prompt ready.");
            Console.WriteLine("");
            Console.WriteLine("Type \"acecommands\" for help.");
            Console.WriteLine("");

# if DEBUG
            Console.WriteLine($"DEBUG: Console.IsOutputRedirected: {Console.IsOutputRedirected}");
            Console.WriteLine($"DEBUG: Console.IsInputRedirected: {Console.IsInputRedirected}");
#endif

            IEnumerator<Task<string>> console = null;
            if (Console.IsInputRedirected)
                console = AsyncConsoleInput();

            ServerManager.FlushConsoleLogBuffers();
            for (; ; )
            {
                Thread.Sleep(20);
                Console.WriteLine();
                Console.Write("ACRealms >> ");

                string commandLine;
                if (Console.IsInputRedirected)
                {
                    var resetTask = Task.Run(() =>
                    {
                        while (AsyncConsoleCommand == null)
                        {
                            Thread.Sleep(100);
                        }
                        
                        //ResetConsole = false;
                    });

                    if (Task.WaitAny(console.ReadLine(), resetTask) == 0) // if ReadLine finished first
                    {
                        Console.WriteLine("DEBUG: ReadLine");
                        commandLine = console.Current.Result; // last user input (await instead of Result in async method)
                    }
                    else // reset 
                    {
                        Console.WriteLine("DEBUG: AsyncCommand");
                        commandLine = AsyncConsoleCommand;
                        AsyncConsoleCommand = null;
                        //resetTask.Wait();
                        //Thread.Sleep(100);
                        //continue;
                        //commandLine = console.ReadLine(); // this wont issue another read line because user did not input anything yet. 
                    }
                }
                else
                {
                    commandLine = Console.ReadLine();
                }

                if (string.IsNullOrWhiteSpace(commandLine))
                    continue;

                Console.WriteLine($"DEBUG: {commandLine}");

                string command = null;
                string[] parameters = null;
                try
                {
                    ParseCommand(commandLine, out command, out parameters);
                }
                catch (Exception ex)
                {
                    log.Error($"Exception while parsing command: {commandLine}", ex);
                    continue;
                }
                try
                {
                    if (GetCommandHandler(null, command, parameters, out var commandHandler) == CommandHandlerResponse.Ok)
                    {
                        try
                        {
                            if (commandHandler.Attribute.IncludeRaw)
                            {
                                parameters = StuffRawIntoParameters(commandLine, command, parameters);
                            }
                            // Add command to world manager's main thread...
                            ((CommandHandler)commandHandler.Handler).Invoke(null, parameters);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"Exception while invoking command handler for: {commandLine}", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Exception while getting command handler for: {commandLine}", ex);
                }
                ServerManager.FlushConsoleLogBuffers();
            }
        }

        public static string[] StuffRawIntoParameters(string raw, string command, string[] parameters)
        {
            List<string> parametersRehash = new List<string>();
            var regex = new Regex(Regex.Escape(command));
            var newCmdLine = regex.Replace(raw, "", 1).TrimStart();
            parametersRehash.Add(newCmdLine);
            parametersRehash.AddRange(parameters);
            parameters = parametersRehash.ToArray();
            return parameters;
        }

        public static void ParseCommand(string commandLine, out string command, out string[] parameters)
        {
            if (commandLine == "/" || commandLine == "")
            {
                command = null;
                parameters = null;
                return;
            }
            var commandSplit = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            command = commandSplit[0];

            // remove leading '/' or '@' if erroneously entered in console
            if (command.StartsWith("/") || command.StartsWith("@"))
                command = command.Substring(1);

            parameters = new string[commandSplit.Length - 1];

            Array.Copy(commandSplit, 1, parameters, 0, commandSplit.Length - 1);

            if (commandLine.Contains("\""))
            {
                var listParameters = new List<string>();

                for (int start = 0; start < parameters.Length; start++)
                {
                    if (!parameters[start].StartsWith("\"") || parameters[start].EndsWith("\"")) // Make sure we catch parameters like: "someParam"
                        listParameters.Add(parameters[start].Replace("\"", ""));
                    else
                    {
                        listParameters.Add(parameters[start].Replace("\"", ""));
                        for (int end = start + 1; end < parameters.Length; end++)
                        {
                            if (!parameters[end].EndsWith("\""))
                                listParameters[listParameters.Count - 1] += " " + parameters[end];
                            else
                            {
                                listParameters[listParameters.Count - 1] += " " + parameters[end].Replace("\"", "");
                                start = end;
                                break;
                            }
                        }
                    }
                }
                Array.Resize(ref parameters, listParameters.Count);
                parameters = listParameters.ToArray();
            }
        }

        public static CommandHandlerResponse GetCommandHandler(ISession session, string command, string[] parameters, out CommandHandlerInfo commandInfo)
        {
            if (command == null || parameters == null)
            {
                commandInfo = null;
                return CommandHandlerResponse.InvalidCommand;
            }
            bool isSUDOauthorized = false;

            if (command.ToLower() == "sudo")
            {
                string sudoCommand = "";
                if (parameters.Length > 0)
                    sudoCommand = parameters[0];

                if (!commandHandlers.TryGetValue(sudoCommand, out commandInfo))
                    return CommandHandlerResponse.InvalidCommand;

                if (session == null)
                {
                    Console.WriteLine("SUDO does not work on the console because you already have full access. Remove SUDO from command and execute again.");
                    return CommandHandlerResponse.InvalidCommand;
                }

                if (commandInfo.Attribute.Access <= session.AccessLevel)
                    isSUDOauthorized = true;

                if (isSUDOauthorized)
                {
                    command = sudoCommand;
                    var sudoParameters = new string[parameters.Length - 1];
                    for (int i = 1; i < parameters.Length; i++)
                        sudoParameters[i - 1] = parameters[i];
                    parameters = sudoParameters;
                }
            }

            if (!commandHandlers.TryGetValue(command, out commandInfo))
            {
                // Provide some feedback for why the console command failed
                if (session == null)
                    Console.WriteLine($"Invalid Command");

                return CommandHandlerResponse.InvalidCommand;
            }

            if ((commandInfo.Attribute.Flags & CommandHandlerFlag.ConsoleInvoke) != 0 && session != null)
                return CommandHandlerResponse.NoConsoleInvoke;

            if (session != null)
            {
                bool isAdvocate = session.Player.IsAdvocate;
                bool isSentinel = session.Player.IsSentinel;
                bool isEnvoy = session.Player.IsEnvoy;
                bool isArch = session.Player.IsArch;
                bool isAdmin = session.Player.IsAdmin;

                if (commandInfo.Attribute.Access == AccessLevel.Advocate && !(isAdvocate || isSentinel || isEnvoy || isArch || isAdmin || isSUDOauthorized)
                    || commandInfo.Attribute.Access == AccessLevel.Sentinel && !(isSentinel || isEnvoy || isArch || isAdmin || isSUDOauthorized)
                    || commandInfo.Attribute.Access == AccessLevel.Envoy && !(isEnvoy || isArch || isAdmin || isSUDOauthorized)
                    || commandInfo.Attribute.Access == AccessLevel.Developer && !(isArch || isAdmin || isSUDOauthorized)
                    || commandInfo.Attribute.Access == AccessLevel.Admin && !(isAdmin || isSUDOauthorized))
                    return CommandHandlerResponse.NotAuthorized;
            }

            if (commandInfo.Attribute.ParameterCount != -1 && parameters.Length < commandInfo.Attribute.ParameterCount)
            {
                // Provide some feedback for why the console command failed
                if (session == null)
                    Console.WriteLine($"The syntax of the command is incorrect.\nUsage: " + commandInfo.Attribute.Command + " " + commandInfo.Attribute.Usage);
                return CommandHandlerResponse.InvalidParameterCount;
            }

            if ((commandInfo.Attribute.Flags & CommandHandlerFlag.RequiresWorld) != 0 && (session == null || session.Player == null || session.Player.CurrentLandblock == null))
                return CommandHandlerResponse.NotInWorld;

            if (isSUDOauthorized)
                return CommandHandlerResponse.SudoOk;

            return CommandHandlerResponse.Ok;
        }
    }
}
