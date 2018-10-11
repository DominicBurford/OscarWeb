@echo off
cls

rem delete all CSS  files except the bootstrap minified CSS file and the combined site.min.css file
rem that is created by the build process via bundleconfig.json settings


rem CSS files
DEL OscarWeb\wwwroot\css\bootstrap.css /Q
DEL OscarWeb\wwwroot\css\documentmanager.css /Q
DEL OscarWeb\wwwroot\css\documentmanager.min.css /Q
DEL OscarWeb\wwwroot\css\layout.css /Q
DEL OscarWeb\wwwroot\css\layout.min.css /Q
DEL OscarWeb\wwwroot\css\oscar_icons.css /Q
DEL OscarWeb\wwwroot\css\oscar_icons.min.css /Q
DEL OscarWeb\wwwroot\css\static.css /Q
DEL OscarWeb\wwwroot\css\static.min.css /Q
DEL OscarWeb\wwwroot\css\iconset.css /Q
DEL OscarWeb\wwwroot\css\style.css /Q
DEL OscarWeb\wwwroot\css\all.css /Q
DEL OscarWeb\wwwroot\css\site.css /Q

rem delete all JS  files except the bootstrap and jquery minified JS files and the combined site.min.css file
rem that is created by the build process via bundleconfig.json settings

rem JS files
DEL OscarWeb\wwwroot\js\bootstrap.js /Q
DEL OscarWeb\wwwroot\js\site.js /Q
DEL OscarWeb\wwwroot\js\jquery-3.3.1.js /Q
DEL OscarWeb\wwwroot\js\scripts.js /Q