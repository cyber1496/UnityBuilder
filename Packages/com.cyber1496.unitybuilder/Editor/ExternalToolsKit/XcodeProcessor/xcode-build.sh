#!/bin/zsh
SCRIPT_DIR=$(cd $(dirname $0); pwd)
OUTPUT_DIR=$1
CONFIG=$2
EXPORT_OPTION_PLIST_PATH=$3

ARCHIVE_PATH=$OUTPUT_DIR.xcarchive
OUTPUT_PATH=$OUTPUT_DIR.export
SCHEME=Unity-iPhone

xcodebuild -version

xcodebuild \
    -project $OUTPUT_DIR/$SCHEME.xcodeproj \
    -scheme $SCHEME \
    -configuration $CONFIG \
    archive -archivePath $ARCHIVE_PATH

xcodebuild -exportArchive \
    -archivePath $ARCHIVE_PATH \
    -exportPath $OUTPUT_PATH \
    -exportOptionsPlist $EXPORT_OPTION_PLIST_PATH

for IPA_FILE in `find $OUTPUT_PATH -name "*.ipa"`; do
    cp $IPA_FILE $OUTPUT_DIR.ipa
    break
done
