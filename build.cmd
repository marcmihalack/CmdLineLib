@echo off
echo Deleting Release stuff
for /d %%F in (%~dp0_build\Release*) do del /s /q "%%F"
echo Deleting NuGet stuff
del %~dp0..\..\nuget\nupkgsrc\CmdLineLib* /s /f /q
::GOTO EOF
echo Building stuff
::msbuild CmdLineLib\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.5.2
::msbuild CmdLineLib\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.6.1 /p:CreatePackage=true /p:PublishDir=%~dp0..\..\..\nupkgsrc

msbuild CmdLineLib.Tests\CmdLineLib.Tests.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.5.2
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
msbuild CmdLineLib.Tests\CmdLineLib.Tests.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.6.1 /p:CreatePackage=true /p:PublishDir=%~dp0..\..\nuget\nupkgsrc
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
mstest.exe /testcontainer:%~dp0_build\Release.v4.5.2\test\CmdLineLib.Tests\CmdLineLib.Tests.dll
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
mstest.exe /testcontainer:%~dp0_build\Release.v4.6.1\test\CmdLineLib.Tests\CmdLineLib.Tests.dll
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo NuGet source packages:
dir %~dp0..\..\nuget\nupkgsrc

echo Deleting NuGet source stuff
del %~dp0..\..\nuget\nupkg\cmdlinelib\* /s /f /q

echo Packaging stuff
%~dp0..\nuget.exe init %~dp0..\..\nuget\nupkgsrc %~dp0..\..\nuget\nupkg

GOTO EOF
:ERROR
echo Failed!!!
:EOF
