chcp 65001
@echo off
set SCRIPT_DIR=%~dp0
cd %~1
xcopy /Q /Y /I /E %SCRIPT_DIR%\gradle %~1\gradle > nul
copy /Y %SCRIPT_DIR%\gradle.properties %~1\gradle.properties > nul
copy /Y %SCRIPT_DIR%\gradlew %~1\gradlew > nul
copy /Y %SCRIPT_DIR%\gradlew.bat %~1\gradlew.bat > nul
start /wait /b cmd /c gradlew assemble
if %ERRORLEVEL% == 0 (
	copy /Y %~1\launcher\build\outputs\apk\%~2 %~1%~3 > nul
) else (
	exit /B 1
)
