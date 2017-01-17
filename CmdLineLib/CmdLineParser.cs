using System;
using System.Collections.Generic;
using System.Linq;

namespace CmdLineLib
{
    class CmdLineParser
    {
        char ArgSeparator;
        string ArgStartsWith;
        CmdLineConfig Config;

        public string Command { get; private set; }
        public IReadOnlyDictionary<string, string> Args { get; private set; }
        public bool IsHelpCommand { get; }

        public CmdLineParser(string[] args, CmdLineConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            ArgStartsWith = Config.ArgStartsWith.ToString();
            ArgSeparator = Config.ArgSeparator;
            string command = null;
            int c = 0;
            if (args.Length > 0)
            {
                IsHelpCommand = args[0] == $"{config.ArgStartsWith}?"
                    || args[0].Equals("help", StringComparison.CurrentCultureIgnoreCase);
                if (IsHelpCommand)
                {
                    if (args.Length > 1)
                        Command = args[1];
                    return;
                }
            }
            var parsedArgs = args.Select(s => ParseArg(s, config.RequiresCommand && c++ == 0)).ToList();
            if (parsedArgs.Count > 0)
            {
                if (config.RequiresCommand)
                {
                    var first = parsedArgs.First();
                    if (first.Key != null)
                    {
                        command = null;
                    }
                    else
                    {
                        command = first.Value;
                        parsedArgs.RemoveAt(0);
                    }
                }
            }

            Command = command;
            Args = parsedArgs.ToDictionary(p => p.Key, p => p.Value);
        }
        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        KeyValuePair<string, string> ParseArg(string arg)
        {
            if (!arg.StartsWith(ArgStartsWith))
                throw new CmdLineArgException(arg, $"Invalid argument '{arg}'");
            int indexOf = arg.IndexOf(ArgSeparator);
            return indexOf < 0
                ? new KeyValuePair<string, string>(
                    arg.Substring(ArgStartsWith.Length),
                    null)
                : new KeyValuePair<string, string>(
                    arg.Substring(ArgStartsWith.Length, indexOf - ArgStartsWith.Length),
                    arg.Substring(indexOf + 1));
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        KeyValuePair<string, string> ParseArg(string arg, bool isCommandArg)
        {
            if (isCommandArg)
            {
                if (!arg.StartsWith(ArgStartsWith) && !arg.Contains(ArgSeparator))
                    return new KeyValuePair<string, string>(null, arg);
            }
            else if (arg.StartsWith(ArgStartsWith))
            {
                var split = arg.Split(new char[] { ArgSeparator }, 2);
                if (split.Length >= 1)
                {
                    if (split[0].Length > 0)
                    {
                        var argName = split[0].Substring(1);
                        if (split.Length == 1)
                            return new KeyValuePair<string, string>(argName, null);
                        else if (split.Length == 2)
                            return new KeyValuePair<string, string>(argName, split[1]);
                    }
                }
            }
            throw new CmdLineArgException(arg, $"Invalid argument '{arg}'");
        }

    }
}
