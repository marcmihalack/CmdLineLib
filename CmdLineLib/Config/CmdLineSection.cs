using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdLineLib.Config
{
    public class CmdLine : ConfigurationSection
    {
        [ConfigurationProperty("argStartsWith", DefaultValue = "/", IsRequired = false)]
        public char ArgStartsWith
        {
            get { return (char)this["argStartsWith"]; }
            set { this["argStartsWith"] = value; }
        }

        [ConfigurationProperty("argSeparator", DefaultValue = "=", IsRequired = false)]
        public char ArgSeparator
        {
            get { return (char)this["argSeparator"]; }
            set { this["argSeparator"] = value; }
        }

        [ConfigurationProperty("argListSeparator", DefaultValue = "=", IsRequired = false)]
        public char ArgListSeparator
        {
            get { return (char)this["argListSeparator"]; }
            set { this["argListSeparator"] = value; }
        }
        /*
        [ConfigurationProperty("nameComparison", DefaultValue = "=", IsRequired = false)]
        [TypeConverter(typeof(StringComparison))]
        public StringComparison NameComparison
        {
            get { return (StringComparison)this["nameComparison"]; }
            set { this["nameComparison"] = value; }
        }
        */
        [ConfigurationProperty("Parameters", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ParametersCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ParametersCollection Parameters { get { return (ParametersCollection)base["Parameters"]; } }
    }

    public class ParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("value", IsRequired = false)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }

    public class ParametersCollection : ConfigurationElementCollection
    {
        public ParameterElement this[int index]
        {
            get { return (ParameterElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public void Add(ParameterElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterElement)element).Name;
        }

        public void Remove(ParameterElement element)
        {
            BaseRemove(element.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
