"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe" /m TotalCommanderPlugins.sln /p:Configuration=Release /p:Platform=x64
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe" /m TotalCommanderPlugins.sln /p:Configuration=Release /p:Platform=x86
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe" /m TCPlayer.sln /p:Configuration="Release"

cd bin\Release
del *.pdb
del *.exp
del *.iobj
del *.lib
del *.ipdb