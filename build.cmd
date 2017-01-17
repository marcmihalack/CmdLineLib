@echo off
msbuild CmdLineLib\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.5.2
msbuild CmdLineLib\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.6.1 /p:CreatePackage=true /p:PublishDir=..\..\..\nuget\nupkgsrc

del ..\..\nuget\nupkg\cmdlinelib\* /s /f /q

..\nuget.exe init ..\..\nuget\nupkgsrc ..\..\nuget\nupkg
