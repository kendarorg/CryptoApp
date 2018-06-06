@ECHO OFF

set CUDIR=%CD%

echo Rebuilding
time /T

call npm run build
mkdir ..\CryptoApp\static > nul 2>nul

echo CleanUp destination dir

del /Q ..\CryptoApp\static\*.*
cd /d ..\CryptoApp\static
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)
cd %CUDIR%
    
echo Copying dist

copy /Y "%CD%\dist\*.*" ..\CryptoApp > nul 2>nul
xcopy /Q /E /V "%CD%\dist\static" ..\CryptoApp\static > nul 2>nul

echo Dist completed