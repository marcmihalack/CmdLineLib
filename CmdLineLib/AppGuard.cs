using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CmdLineLib
{
    public class AppGuard
    {
        /// <summary> Executes the method and prints exception information if Exception happens.
        /// Use in Program.Main() like this:
        /// Exec.ExecMain(() => DoWhateverTheHellYouWant());
        /// or
        /// Exec.ExecMain(() => DoWhateverTheHellYouWantWithArgs(args));
        /// </summary>
        /// <param name="exec"></param>
        public static void Invoke(Action exec)
        {
            try
            {
                exec();
            }
            catch (CmdLineException ex)
            {
                con.wl(ConsoleColor.Red, "{0}", ex.Message);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException == null)
                {
                    con.wl(ConsoleColor.Red, "{0}", ex.Message);
                    con.wl("{0}", ex);
                }
                else
                {
                    var clex = ex.InnerException as CmdLineException;
                    if (clex != null)
                    {
                        con.wl(ConsoleColor.Red, "{0}", ex.Message);
                    }
                    else
                    {
                        con.wl(ConsoleColor.Red, "{0}", ex.Message);
                        con.wl("{0}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                con.wl(ConsoleColor.Red, "{0}", ex.Message);
                con.wl("{0}", ex);
            }
        }

        public static void WriteException(Exception ex, TextWriter writer)
        {
            Exception ex1 = ex;
            string indent = "";
            while (ex1 != null)
            {
                writer.WriteLine("{0}{1}", indent, ex1.Message);
                ex1 = ex1.InnerException;
                indent += "   ";
            }
            writer.WriteLine(ex.StackTrace);
        }

        [Conditional("DEBUG")]
        static public void DebugReadLine()
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }

        static IColorConsole con = ColorConsole.Default;
    }
}
