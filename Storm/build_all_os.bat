@echo off
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./build
ren .\build\Storm.exe Storm-win-x64.exe

dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./build
ren .\build\Storm Storm-linux-x64

dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./build
ren .\build\Storm Storm-osx-x64

dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./build
ren .\build\Storm.exe Storm-win-x86.exe

dotnet publish -c Release -r linux-x86 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./build
ren .\build\Storm Storm-linux-x86

dotnet publish -c Release -r osx-x86 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./build
ren .\build\Storm Storm-osx-x86

dotnet publish -c Release -r win-x32 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./build
ren .\build\Storm.exe Storm-win-x32.exe

del Storm.pdb