@echo off
echo Deleting Release stuff
for /d %%F in (_build\Release*) do del /s /q "%%F"

echo Building stuff
msbuild CmdLineLib\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.5.2
msbuild CmdLineLib\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.6.1 /p:CreatePackage=true /p:PublishDir=..\..\..\nuget\nupkgsrc

echo Deleting other stuff
del ..\..\nuget\nupkg\cmdlinelib\* /s /f /q

echo Packaging stuff
..\nuget.exe init ..\..\nuget\nupkgsrc ..\..\nuget\nupkg
