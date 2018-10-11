@echo off
cls

cd OscarWeb
dotnet publish -c release

cd bin\release\netcoreapp2.1\publish
DEL *.pdb /Q
