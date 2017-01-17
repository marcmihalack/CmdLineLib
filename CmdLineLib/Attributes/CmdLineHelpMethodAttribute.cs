using System;

namespace CmdLineLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CmdLineHelpMethodAttribute : CmdLineBaseAttribute
    {
        public CmdLineHelpMethodAttribute()
        {
        }
    }
}
