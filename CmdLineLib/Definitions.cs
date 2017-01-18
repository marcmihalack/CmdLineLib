using CmdLineLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CmdLineLib
{
    class ClassDefinition
    {
        public Type Type { get; }
        public string AppTitle { get; }
        public Version Version { get; }
        public string Copyright { get; }
        public string Description { get; }
        public CmdLineClassAttribute Attribute { get; }
        public string HelpText { get { return Attribute?.HelpText; } }
        public string ExeName { get; }
        public InclusionBehavior InclusionBehavior { get; }
        public CmdLineConfig Config { get; }

        public MethodDefinition[] IncludedMethods { get { return m_IncludedMethods ?? (m_IncludedMethods = GetIncludedMethods().ToArray()); } }
        PropertyDefinition[] IncludedProperties { get { return m_IncludedProperties ?? (m_IncludedProperties = GetIncludedProperties().ToArray()); } }
        FieldDefinition[] IncludedFields { get { return m_IncludedFields ?? (m_IncludedFields = GetIncludedFields().ToArray()); } }
        public CmdLineParameter[] CommonParameters { get { return m_CommonParameters ?? (m_CommonParameters = GetCommonParameters().ToArray()); } }

        MethodDefinition[] m_IncludedMethods = null;
        PropertyDefinition[] m_IncludedProperties = null;
        FieldDefinition[] m_IncludedFields = null;
        CmdLineParameter[] m_CommonParameters = null;

        public ClassDefinition(Type type, CmdLineConfig config)
        {
            Config = config;
            Type = type;
            Attribute = Type.GetCustomAttributes(typeof(CmdLineClassAttribute), false)
                .FirstOrDefault() as CmdLineClassAttribute;
            InclusionBehavior = Attribute != null
                ? Attribute.InclusionBehavior
                : InclusionBehavior.Default;

            var ass = Assembly.GetEntryAssembly();
            ExeName = System.IO.Path.GetFileName(ass.Location);
            Version = ass.GetName().Version;
            var attrs = ass.GetCustomAttributes(true);
            AssemblyDescriptionAttribute vd = null;
            AssemblyTitleAttribute vt = null;
            AssemblyCopyrightAttribute vc = null;
            foreach (var attr in attrs)
            {
                if (vd == null)
                {
                    vd = attr as AssemblyDescriptionAttribute;
                    if (vd != null)
                    {
                        Description = vd.Description;
                        continue;
                    }
                }
                if (vt == null)
                {
                    vt = attr as AssemblyTitleAttribute;
                    if (vt != null)
                    {
                        AppTitle = vt.Title;
                        continue;
                    }
                }
                if (vc == null)
                {
                    vc = attr as AssemblyCopyrightAttribute;
                    if (vc != null)
                    {
                        Copyright = vc.Copyright;
                        continue;
                    }
                }
            }
        }

        public MethodDefinition[] FindMethods(string command)
        {
            var methods = Type.GetMethods();
            List<MethodDefinition> matchingMethods = new List<MethodDefinition>();
            foreach (MethodDefinition md in IncludedMethods)
            {
                if (md.Attribute != null && md.Attribute.Command != null)
                {
                    if (md.Attribute.Command.Equals(command, Config.NameComparison))
                        matchingMethods.Add(md);
                }
                else if (md.Method.Name.Equals(command, Config.NameComparison))
                    matchingMethods.Add(md);
            }

            return matchingMethods.ToArray();
        }

        IEnumerable<MethodDefinition> GetIncludedMethods()
        {
            var methods = Type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => !m.IsSpecialName);
            foreach (var mi in methods)
            {
                bool isIncludedByBehavior = (mi.IsStatic
                    ? (InclusionBehavior & InclusionBehavior.IncludeStaticMethods)
                    : (InclusionBehavior & InclusionBehavior.IncludeNonStaticMethods)) != 0;
                if (IsMemberIncluded(mi, isIncludedByBehavior, out CmdLineMethodAttribute attribute))
                    yield return new MethodDefinition(mi, attribute, Config);
            }
        }

        IEnumerable<PropertyDefinition> GetIncludedProperties()
        {
            var items = Type.GetProperties();
            foreach (var item in items)
            {
                bool isIncludedByBehavior = (item.GetGetMethod().IsStatic
                    ? (InclusionBehavior & InclusionBehavior.IncludeStaticProperties)
                    : (InclusionBehavior & InclusionBehavior.IncludeNonStaticProperties)) != 0;
                if (IsMemberIncluded(item, isIncludedByBehavior, out CmdLineArgAttribute attribute))
                    yield return new PropertyDefinition(item, attribute);
            }
        }

        IEnumerable<CmdLineParameter> GetCommonParameters()
        {
            var properties = Type.GetProperties();
            foreach (var property in properties)
            {
                bool isIncludedByBehavior = (property.GetGetMethod().IsStatic
                    ? (InclusionBehavior & InclusionBehavior.IncludeStaticProperties)
                    : (InclusionBehavior & InclusionBehavior.IncludeNonStaticProperties)) != 0;
                if (IsMemberIncluded(property, isIncludedByBehavior, out CmdLineArgAttribute attribute))
                    yield return new CmdLineParameter(property, attribute, Config);
            }
            var fields = Type.GetFields();
            foreach (var field in fields)
            {
                bool isIncludedByBehavior = (field.IsStatic
                    ? (InclusionBehavior & InclusionBehavior.IncludeStaticFields)
                    : (InclusionBehavior & InclusionBehavior.IncludeNonStaticFields)) != 0;
                if (IsMemberIncluded(field, isIncludedByBehavior, out CmdLineArgAttribute attribute))
                    yield return new CmdLineParameter(field, attribute, Config);
            }
        }

        IEnumerable<FieldDefinition> GetIncludedFields()
        {
            var items = Type.GetFields();
            foreach (var item in items)
            {
                bool isIncludedByBehavior = (item.IsStatic
                    ? (InclusionBehavior & InclusionBehavior.IncludeStaticFields)
                    : (InclusionBehavior & InclusionBehavior.IncludeNonStaticFields)) != 0;
                if (IsMemberIncluded(item, isIncludedByBehavior, out CmdLineArgAttribute attribute))
                    yield return new FieldDefinition(item, attribute);
            }
        }

        static bool IsMemberIncluded<TMember, TAttribute>(TMember member, bool isIncludedByBehavior, out TAttribute memberAttribute)
            where TAttribute : CmdLineBaseAttribute
            where TMember : MemberInfo
        {
            if (isIncludedByBehavior)
            {
                var exclusionAttribute = member.GetCustomAttributes(typeof(CmdLineExcludeAttribute), true)
                    .FirstOrDefault() as CmdLineExcludeAttribute;
                memberAttribute = member.GetCustomAttributes(typeof(TAttribute), true)
                    .FirstOrDefault() as TAttribute;
                return exclusionAttribute == null;
            }
            else
            {
                var attribute = member.GetCustomAttributes(typeof(TAttribute), true)
                    .FirstOrDefault() as TAttribute;
                memberAttribute = attribute;
                return attribute != null;
            }
        }
    }

    class MethodDefinition
    {
        public MethodDefinition(MethodBase method, CmdLineMethodAttribute attribute, CmdLineConfig config)
        {
            Method = method;
            Attribute = attribute;
            Config = config;
        }

        public string Command { get { return (Attribute == null || Attribute.Command == null) ? Method.Name : Attribute.Command; } }
        public CmdLineConfig Config { get; }
        public MethodBase Method { get; }
        public CmdLineMethodAttribute Attribute { get; }
        public CmdLineParameter[] Parameters { get { return m_Parameters ?? (m_Parameters = GetParameters()); } }
        public string HelpText { get { return Attribute?.HelpText; } }

        CmdLineParameter[] m_Parameters = null;
        CmdLineParameter[] GetParameters()
        {
            return Method.GetParameters()
                .Select(pi => new CmdLineParameter(pi, pi.GetCustomAttributes(typeof(CmdLineArgAttribute), true).FirstOrDefault() as CmdLineArgAttribute, Config))
                .ToArray();
        }
    }

    class PropertyDefinition
    {
        public PropertyDefinition(PropertyInfo property, CmdLineArgAttribute attribute)
        {
            Property = property;
            Attribute = attribute;
        }
        public PropertyInfo Property { get; private set; }
        public CmdLineArgAttribute Attribute { get; private set; }
    }

    class FieldDefinition
    {
        public FieldDefinition(FieldInfo field, CmdLineArgAttribute attribute)
        {
            Field = field;
            Attribute = attribute;
        }
        public FieldInfo Field { get; private set; }
        public CmdLineArgAttribute Attribute { get; private set; }
    }
}
