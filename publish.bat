@echo off
dotnet pack NetSerializer\NetSerializer.csproj --configuration Release --output ..\Assemblies\PrivateNuGet
pause
