#!/bin/bash
SCRIPT_DIR=$(cd $(dirname $0); pwd)
cd $1
cp -r $SCRIPT_DIR/gradle $1/gradle
cp $SCRIPT_DIR/gradle.properties $1/gradle.properties
cp $SCRIPT_DIR/gradlew $1/gradlew
cp $SCRIPT_DIR/gradlew.bat $1/gradlew.bat
chmod 755 $1/gradlew
./gradlew assemble
cp $1/launcher/build/outputs/apk/$2 $1$3
