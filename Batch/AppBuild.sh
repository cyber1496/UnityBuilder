#!/bin/zsh
SCRIPT_DIR=$(cd $(dirname $0); pwd)
PLATFORM=$1
CONFIG=$2
SCHEME=$3
TMP_LOG="Logs/$PLATFORM/logFile.log"

. $SCRIPT_DIR/SetUp.sh

if [ $OS == 'Windows_NT' ] && [ $PLATFORM == 'iOS' ]; then
	echo "Not supported OS."
	exit 1
fi

"${UNITY_PATH}" \
	-quit \
	-batchmode \
	-projectPath $PWD \
	-buildTarget `echo $PLATFORM | tr '[A-Z]' '[a-z]'` \
	-executeMethod Example.DoIt \
	-config $CONFIG \
	-scheme $SCHEME \
	-logFile $TMP_LOG
EXIT_CODE=$?

echo "$(<$TMP_LOG)"
exit $EXIT_CODE
