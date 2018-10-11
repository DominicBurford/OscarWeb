@echo off
cls

rem Windows batch file that is invoked as part of the build process 
rem and performs code coverage 
rem Dominic Burford 23/03/2018

ECHO Invoking dotnet test from build script to perform code coverage analysis
rem dotnet restore
dotnet test "E:\TfsData\Build\_work\4\s\OscarWeb.Tests\OscarWeb.Tests.csproj" --no-build --no-restore /p:CollectCoverage=true
