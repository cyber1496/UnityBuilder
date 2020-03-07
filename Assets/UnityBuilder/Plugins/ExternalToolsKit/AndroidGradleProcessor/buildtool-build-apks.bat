chcp 65001
@echo off

set BUNDLE=%~1
set APKS=%~2
set KS=%~3
set KS_PASS=%~4
set KS_KEY_ALIAS=%~5
set KEY_PASS=%~6

java -jar -Dfile.encoding=EUC-JP Assets\UnityBuilder\Plugins\ExternalToolsKit\AndroidGradleProcessor\bundletool-all-0.13.0.jar ^
build-apks ^
--overwrite ^
--bundle=%BUNDLE% ^
--output=%APKS% ^
--ks=%KS% ^
--ks-pass=pass:%KS_PASS% ^
--ks-key-alias=%KS_KEY_ALIAS% ^
--key-pass=pass:%KEY_PASS%
