using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CmdLineLib
{
    static class TypeExtensions
    {
        static string[] StringToBoolTrues = new[] { "true", "yes", "1" };
        static string[] StringToBoolFalses = new[] { "false", "no", "0" };

        public static bool ToBool(this string value)
        {
            var value1 = value.ToLower();
            if (StringToBoolTrues.Contains(value1))
                return true;
            if (StringToBoolFalses.Contains(value1))
                return false;
            throw new CmdLineException("Invalid boolean value");
        }

    }

    static class Extensions
    {
        public const string DefaultIndent = "    ";
        public static string Indent = DefaultIndent;
        static ConsoleColor ExeColor = ConsoleColor.DarkGray;
        static ConsoleColor CommandColor = ConsoleColor.White;
        static ConsoleColor ParameterColor = ConsoleColor.Cyan;
        static ConsoleColor OptionalParameterColor = ConsoleColor.DarkCyan;
        static ConsoleColor DefaultValueColor = ConsoleColor.White;

        public static IColorConsole WriteAppHeader(this IColorConsole con, ClassDefinition cd)
        {
            con.w("{0}", cd.AppTitle).w(ConsoleColor.Green, $" {cd.Version}").wl($" {cd.Copyright}");
            if (cd.HelpText != null)
                con.wl("{0}", cd.HelpText);
            else if (!string.IsNullOrEmpty(cd.Description))
                con.wl("{0}", cd.Description);
            return con;
        }

        public static string ToDisplayString(this Type type, char argListSeparator)
        {
            var name = type.Name.ToLower();
            if (type.IsArray)
            {
                var ename = type.GetElementType().ToDisplayString(argListSeparator);
                return string.Concat("{", ename, ",", ename, ",...}");
            }
            if (type.IsEquivalentTo(typeof(Boolean)))
                return "<flag>";
            var rx = new Regex(@"^([a-z]+)[0-9]{1,}$");
            var match = rx.Match(name);
            if (match.Success)
                return match.Groups[1].Value;
            return name;
        }

        public static string ToDisplayName(this Type type)
        {
            if (type.IsArray)
                return string.Concat(type.GetElementType().ToDisplayName(), "[]");
            if (type.IsEquivalentTo(typeof(Boolean)))
                return "flag";

            var name = type.Name.ToLower();
            var rx = new Regex(@"^([a-z]+)[0-9]{1,}$");
            var match = rx.Match(name);
            if (match.Success)
                return match.Groups[1].Value;

            return name;
        }

        public static string DefaultValueToDisplayString(this CmdLineParameter p)
        {
            if (p.HasDefaultValue)
            {
                if (p.Type.IsEquivalentTo(typeof(Boolean)))
                    return p.DefaultValue.ToString().ToLower();
                return p.DefaultValue?.ToString();
            }
            return null;
        }

        public static string TypeToDisplayString(this CmdLineParameter p)
        {
            var type = p.Type;
            if (type.IsArray)
            {
                var ename = type.GetElementType().ToDisplayString(p.Config.ArgListSeparator);
                return string.Concat("{", ename, ",...}");
            }
            if (type.IsEquivalentTo(typeof(Boolean)))
                return "<flag>";

            var name = type.Name.ToLower();
            var rx = new Regex(@"^([a-z]+)[0-9]{1,}$");
            var match = rx.Match(name);
            if (match.Success)
            {
                return string.Concat("<", match.Groups[1].Value, ">");
            }
            return string.Concat("<", name, ">");
        }

        static IColorConsole WriteShortHelp(this IColorConsole con, CmdLineParameter p)
        {
            bool optional = p.HasDefaultValue;
            var color = optional ? OptionalParameterColor : ParameterColor;
            con.w(" ");
            if (p.Type.IsEquivalentTo(typeof(Boolean)))
            {
                con.w("[{0}", p.Config.ArgStartsWith).w(OptionalParameterColor, p.Name).w("]");
                if (optional)
                    con.w("(default=").w(DefaultValueColor, "{0}", p.DefaultValueToDisplayString()).w(")");
            }
            else
            {
                if (optional)
                    con.w("[");
                con.w("{0}", p.Config.ArgStartsWith).w(color, p.Name).w("=").w("{0}", p.TypeToDisplayString());
                if (optional)
                {
                    if (!p.Type.IsEquivalentTo(typeof(string)) || p.DefaultValue != null)
                        con.w("(default=").w(DefaultValueColor, "{0}", p.DefaultValueToDisplayString()).w(")");
                    con.w("]");
                }
            }
            return con;
        }

        static IColorConsole WriteHelp(this IColorConsole con, CmdLineParameter p)
        {
            bool optional = p.HasDefaultValue;
            bool isFlag = p.Type.IsEquivalentTo(typeof(Boolean));
            var color = optional || isFlag ? OptionalParameterColor : ParameterColor;

            con.w(Indent).w(color, p.Name.PadRight(7)).w(" - {0}", p.Type.ToDisplayName());
            if (p.HelpText != null)
                con.w(p.HelpText);

            if (p.Type.IsEquivalentTo(typeof(Boolean)))
            {
                if (optional)
                {
                    con.w("(default=").w(DefaultValueColor, "{0}", p.DefaultValueToDisplayString()).w(")");
                }
            }
            else
            {
                if (optional)
                {
                    if (!p.Type.IsEquivalentTo(typeof(string)) || p.DefaultValue != null)
                        con.w(" (default=").w(DefaultValueColor, "{0}", p.DefaultValueToDisplayString()).w(")");
                    //con.w("]");
                }
            }
            con.wl();
            return con;
        }

        public static IColorConsole WriteHelp(this IColorConsole con, ClassDefinition cd, string command)
        {
            var methods = cd.FindMethods(command);
            if (methods.Length == 0)
            {
                con.w("Help for ").w(ConsoleColor.Red, "{0}", command).wl(" not found");
            }
            else
            {
                char argStartsWith = cd.Config.ArgStartsWith;
                char argSeparator = cd.Config.ArgSeparator;
                char argListSeparator = cd.Config.ArgListSeparator;
                foreach (var method in methods)
                {
                    con.w(ExeColor, "{0}", cd.ExeName);
                    con.w(CommandColor, " {0}", method.Command);
                    foreach (var group in method.Parameters.GroupBy(p => !p.HasDefaultValue))
                    {
                        bool optionals = group.Key;
                        foreach (var parameter in group)
                            con.WriteShortHelp(parameter);
                    }
                    con.wl();
                    if (method.HelpText != null)
                        con.w(Indent).wl("{0}", method.HelpText);
                    foreach (var group in method.Parameters.GroupBy(p => !p.HasDefaultValue))
                    {
                        bool optionals = group.Key;
                        foreach (var parameter in group)
                            con.WriteHelp(parameter);
                    }
                    //con.wl();
                }
            }
            if (cd.CommonParameters.Length > 0)
            {
                con.wl("Common parameters:");
                foreach (var parameter in cd.CommonParameters)
                    con.WriteHelp(parameter);
            }
            return con;
        }

        public static IColorConsole WriteHelp(this IColorConsole con, ClassDefinition cd)
        {
            foreach (var method in cd.IncludedMethods)
            {
                con.w(ExeColor, "{0}", cd.ExeName).w(CommandColor, " {0}", method.Command);
                foreach (var group in method.Parameters.GroupBy(p => !p.HasDefaultValue))
                {
                    bool optionals = group.Key;
                    foreach (var parameter in group)
                        con.WriteShortHelp(parameter);
                }
                con.wl();
            }

            if (cd.CommonParameters.Length > 0)
            {
                con.w("Common parameters:");
                foreach (var parameter in cd.CommonParameters)
                    con.WriteShortHelp(parameter);
                con.wl();
            }
            return con;
        }
    }
}
