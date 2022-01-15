#!/bin/zsh
SCRIPT_DIR=$(cd $(dirname $0); pwd)

function usage {
    cat <<EOF
$(basename ${0}) is a tool for ...

Usage:
    $(basename ${0}) [<options>]

Options:
    --platform  -p  Platform [Android | iOS]
    --config    -c  Configuration Identifier
    --scheme    -s  Scheme Identifier
    --entry     -e  Entry Method Name
    --help      -h  print this
EOF
}
while [ $# -gt 0 ];
do
    case ${1} in
        --platform|-p)
            PLATFORM=${2}
            shift
        ;;
        --config|-c)
            CONFIG=${2}
            shift
        ;;
        --scheme|-s)
            SCHEME=${2}
            shift
        ;;
        --entry|-e)
            ENTRY_METHOD=${2}
            shift
        ;;
        --help|-h)
            usage
            exit 0
        ;;
        *)
			echo "[ERROR] Invalid option '${1}'"
            usage
            exit 1
        ;;
    esac
    shift
done

TMP_LOG="Logs/${PLATFORM}/${CONFIG}_${SCHEME}.log"

. $SCRIPT_DIR/SetUp.sh

if [ $OS == 'Windows_NT' ] && [ $PLATFORM == 'iOS' ]; then
	echo "Not supported OS."
	exit 1
fi

"${UNITY_PATH}" \
	-quit \
	-batchmode \
	-projectPath "${PWD}" \
	-buildTarget `echo "${PLATFORM}" | tr '[A-Z]' '[a-z]'` \
	-executeMethod "${ENTRY_METHOD}" \
	-config "${CONFIG}" \
	-scheme "${SCHEME}" \
	-logFile "${TMP_LOG}"
EXIT_CODE=$?

echo "$(<$TMP_LOG)"
exit $EXIT_CODE
