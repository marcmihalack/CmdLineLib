using CmdLineLib.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CmdLineLib
{
    class CmdLineParameter
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public bool HasDefaultValue { get; private set; }
        public object DefaultValue { get; private set; }
        public CmdLineArgAttribute Attribute { get; private set; }
        public bool IsCommon { get { return Field == null && Property == null; } }

        public CmdLineConfig Config { get; }
        public string HelpText { get { return Attribute?.HelpText; } }

        FieldInfo Field;
        PropertyInfo Property;

        public CmdLineParameter(FieldInfo info, CmdLineArgAttribute attribute, CmdLineConfig config)
        {
            Field = info;
            Property = null;
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Name = attribute != null && attribute.ArgName != null
                ? attribute.ArgName
                : info.Name;
            Type = info.FieldType;
            Attribute = attribute;
            if (attribute != null && attribute.Default != null)
            {
                HasDefaultValue = true;
                DefaultValue = attribute.Default;
            }
            else if (info.IsStatic)
            {
                DefaultValue = info.GetValue(null);
                HasDefaultValue = DefaultValue != null;
            }
            else
            {
                // pi.GetValue() for non-static requires object
                HasDefaultValue = false;
                DefaultValue = null;
            }

            SetDefaultValue();
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        public CmdLineParameter(PropertyInfo info, CmdLineArgAttribute attribute, CmdLineConfig config)
        {
            Field = null;
            Property = info;
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Name = attribute != null && attribute.ArgName != null
                ? attribute.ArgName
                : info.Name;
            Type = info.PropertyType;
            Attribute = attribute;
            if (attribute != null && attribute.Default != null)
            {
                HasDefaultValue = true;
                DefaultValue = attribute.Default;
            }
            else if (info.GetGetMethod().IsStatic)
            {
                DefaultValue = info.GetValue(null, null);
                HasDefaultValue = DefaultValue != null;
            }
            else
            {
                // pi.GetValue() for non-static requires object
                HasDefaultValue = false;
                DefaultValue = null;
            }

            SetDefaultValue();
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        public CmdLineParameter(ParameterInfo info, CmdLineArgAttribute attribute, CmdLineConfig config)
        {
            Field = null;
            Property = null;
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Name = attribute != null && attribute.ArgName != null
                ? attribute.ArgName
                : info.Name;
            Type = info.ParameterType;
            Attribute = attribute;
            if (info.HasDefaultValue)
            {
                HasDefaultValue = info.HasDefaultValue;
                DefaultValue = info.DefaultValue;
            }
            else if (attribute != null && attribute.Default != null)
            {
                HasDefaultValue = true;
                DefaultValue = attribute.Default;
            }
            else
            {
                HasDefaultValue = false;
                DefaultValue = null;
            }
        }

        public void SetValue(object instance, object value)
        {
            if (Property != null)
                Property.SetValue(instance, value);
            else if (Field != null)
                Field.SetValue(instance, value);
            else
                throw new Exception("Cannot set value on parameter");
        }
        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        void SetDefaultValue()
        {
            if (HasDefaultValue && DefaultValue != null)
            {
                if (!DefaultValue.GetType().IsEquivalentTo(Type))
                    throw new CmdLineException("Invalid default value type");
            }
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        /*
        static CmdLineArgAttribute getArgAttribute(ICustomAttributeProvider attrProvider) //IEnumerable<CmdLineBaseAttribute> attributes
        {
            var attributes = attrProvider.GetCustomAttributes(typeof(CmdLineBaseAttribute), false).Select(p => p as CmdLineBaseAttribute);
            CmdLineArgAttribute argAttribute = null;
            foreach (var attribute in attributes)
            {
                if (attribute is CmdLineExcludeAttribute)
                    throw new CmdLineArgException("Attribute is excluded");
                else if (argAttribute != null)
                    throw new CmdLineArgException("Attribute already assigned");
                else
                {
                    argAttribute = attribute as CmdLineArgAttribute;
                    if (argAttribute == null)
                        throw new CmdLineArgException(string.Format("Unexpected attribute {0}", typeof(Attribute).Name));
                }
            }
            return argAttribute;
        }
        */
        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        //public static ByNameComparer CompareByName = new ByNameComparer();
        public class ByNameComparer : IEqualityComparer<CmdLineParameter>
        {
            public bool Equals(CmdLineParameter x, CmdLineParameter y)
            {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(CmdLineParameter obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        public bool TryGetParameterValue(bool isArgProvided, string parameterValue, out object value)
        {
            return InternalTryGetParameterValue(isArgProvided, parameterValue, false, out value);
        }

        public object GetParameterValue(bool isArgProvided, string parameterValue)
        {
            bool really = InternalTryGetParameterValue(isArgProvided, parameterValue, true, out object value);
            if (!really)
                throw new Exception("Invalid state");
            return value;
        }

        bool InternalTryGetParameterValue(bool isArgProvided, string parameterValue, bool throwOnError, out object value)
        {
            if (parameterValue != null)
            {
                if (Type.IsEquivalentTo(typeof(Boolean)))
                {
                    value = parameterValue.ToBool();
                }
                else
                { // /arg:value
                    TypeConverter typeConverter;
                    if (Type.IsArray)
                    {
                        var split = parameterValue.Split(Config.ArgListSeparator);
                        var elementType = Type.GetElementType();
                        var array = Array.CreateInstance(elementType, split.Length);
                        typeConverter = TypeDescriptor.GetConverter(elementType);
                        int i = 0;
                        try
                        {
                            for (; i < split.Length; i++)
                                array.SetValue(typeConverter.ConvertFromString(split[i]), i);
                        }
                        catch (NotSupportedException ex)
                        {
                            if (throwOnError)
                            {
                                int n = i + 1;
                                string s = (n == 1 ? "st" : (n == 2 ? "nd" : (n == 3 ? "rd" : "th")));
                                throw new CmdLineArgException(Name, $"Cannot convert argument {Name}'s {n}{s} element to {Type.Name}", ex);
                            }
                            value = null;
                            return false;
                        }
                        value = array;
                    }
                    else
                    {
                        typeConverter = TypeDescriptor.GetConverter(Type);
                        try
                        {
                            value = typeConverter.ConvertFromString(parameterValue);
                        }
                        catch (NotSupportedException ex)
                        {
                            if (throwOnError)
                                throw new CmdLineArgException(Name, $"Cannot convert argument {Name} to {Type.Name}", ex);
                            value = null;
                            return false;
                        }
                    }
                }
            }
            else // arg is null
            {
                if (Type.IsEquivalentTo(typeof(Boolean)))
                {
                    if (isArgProvided)
                        value = true;
                    else if (HasDefaultValue)
                        value = DefaultValue;
                    else
                        value = false;
                }
                else
                {
                    if (HasDefaultValue)
                        value = DefaultValue;
                    else
                    {
                        if (throwOnError)
                            throw new CmdLineArgException(Name, $"Argument '{Name}' requires value");
                        value = null;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
