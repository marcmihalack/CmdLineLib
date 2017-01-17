using System;
using System.Collections.ObjectModel;

namespace CmdLineLib.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    public class CmdLineArgAttribute : CmdLineBaseAttribute
    {
        public bool HasDefault { get; private set; }
        public object Default { get; private set; }
        public string HelpText { get; private set; }

        public string ArgName { get; private set; }
        public ReadOnlyCollection<object> Options { get; private set; }

        public CmdLineArgAttribute(string argName = null, object defaultValue = null, string helpText = null)
        {
            HasDefault = defaultValue != null;
            Default = defaultValue;
            ArgName = argName;
            HelpText = helpText;
        }

        /*
        public CmdLineNamedArgAttribute(string ArgName, object DefaultValue = null, string HelpText = null, object[] Options = null)
            : base(DefaultValue, HelpText)
        {
            this.ArgName = ArgName;
            if (Options != null)
            {
                Type type = null;
                foreach (var option in Options)
                {
                    if (type == null)
                        type = option.GetType();
                    else if (!option.GetType().IsEquivalentTo(type))
                        throw new CmdLineException("Inconsistent options types");
                }
                this.Options = Options;
            }
        }*/

        // Required with value and optional default: name(type):value[(=default)]
        // Optional [name(type):value[(=default)]]
        // Required with no value: name
        // optional with no value [name
        /*
        public override string ToString()
        {
            string s;
            if (Flags.HasFlag(ArgFlags.RequiresValue))
            {
                s = string.Format("{0}({1}):value", ArgName, "");
                if (Default != null)
                    s += string.Format("(={0})", Default.ToString());
            }
            else
            {
                s = string.Format("{0}", ArgName);
            }
            if(Flags.HasFlag(ArgFlags.Optional))
                s = string.Format("[{0}]",s);
            return s;
        }
        // */
        // format: s - short, m - medium, l - long
        public string ToString(string format, char argStartsWith, char argSeparator)
        {
            /*
            string s;
            if (format == "s")
            {
                s = string.Format(Flags.HasFlag(ArgFlags.RequiresValue) ? (argStartsWith + "{0}" + argSeparator + "value") : (argStartsWith + "{0}"), ArgName);
                return Flags.HasFlag(ArgFlags.Optional) ? string.Format("[{0}]", s) : s;
            }
            if (format == "m")
            {
                s = Flags.HasFlag(ArgFlags.RequiresValue)
                    ? string.Format(argStartsWith + "{0}" + argSeparator, ArgName)
                    : string.Format(argStartsWith + "{0}", ArgName);
                return Flags.HasFlag(ArgFlags.Optional) ? string.Format("[{0}]", s) : s;
            }
            if (format == "l")
                return ToString();
            throw new FormatException();
            */
            return ToString();
        }
        // */
    }
}
