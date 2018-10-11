@echo off
cls

rem Copy the code coverage results over to the TestResults folder so they can be 
rem viewed using a browser.
rem Dominic Burford 23/03/20168

ECHO Copying file to wwwroot folder for build %1
COPY "E:\TfsData\Build\_work\4\s\OscarWeb.Tests\coverage.json" \\ifm-it-dev\wwwroot\TestResults_OscarWeb\coverage%1.json /Y
COPY "E:\TfsData\Build\_work\4\s\OscarWeb.Tests\coverage.json" \\ifm-it-dev\wwwroot\TestResults_OscarWeb\coverage.json /Y
