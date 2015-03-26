@echo off

rem Parameters
set SOLUTION=Serilog.Sinks.Udp.sln
set PROJECT=src\Serilog.Sinks.Udp\Serilog.Sinks.Udp.csproj
set MSBUILD="%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe"
set NUGET=.nuget\NuGet.exe

rem Build solution
%MSBUILD% %SOLUTION% /t:Rebuild /p:Configuration=Release /m

REM Create nuget package
%NUGET% pack -Prop Configuration=Release %PROJECT%

pause