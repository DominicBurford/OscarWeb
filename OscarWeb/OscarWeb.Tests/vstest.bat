@echo off
cls

rem Author Dominic Burford
rem Date 21.03.2018

rem Execute ASP.NET Core 2.0 unit tests from the command-line using the dotnet CLI.
rem The TFS2015 VSTest task doesn't create the test results .trx files.

ECHO Invoking dotnet test from build script to perform execute unit tests
dotnet test "E:\TfsData\Build\_work\4\s\OscarWeb.Tests\OscarWeb.Tests.csproj" --list-tests --verbosity minimal
dotnet test "E:\TfsData\Build\_work\4\s\OscarWeb.Tests\OscarWeb.Tests.csproj" --no-build --no-restore --logger "trx;LogFileName=TestResults.trx" --verbosity minimal
