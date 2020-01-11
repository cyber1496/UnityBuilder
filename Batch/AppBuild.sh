#!/bin/zsh

VERSION=2019.3.0f3
UNITY=/Applications/Unity/Hub/Editor/$VERSION/Unity.app/Contents/MacOS/Unity

PLATFORM=Android
$UNITY \
	-quit \
	-batchmode \
	-projectPath $PWD \
	-buildTarget `echo $PLATFORM | tr '[A-Z]' '[a-z]'` \
	-executeMethod Example.DoIt \
	-logFile "Logs/$PLATFORM/logFile.log"
echo "exitcode:"$?

PLATFORM="iOS"
$UNITY \
	-quit \
	-batchmode \
	-projectPath $PWD \
	-buildTarget `echo $PLATFORM | tr '[A-Z]' '[a-z]'` \
	-executeMethod Example.DoIt \
	-logFile "Logs/$PLATFORM/logFile.log"
echo "exitcode:"$?
