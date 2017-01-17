#CmdLineLib
Command line library with strongly typed mapping of app input arguments to method parameters.

###Usage
For an app net.exe define and implement the following method in Program class:
> public static void ping(string address, int count = 4) { ... }

In `Main(string[] args)`
> CmdLine<Program>.Execute(args);

This will allow to call net.exe as:
> net.exe ping /address=192.168.1.1

###Configuration
`CmdLineConfig` allows to configure how paramaters are interpreted.

###AppGuard
Not required but very helpful `AppGuard` catches all exceptions and displays error with optional stack trace, so you don't need to worry about unhandled exceptions.

Use it in Main() along with CmdLine class:
> AppGuard.Run(() => CmdLine<Program>.Execute(args));

