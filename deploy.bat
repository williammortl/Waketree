REM compile and deploy Neptune programs
call compileanddeploy.bat

REM Delete and recreate directories
rmdir t:\Waketree /S /Q
mkdir t:\Waketree
mkdir t:\Waketree\Service
mkdir t:\Waketree\Supervisor
rmdir m:\Waketree /S /Q
mkdir m:\Waketree
mkdir m:\Waketree\Service
mkdir m:\Waketree\Supervisor

REM deploy binaries to Cygnus
copy /Y e:\Code\Waketree\Waketree.Supervisor\bin\Debug\net7.0\*.* m:\Waketree\Supervisor
copy /Y e:\Code\Waketree\Waketree.Service\bin\Debug\net7.0\*.* m:\Waketree\Service
copy /Y E:\Code\Waketree\Waketree.Runtimes.Neptune\bin\Debug\net7.0\*.* m:\Waketree\Service

REM deploy binaries to Taylor
copy /Y e:\Code\Waketree\Waketree.Supervisor\bin\Debug\net7.0\*.* t:\Waketree\Supervisor
copy /Y e:\Code\Waketree\Waketree.Service\bin\Debug\net7.0\*.* t:\Waketree\Service
copy /Y E:\Code\Waketree\Waketree.Runtimes.Neptune\bin\Debug\net7.0\*.* t:\Waketree\Service