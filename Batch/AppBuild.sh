#!/bin/sh
VERSION=2019.3.0f3
UNITY=/Applications/Unity/Hub/Editor/$VERSION/Unity.app/Contents/MacOS/Unity

LOG="Logs/logFile.log"
$UNITY \
	-quit \
	-batchmode \
	-projectPath $PWD \
	-buildTarget android \
	-executeMethod Example.DoIt \
	-logFile $LOG
echo "exitcode:"$?
