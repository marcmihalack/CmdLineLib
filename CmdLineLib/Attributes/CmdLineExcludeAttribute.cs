using System;

namespace CmdLineLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false)]
    public class CmdLineExcludeAttribute : CmdLineBaseAttribute
    {
    }
}
