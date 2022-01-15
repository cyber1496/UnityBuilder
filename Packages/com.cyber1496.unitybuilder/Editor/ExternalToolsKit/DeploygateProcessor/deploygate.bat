@echo off
chcp 65001
set URL="https://deploygate.com/api/users/%~1/apps"
set AUTH=%~2
set FILE=%~3
set USE_GIT=%~4

setlocal enabledelayedexpansion
if %USE_GIT% == True (
	FOR /F %%i in ('git symbolic-ref --short HEAD') do set GIT_BRANCH=%%i
	git log -1 --pretty=format:"[%%ad] %%an : %%s" >tmp.txt
	set /P MESSAGE=<tmp.txt
	curl ^
		-H "Authorization: %AUTH%" ^
		-F "file=@%FILE%" ^
		-F "message=!MESSAGE!" ^
		-F "distribution_name=!GIT_BRANCH!" ^
		-F "release_note=!MESSAGE!" ^
		%URL%
	del tmp.txt
) else (
	curl ^
		-H "Authorization: %AUTH%" ^
		-F "file=@%FILE%" ^
		-F "message=no message." ^
		%URL%
)
