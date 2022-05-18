#!/bin/zsh

TOOL="$1"
BUNDLE=$2
APKS=$3
KS=$4
KS_PASS=$5
KS_KEY_ALIAS=$6
KEY_PASS=$7

java -jar -Dfile.encoding=EUC-JP $TOOL \
build-apks \
--overwrite \
--bundle=$BUNDLE \
--output=$APKS \
--ks=$KS \
--ks-pass=pass:$KS_PASS \
--ks-key-alias=$KS_KEY_ALIAS \
--key-pass=pass:$KEY_PASS
