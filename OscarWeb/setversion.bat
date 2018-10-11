@echo off
cls

ECHO Setting version number to %1

cd OscarWeb
dotnet restore
dotnet setversion %1

