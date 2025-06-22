#!/bin/sh
SCRIPT_DIR=$(cd $(dirname $0); pwd)

usage() {
    cat << 'EOF'
AppBuild.sh is a tool for ...

Usage:
    AppBuild.sh [<options>]

Options:
    --platform  -p  Platform [Android | iOS]
    --config    -c  Configuration Identifier
    --scheme    -s  Scheme Identifier
    --entry     -e  Entry Method Name
    --help      -h  print this
EOF
}

while [ $# -gt 0 ]
do
    case "$1" in
        --platform|-p)
            PLATFORM="$2"
            shift
        ;;
        --config|-c)
            CONFIG="$2"
            shift
        ;;
        --scheme|-s)
            SCHEME="$2"
            shift
        ;;
        --entry|-e)
            ENTRY_METHOD="$2"
            shift
        ;;
        --help|-h)
            usage
            exit 0
        ;;
        *)
            echo "[ERROR] Invalid option '$1'"
            usage
            exit 1
        ;;
    esac
    shift
done

TMP_LOG="Logs/${PLATFORM}/${CONFIG}_${SCHEME}.log"

# Create log directory if it doesn't exist
mkdir -p "Logs/${PLATFORM}"

. "$SCRIPT_DIR/SetUp.sh"

if [ "$ENVIRONMENT" = 'Windows_NT' ] && [ "$PLATFORM" = 'iOS' ]; then
    echo "Not supported environment."
    exit 1
fi

# Get the correct project path
PROJECT_PATH=$(pwd)

# Convert WSL path to Windows path if running in WSL
if [ "$ENVIRONMENT" = 'WSL' ]; then
    # WSL environment - convert /mnt/f/Work/UnityBuilder to F:\Work\UnityBuilder
    PROJECT_PATH=$(echo "$PROJECT_PATH" | sed 's|^/mnt/f/|F:\\|' | sed 's|/|\\|g')
fi

# Debug: Print the Unity command that will be executed
echo "Executing Unity command:"
echo "${UNITY_PATH} -quit -batchmode -projectPath \"${PROJECT_PATH}\" -buildTarget \"$(echo "${PLATFORM}" | tr '[A-Z]' '[a-z]')\" -executeMethod \"${ENTRY_METHOD}\" -config \"${CONFIG}\" -scheme \"${SCHEME}\" -logFile \"${TMP_LOG}\""

"${UNITY_PATH}" \
    -quit \
    -batchmode \
    -projectPath "${PROJECT_PATH}" \
    -buildTarget "$(echo "${PLATFORM}" | tr '[A-Z]' '[a-z]')" \
    -executeMethod "${ENTRY_METHOD}" \
    -config "${CONFIG}" \
    -scheme "${SCHEME}" \
    -logFile "${TMP_LOG}"
EXIT_CODE=$?

cat "$TMP_LOG"
exit $EXIT_CODE 