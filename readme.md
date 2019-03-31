# CmdLineLib

Command line library with strongly typed mapping of app input arguments to method parameters.

### What's new

- 1.2.0:
    - Added SystemConsole handling common system console features
    - Added AnsiColorConsole to support ANSI console colors
    - Changing default ArgStartsWith to be '/' for non-TTY and '-' for TTY console
      Does not affect property value if explicitly set
- 1.1.3:
    - Added descriptive exception message for invalid default value type.
- 1.1.2:
    - Fixed nullable display.
- 1.1.1:
    - Fixed handling of enum types when invalid value passed.
   - Fixed handling of enum type with FlagsAttribute and multiple flags passed.
- 1.1.0:
   - Added IncludeInherited inclusion flags; inherited members are excluded by default.
   - Fixed arg not found bug due to incorrect name comparison used.

### Quick start

In a console app project define and implement the following method in Program class:

```csharp
	class Program
	{
		public static void ping(string address, int count = 4)
		{
			// ...
		}

		static int Main(string[] args)
		{
			CmdLine<Program>.Execute(args);
		}
	}
```

Let's assume the project generates `net.exe` executable.  Than executing `net.exe` as:

	net.exe ping /address=192.168.1.1

will execute:

	Program.ping("192.168.1.1", 4);

And:

	net.exe ping /address=192.168.1.1 /count=many

will result in _Invalid argument **count**_ error message (since "**many**" cannot be converted to **int** parameter)

Behind the scenes **CmdLine\<T\>.Execute()** analyzes class T public members, matches a method that corresponds to the arguments provided on a command line and, if a match is found, converts the arguments to the parameters' types, and executes found method.

Command line arguments start with _**command**_ argument - a single word - which corresponds to requested method name or `command` argument of `CmdLineMethodAttribute` constructor.

### Configuration

**`CmdLineConfig`** allows to configure how paramaters are interpreted.

All arguments following _**command**_ must start with a `CmdLineConfig.ArgStartsWith` character (default '/') and separated with `CmdLineConfig.ArgSeparator` (default '=')

```csharp
CmdLine<Program>.Execute(args, new CmdLineConfig { ArgStartsWith = '-'});
```
Now you need to call

	net.exe ping -address=192.168.1.1

Starting from version 1.2.0 the default value of `CmdLineConfig.ArgStartsWith` is set to '/' for DOS cmd prompt, and '-' for bash interpreters (cygwin, msys2, etc.).

Values of array parameters must be separated with `CmdLineConfig.ArgListSeparator` (default ','), and spaces are not allowed between values:

	net.exe ping /addresses=192.168.1.1,192.168.1.2,192.168.1.99

will call Ping() method (by default letter casing doesn't matter - you can change this behavior with `CmdLineConfig.NameComparison`) defined as:
```csharp
public static void Ping(string[] addresses, int count = 4) { ... }
```

By default all public class methods and properties, both static and non-static, of class **T** are included in processing.  You can change the default inclusion behavior with `CmdLineClassAttribute` (see **Member Inclusion** below)


### Attributes

Use **CmdLine*Attribute** attributes to override method or argument name, provide additional help text, provide default value, exclude member from **CmdLine** processing, and whatnot.
However none of them are required - all of the values (command, parameter names, types and default values, if any) are grabbed from code metadata by using [Reflection](https://msdn.microsoft.com/en-us/library/f7ykdhsy.aspx).  Attributes allow you to provide additional information (like help text) or to override names and default parameter values.

**CmdLineArgAttribute** can be applied to any included method's parameter or included class field or property.
```csharp
public static void ping([CmdLineArg("addr")]string address, int count = 4) { ... }
```

**CmdLineMethodAttribute** can be applied to any included method
```csharp
[CmdLineMethod("tracert")]
public static void TraceRoute([CmdLineArg("addr")]string address, int count = 4) { ... }
```

**CmdLineClassAttribute** can be applied to the class and allows to specify member inclusion behavior (see below) or to provide help text.
```csharp
[CmdLineClass(inclusionBehavior = InclusionBehavior.AllStatic)]
public class Program { ... }
```

**CmdLineExcludeAttribute** can be applied to any public class member and it tell **CmdLine** to ignore this member if it were to be included by processing.
```csharp
[CmdLineExclude]
public static void TraceRoute(string address, int count, int someOtherParameter) { ... }
```

### Arguments

Arguments (method parameters and/or class properties or fields included by CmdLine) can be:
 - a value type (**string**, **int**, etc.)
 - **boolean**, which is also a value type, but it does not require a value, so it acts as a switch: if it's there it's **on** (or **true**), if it's not there then it's **off** (or **false**): `/recursive`.  You can also specify a value if you want to be sure: `/recursive=false`.  And yes, it can have values **on**/**off**, **yes**/**no**, **true**/**false**, **0**/**1**
 - an array or value types (**string[]**, **int[]**, etc.), where arg values are separated by **commas** *without* spaces, e.i.: ` /values=1,3,4,5 `
 - **enum** with case insensitive comparison `/verbosity=quiet`
 - flags (**enum** type with [System.FlagsAttribute](https://msdn.microsoft.com/en-us/library/system.flagsattribute.aspx), where arg values are separated by **commas** *without* spaces (nope, it is not a pipe '|' character for it would have to be escaped), e.i.: ` /flags=F1,F2 `
 - any other type that can be converted from a **string** using a [TypeConverter](https://msdn.microsoft.com/en-us/library/system.componentmodel.typeconverter.aspx).

CmdLine retrieves type converter by calling [TypeDescriptor.GetConverter()](https://msdn.microsoft.com/en-us/library/system.componentmodel.typedescriptor.getconverter.aspx) method.
You can [create a custom type converter](https://msdn.microsoft.com/en-us/library/ayybcxe5.aspx) and it might work... I think.

#### Argument default values

Argument is not required if its corresponding class member (method parameter, class property or field) has a default value.  Optionally you can use **CmdLineArgAttribute** to set a default value.

### Member inclusion

- All public members if the class **T** can be included in **CmdLine\<T\>** processing.  By default all public class methods and properties, both static and non-static, are included.
- You can specify **default inclusion behavior** by passing `InclusionBehavior` flags to `CmdLineClassAttribute` constructor.
- You can specifically exclude member from processing by adding `CmdLineExcludeAttribute` to the member.
- You can specifically include property/field by adding `CmdLineArgAttribute` or method by adding `CmdLineMethodAttribute` - even if the member would not be included by **default inclusion behavior**.
- You can use either/or static and non-static members.  For non-static, an instance of class **T** will be created (class is requires to have a default constructor), or optionally you can provide your class instance to the `CmdLine\<T\>.Execute()` method.
- `// TODO: add factory interface/method to allow caller create instance based on arguments`

### Help

By default **CmdLine** generates help based on the method and parameters' names.
You can provide additional information for class, method or argument by passing `helpText` parameter to the construtor of the corresponding attribute:
```csharp
[CmdLineMethod(helpText:"Pings host 'count' number of times")]
public static void Ping(
	[CmdLineArg("host", helpText:"IP address or DNS name of the host to ping")]string address
	, int count = 4)
	{
	    // ...
	}
```

You can use "**help**" command to display help information:

	net.exe help

`// TODO: provide output of help command`

### AppGuard

Not required but very helpful **AppGuard** catches all exceptions and displays error with optional stack trace, so you don't need to worry about unhandled exceptions.

```csharp
static int Main(string[] args)
{
	AppGuard.Invoke(() => CmdLine<Program>.Execute(args));
}
```
