@echo off
rem set solution folder as %SOLUTION%
set SOLUTION=%3
cd build
rem set build tools folder as %BF%
set BF=%SOLUTION%Build
echo -----------------------------------------------------------------------------
echo Build tools folder:
echo %BF%
echo -----------------------------------------------------------------------------
echo Solution folder:
echo %SOLUTION%
echo -----------------------------------------------------------------------------

IF "%4"=="" goto notincrement
:increment
rem increment version
echo Running command:
echo %BF%\AppLib.VersionIncrementer.exe /increment "%BF%\template.xml" "%BF%\%1" "%SOLUTION%\%2\Properties\AssemblyInfo.cs"
%BF%\AppLib.VersionIncrementer.exe /increment "%BF%\template.xml" "%BF%\%1" "%SOLUTION%\%2\Properties\AssemblyInfo.cs"
echo -----------------------------------------------------------------------------
goto exit


:notincrement
echo Running command:
echo %BF%\AppLib.VersionIncrementer.exe "%BF%\template.xml" "%BF%\%1" "%SOLUTION%\%2\Properties\AssemblyInfo.cs"
%BF%\AppLib.VersionIncrementer.exe "%BF%\template.xml" "%BF%\%1" "%SOLUTION%\%2\Properties\AssemblyInfo.cs"
echo -----------------------------------------------------------------------------

:exit
rem exit point