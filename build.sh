# msbuild=`cygpath -u "$VS150COMNTOOLS"`
msbuild=/cygdrive/c/Program\ Files\ \(x86\)/MSBuild/15.0/Bin/MSBuild.exe
# echo $msbuild
# ls "$msbuild"
# "$msbuild/VsMSBuildCmd.bat"
"$msbuild" CmdLineLib\\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.5.2 || exit 1
"$msbuild" CmdLineLib\\CmdLineLib.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.6.1 /p:CreatePackage=true /p:PublishDir=..\\..\\..\\nupkgsrc || exit 1
# ../nuget.exe pack CmdLineLib\\CmdLineLib.nuspec -OutputDirectory . -Version 0.1.3-beta || exit 1

rm -rf ../../nupkg/cmdlinelib/* || exit 1
../nuget.exe init ..\\..\\nupkgsrc ..\\..\\nupkg
