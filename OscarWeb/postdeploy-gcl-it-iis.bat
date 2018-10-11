@echo off
cls

rem Dominic Burford 06.07.2018
rem post deployment script needed for GCL-IT-IIS
rem Need to force the server to run using DEVELOPMENT environment settings

DEL \\gcl-it-iis\OscarWeb_Test\appsettings.json
DEL \\gcl-it-iis\OscarWeb_Test\appsettings.Development.json
DEL \\gcl-it-iis\OscarWeb_Test\appsettings.Staging.json
DEL \\gcl-it-iis\OscarWeb_Test\appsettings.Production.json
COPY \\gcl-it-iis\OscarWeb_Test\appsettings.gcl-it-iis.json \\gcl-it-iis\OscarWeb_Test\appsettings.json
DEL \\gcl-it-iis\OscarWeb_Test\appsettings.gcl-it-iis.json

rem we don't want to deploy this to any of the other endpoints so delete it from the builds folder
DEL "\\ifm-it-dev\Builds\TFS2015\Build Oscar Web\Latest\oscarwebsharelatest\Appsettings.gcl-it-iis.json"