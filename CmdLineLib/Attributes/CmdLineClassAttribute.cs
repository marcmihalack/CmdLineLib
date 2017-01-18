using System;

namespace CmdLineLib.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CmdLineClassAttribute : CmdLineBaseAttribute
    {
        public string HelpText { get; protected set; }
        public InclusionBehavior InclusionBehavior { get; protected set; }

        public CmdLineClassAttribute(
            InclusionBehavior inclusionBehavior = InclusionBehavior.Default,
            string helpText = null)
        {
            HelpText = helpText;
            InclusionBehavior = inclusionBehavior;
        }
    }

}
