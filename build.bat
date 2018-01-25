@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=.\bin\nuget.exe

start dotnet restore KBL.Framework && dotnet build KBL.Framework

xcopy ".\KBL.Framework\KBL.Framework.Base\bin\Release\netcoreapp2.0\*.dll" ".\nuget\lib\netcoreapp2.0\" /Y

FOR /r %%f IN (*.nuspec) DO (
  echo "packing..."
  %NUGET% pack %%f -Version %VERSION%
)