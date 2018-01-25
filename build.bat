@ECHO OFF
SETLOCAL

start dotnet restore KBL.Framework && dotnet build KBL.Framework

xcopy ".\KBL.Framework\KBL.Framework.Base\bin\Release\netcoreapp2.0\*.dll" ".\nuget\lib\netcoreapp2.0\" /Y
