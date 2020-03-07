chcp 65001
@echo off

set TOOL="%~1"
set BUNDLE=%~2
set APKS=%~3
set KS=%~4
set KS_PASS=%~5
set KS_KEY_ALIAS=%~6
set KEY_PASS=%~7

java -jar -Dfile.encoding=EUC-JP %TOOL% ^
build-apks ^
--overwrite ^
--bundle=%BUNDLE% ^
--output=%APKS% ^
--ks=%KS% ^
--ks-pass=pass:%KS_PASS% ^
--ks-key-alias=%KS_KEY_ALIAS% ^
--key-pass=pass:%KEY_PASS%
