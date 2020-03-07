#!/bin/zsh

BUNDLE=$1
APKS=$2
KS=$3
KS_PASS=$4
KS_KEY_ALIAS=$5
KEY_PASS=$6

java -jar -Dfile.encoding=EUC-JP Assets/UnityBuilder/Plugins/ExternalToolsKit/AndroidGradleProcessor/bundletool-all-0.13.0.jar \
build-apks \
--overwrite \
--bundle=$BUNDLE \
--output=$APKS \
--ks=$KS \
--ks-pass=pass:$KS_PASS \
--ks-key-alias=$KS_KEY_ALIAS \
--key-pass=pass:$KEY_PASS
