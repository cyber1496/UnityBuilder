chcp 65001
@echo off
set SCRIPT_DIR=%~dp0
set GRADLE_ROOT=%~1
set OUTPUT_FILE=%~2
set FILE_EXT=%~3
set BUILD_VARIANT=%~4
set USE_APP_BUNDLE=%~5

cd %GRADLE_ROOT%
xcopy /Q /Y /I /E %SCRIPT_DIR%\gradle %GRADLE_ROOT%\gradle > nul
copy /Y %SCRIPT_DIR%\gradle.properties %GRADLE_ROOT%\gradle.properties > nul
copy /Y %SCRIPT_DIR%\gradlew %GRADLE_ROOT%\gradlew > nul
copy /Y %SCRIPT_DIR%\gradlew.bat %GRADLE_ROOT%\gradlew.bat > nul
if %USE_APP_BUNDLE% == True (
	start /wait /b cmd /c gradlew bundle
) else (
	start /wait /b cmd /c gradlew assemble
)
if %ERRORLEVEL% == 0 (
	copy /Y %GRADLE_ROOT%\%OUTPUT_FILE%%FILE_EXT% %GRADLE_ROOT%%FILE_EXT% > nul
) else (
	exit /B 1
)
