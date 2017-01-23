using System;

namespace CmdLineLib
{
    /**
     Ways to include/exclude methods/properties/fields:
     - Use CmdLineClassAttribute.InclusionBehavior to define default inclusion behavior
     - Use CmdLineExcludeAttribute to exclude specific member if it would be included by default inclusion behavior
     - Use CmdLineMethodAttribute to include method if it would be excluded by default inclusion behavior
     - Use CmdLineArgAttribute property/field to include class property/field if it would be excluded by default inclusion behavior
     */
    [Flags]
    public enum InclusionBehavior
    {
        ExcludeAll = 0,
        IncludeStaticMethods = 1,
        IncludeNonStaticMethods = 2,
        IncludeStaticProperties = 4,
        IncludeNonStaticProperties = 8,
        IncludeStaticFields = 16,
        IncludeNonStaticFields = 32,
        IncludeInherited = 64,

        IncludeAllMethods = IncludeNonStaticMethods | IncludeStaticMethods,
        IncludeAllProperties = IncludeNonStaticProperties | IncludeStaticProperties,
        IncludeAllFields = IncludeNonStaticFields | IncludeStaticFields,
        IncludeAllNonStatic = IncludeNonStaticMethods | IncludeNonStaticFields | IncludeNonStaticProperties,
        IncludeAllStatic = IncludeStaticMethods | IncludeStaticFields | IncludeStaticProperties,
        IncludeAll = IncludeAllNonStatic | IncludeAllStatic,
        Default = IncludeAllMethods | IncludeAllProperties,
    };

}
