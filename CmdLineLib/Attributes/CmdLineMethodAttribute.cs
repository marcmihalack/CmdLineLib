using System;

namespace CmdLineLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CmdLineMethodAttribute : CmdLineBaseAttribute
    {
        public string Command { get; private set; }
        public string HelpText { get; private set; }
        public CmdLineMethodAttribute(string command = null, string helpText = null)
        {
            Command = command;
            HelpText = helpText;
        }
    }

}
