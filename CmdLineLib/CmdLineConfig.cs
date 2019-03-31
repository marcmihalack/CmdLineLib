using System;

namespace CmdLineLib
{
    /// <summary>
    /// Configuration settings for CmdLine<>.Execute() method.
    /// </summary>
    public class CmdLineConfig
    {
        public CmdLineConfig()
        {
            NameComparison = StringComparison.CurrentCultureIgnoreCase;
            ArgListSeparator = ',';
            ArgStartsWith = SystemConsole.IsTTY ? '-' : '/';
            ArgSeparator = '=';
            RequiresCommand = true;
        }

        /// <summary>
        /// Character used to indicate start of an argument on a command line.
        /// All arguments after 'command' argument must start with this character.
        /// Default '/'
        /// </summary>
        public char ArgStartsWith { get; set; }
        /// <summary>
        /// Character used as separator of an argument name and its value on a command line.
        /// All arguments except 'command' argument and boolean arguments must contain separator.
        /// Default '='
        /// </summary>
        public char ArgSeparator { get; set; }
        /// <summary>
        /// Character used as separator of an array of values for array type arguments.
        /// Default ','
        /// </summary>
        /// <remarks>
        /// You can define method argument as an array of elements (ex.: int[] arg) and pass /arg=1,2,3,4 and values will be converted to an array with provided values.
        /// </remarks>
        public char ArgListSeparator { get; set; }
        /// <summary>
        /// Comparison used for method and argument name comparsion. Default CurrentCultureIgnoreCase
        /// </summary>
        public StringComparison NameComparison { get; set; }
        internal bool RequiresCommand { get; set; }
    }
}
