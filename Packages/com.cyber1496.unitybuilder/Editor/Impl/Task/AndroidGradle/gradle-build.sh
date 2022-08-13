#!/bin/zsh
SCRIPT_DIR=$(cd $(dirname $0); pwd)
GRADLE_ROOT=$1
OUTPUT_FILE=$2
FILE_EXT=$3
BUILD_VARIANT=$4
USE_APP_BUNDLE=$5

cd $GRADLE_ROOT
cp -r $SCRIPT_DIR/gradle $GRADLE_ROOT/gradle/.
cp $SCRIPT_DIR/gradlew $GRADLE_ROOT/gradlew
cp $SCRIPT_DIR/gradlew.bat $GRADLE_ROOT/gradlew.bat
chmod 755 $GRADLE_ROOT/gradlew

if [ $USE_APP_BUNDLE = "True" ] ; then
	./gradlew bundle
else
	./gradlew assemble;
fi
GRADLE_EXIT=$?

if [ $GRADLE_EXIT -eq 0 ] ; then
	cp $GRADLE_ROOT/$OUTPUT_FILE$FILE_EXT $GRADLE_ROOT$FILE_EXT
else
	exit 1
fi
