using System;

namespace CmdLineLib.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CmdLineClassAttribute : CmdLineBaseAttribute
    {
        public string HelpArgName { get; protected set; }
        public string HelpText { get; protected set; }
        public InclusionBehavior InclusionBehavior { get; protected set; }

        public CmdLineClassAttribute(
            InclusionBehavior InclusionBehavior = InclusionBehavior.Default,
            string HelpText = null,
            string HelpArgName = "help")
        {
            this.HelpArgName = HelpArgName;
            this.HelpText = HelpText;
            this.InclusionBehavior = InclusionBehavior;
        }
    }

}
