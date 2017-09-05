@echo off
cd "RepositoryServer\bin\Debug"
start RepositoryServer.exe

cd "..\..\..\TestHarnessServer\bin\Debug"
start TestHarnessServer.exe

cd "..\..\..\Client\bin\Debug"
start Client.exe

cd "..\..\..\Client1\bin\Debug"
start Client1.exe

cd "..\..\..\Client2\bin\Debug"
start Client2.exe

