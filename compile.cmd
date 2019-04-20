set vs2017="c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
set vs2019="c:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe"
set compiler=""

if exist %vs2019% (
	set compiler=%vs2019%
	goto compile
)
if exist %vs2017% (
	set compiler=%vs2017%
	goto compile
)

REM No compiler found exit...
goto exit


:compile
%compiler% /m TotalCommanderPlugins.sln /p:Configuration=Release /p:Platform=x64
%compiler% /m TotalCommanderPlugins.sln /p:Configuration=Release /p:Platform=x86
%compiler% /m TCPlayer.sln /p:Configuration="Release"

cd bin\Release
del *.pdb
del *.exp
del *.iobj
del *.lib
del *.ipdb

:exit
