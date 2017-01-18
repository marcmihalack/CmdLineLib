#CmdLineLib
Command line library with strongly typed mapping of app input arguments to method parameters.

###Quick Start
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

###Configuration
**`CmdLineConfig`** allows to configure how paramaters are interpreted.

All arguments following _**command**_ must start with a `CmdLineConfig.ArgStartsWith` character (default '/') and separated with `CmdLineConfig.ArgSeparator` (default '=')

```csharp
CmdLine<Program>.Execute(args, new CmdLineConfig { ArgStartsWith = '-'});
```
Now you need to call

	net.exe ping -address=192.168.1.1

Values of array parameters must be separated with `CmdLineConfig.ArgListSeparator` (default ','), and spaces are not allowed between values:

	net.exe ping /addresses=192.168.1.1,192.168.1.2,192.168.1.99

will call Ping() method (by default letter casing doesn't matter - you can change this behavior with `CmdLineConfig.NameComparison`) defined as:
```csharp
public static void Ping(string[] addresses, int count = 4) { ... }
```

By default all public class methods and properties, both static and non-static, of class **T** are included in processing.  You can change the default inclusion behavior with `CmdLineClassAttribute` (see **Member Inclusion** below)


###Attributes
Use **CmdLine*Attribute** attributes to override method or argument name, provide additional help text, provide default value, exclude member from **CmdLine** processing, and whatnot.

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

###Member Inclusion
- All public members if the class **T** can be included in **CmdLine\<T\>** processing.  By default all public class methods and properties, both static and non-static, are included.
- You can specify **default inclusion behavior** by passing `InclusionBehavior` flags to `CmdLineClassAttribute` constructor.
- You can specifically exclude member from processing by adding `CmdLineExcludeAttribute` to the member.
- You can specifically include property/field by adding `CmdLineArgAttribute` or method by adding `CmdLineMethodAttribute` - even if the member would not be included by **default inclusion behavior**.
- You can use either/or static and non-static members.  For non-static, an instance of class **T** will be created (class is requires to have a default constructor), or optionally you can provide your class instance to the `CmdLine\<T\>.Execute()` method.
- `// TODO: add factory interface/method to allow caller create instance based on arguments`

###Help
By default **CmdLine** generates help based on the method and parameters' names.  You can provide additional information for class, method or argument by passing `helpText` parameter to the construtor of the corresponding attribute.

You can use "**help**" command to display help information:

	net.exe help

`// TODO: provide output of help command`

###AppGuard
Not required but very helpful **AppGuard** catches all exceptions and displays error with optional stack trace, so you don't need to worry about unhandled exceptions.

```csharp
static int Main(string[] args)
{
	AppGuard.Invoke(() => CmdLine<Program>.Execute(args));
}
```
