echo off
cls

rem Dominic Burford 27.06.2018
rem Create the zip file deployment file needed to deploy the application on Azure

CD OscarWeb

"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" /p:configuration="release";platform="any cpu";WebPublishMethod=Package;PackageFileName="E:\TfsData\Build\_work\4\s\OscarWeb\package.zip";DesktopBuildPackageLocation="E:\TfsData\Build\_work\4\s\OscarWeb\package.zip";PackageAsSingleFile=true;PackageLocation="E:\TfsData\Build\_work\4\s\OscarWeb\package.zip";DeployOnBuild=true;DeployTarget=Package

rem remove the temp deployment creation files
DEL OscarWeb.deploy.cmd
DEL OscarWeb.deploy-readme.txt
DEL OscarWeb.Parameters.xml
DEL OscarWeb.SetParameters.xml
DEL OscarWeb.SourceManifest.xml

rem move the deployment file into the obj folder
COPY package.zip "obj\any cpu\release\netcoreapp2.1"
DEL package.zip



