using System;
using System.Collections.Generic;
using System.Linq;
using CmdLineLib.Attributes;

namespace CmdLineLib
{
    public interface IInstanceProvider
    {
        Type TypeOf { get; }
        object Instance { get; }
        object GetSafeInstance();
    }

    /// <summary> Enumerates args and execute method of object of type T.
    /// Method match is done based on <see cref="CmdLineMethodAttribute"/> attribute.
    /// Method with ArgMethodAttribute can have one or more parameters defined with <see cref="CmdLineNamelessArgAttribute"/>
    /// and <see cref=" CmdLineArgAttribute"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CmdLine<T> where T : class
    {
        static public int Execute(string[] args, CmdLineConfig config = null)
        {
            return Execute(args, null, config ?? new CmdLineConfig());
        }

        static public int Execute(string[] args, T instance, CmdLineConfig config = null)
        {
            if (config == null)
                config = new CmdLineConfig();
            Instance = instance;
            CmdLineParser parser;
            var cd = new ClassDefinition(typeof(T), config);

            try
            {
                parser = new CmdLineParser(args, config);
            }
            catch (CmdLineArgException ex)
            {
                con.WriteAppHeader(cd);
                con.wl(ConsoleColor.Red, "{0}", ex.Message);
                con.WriteHelp(cd);
                return -1;
            }

            if (parser.IsHelpCommand)
            {
                con.WriteAppHeader(cd);
                if (parser.IsHelpCommand && parser.Command != null)
                    con.WriteHelp(cd, parser.Command);
                else
                    con.WriteHelp(cd);
                return 0;
            }

            var executor = new CmdLineExecutor(cd, new InstanceProvider(Instance));
            if (config.RequiresCommand)
            {
                if (parser.Command != null)
                {
                    CmdLineException exception = null;
                    try
                    {
                        if (executor.Execute(parser.Command, parser.Args, out object result))
                        {
                            return result != null && result.GetType().IsAssignableFrom(typeof(int))
                                ? (int)result
                                : 0;
                        }
                    }
                    catch (CmdLineException ex)
                    {
                        exception = ex;
                    }

                    con.WriteAppHeader(cd);
                    if (exception != null)
                    {
                        con.wl(ConsoleColor.Red, "{0}", exception.Message);
                        con.WriteHelp(cd, parser.Command);
                    }
                    else
                    {
                        con.w("Command ").w(ConsoleColor.Red, "{0}", parser.Command).wl(" not found");
                        con.WriteHelp(cd);
                    }
                    return -1;
                    // help on command.  parser.Args contain unknown parameters
                }
                else
                {
                    // general help
                }
            }
            else if (parser.Command == null)
            {
                if (Execute(null, cd, parser.Args, config, out object result))
                {
                    return result != null && result.GetType().IsAssignableFrom(typeof(int))
                        ? (int)result
                        : 0;
                }
                else
                {
                    // help on command.  parser.Args contain unknown parameters
                }
            }
            else
            {
                // unknown parameter, no command expected
            }

            con.WriteAppHeader(cd);
            if (parser.IsHelpCommand && parser.Command != null)
                con.WriteHelp(cd, parser.Command);
            else
                con.WriteHelp(cd);
            return 0;
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        static bool Execute(string command, ClassDefinition cd, IReadOnlyDictionary<string, string> inputArgs, CmdLineConfig config, out object result)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            var cl = new CmdLineExecutor(cd, new InstanceProvider(Instance));
            try
            {
                return cl.Execute(command, inputArgs, out result);
            }
            catch (CmdLineArgException ex)
            {
                con.wl(ConsoleColor.Red, "{0}", ex.Message);
                result = null;
                return false;
            }
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        static public void Validate()
        {
            var methods = typeof(T).GetMethods().Where(
                    mi => mi.GetCustomAttributes(false).Any(attr => attr is CmdLineMethodAttribute));
            foreach (var method in methods)
            {
                if (method.GetCustomAttributes(false).Count(attr => attr is CmdLineMethodAttribute) != 1)
                {
                    // error
                }
            }
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        class InstanceProvider : IInstanceProvider
        {
            public Type TypeOf { get { return typeof(T); } }
            public object Instance { get; private set; }

            public InstanceProvider(T instance)
            {
                Instance = instance;
            }

            public object GetSafeInstance()
            {
                if (Instance == null)
                    Instance = Activator.CreateInstance(typeof(T));
                return Instance;
            }
        }

        CmdLine()
        {
        }

        static public void DumpCmdLineTypeInfo(bool dumpAll = false)
        {
            IColorConsole con = ColorConsole.Default;
            Type type = typeof(T);
            var classAttrs = type.GetCustomAttributes(false).Where(a => a is CmdLineClassAttribute).ToList();
            if (classAttrs.Count == 0)
            {
                con.wl(ConsoleColor.Red, "{0} is not CmdLine class", type.Name);
                return;
            }
            foreach (var attr in classAttrs)
            {
                con.wl("{0}: {1}", type.Name, attr.GetType().Name);
            }
            foreach (var method in type.GetMethods())
            {
                var attrs = method.GetCustomAttributes(false);
                var methodAttrs = attrs.Where(a => a is CmdLineMethodAttribute).ToList();
                bool isCmdLineMethod = methodAttrs.Count > 0;
                var methodColor = isCmdLineMethod ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
                if (isCmdLineMethod || dumpAll)
                {
                    con.w(methodColor, "{0}()", method.Name);
                    if (isCmdLineMethod && methodAttrs.Count != 1)
                        con.wl(ConsoleColor.Yellow, ": too many attributes {0}", methodAttrs.Count);
                    else
                        con.wl();
                    foreach (var parameter in method.GetParameters())
                    {
                        var attrs1 = parameter.GetCustomAttributes(false);
                        var paramAttrs = attrs1.Where(a => a is CmdLineArgAttribute).ToList();
                        var paramColor = isCmdLineMethod ? ConsoleColor.White : ConsoleColor.DarkGray;
                        if (isCmdLineMethod)
                        {
                            con.w(paramColor, "  {0}", parameter.Name);
                            con.w("[{0}]", parameter.ParameterType.Name);
                            if (paramAttrs.Count == 0)
                                con.wl(ConsoleColor.Yellow, "  {0}: not a CmdLine parameter", parameter.Name);
                            else if (paramAttrs.Count != 1)
                                con.wl(ConsoleColor.Yellow, "  {0}: too many CmdLine parameters {1}", parameter.Name, paramAttrs.Count);
                            else
                            {
                                var attr = attrs1[0];
                                var a = attr as CmdLineArgAttribute;
                                if (a != null)
                                {
                                    con.wl(": {0} - {1}", a.ArgName, a.HelpText);
                                }
                                else
                                {
                                    con.wl(": {0}", a.HelpText);
                                }
                            }
                        }
                        else if (dumpAll)
                        {
                            con.wl(paramColor, "  {0}[{1}]", parameter.Name, parameter.ParameterType.Name);
                        }
                    }
                }

            }

            /*
            foreach (var attr in type.GetCustomAttributes(false).Where(a => a is CmdLineClassAttribute))
            {
                con.wl(ConsoleColor.Yellow, "{0}", attr.GetType().Name);
            }*/
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        static public void DumpTypeInfo()
        {
            IColorConsole con = ColorConsole.Default;
            Type type = typeof(T);
            foreach (var attr in type.GetCustomAttributes(false).Where(a => a is CmdLineBaseAttribute))
            {
                con.wl(ConsoleColor.Yellow, "{0}", attr.GetType().Name);
            }

            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes(false).Count(a => a is CmdLineMethodAttribute) == 1))
            {
                con.wl(ConsoleColor.Green, "{0}", method.Name);
                foreach (var attr in method.GetCustomAttributes(false).Where(m => m is CmdLineBaseAttribute))
                {
                    con.wl(ConsoleColor.DarkYellow, "   {0}", attr.GetType().Name);
                }
                foreach (var prm in method.GetParameters())
                {
                    con.wl(ConsoleColor.DarkMagenta, "   {0}", prm.Name);
                    foreach (var attr in prm.GetCustomAttributes(false).Where(m => m is CmdLineBaseAttribute))
                    {
                        con.wl(ConsoleColor.DarkYellow, "      {0}", attr.GetType().Name);
                    }
                }
            }
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        static T Instance;
        static IColorConsole con = ColorConsole.Default;
    }

}
