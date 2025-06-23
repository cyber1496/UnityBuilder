#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../../../../../../" && pwd)"
echo "SCRIPT_DIR: $SCRIPT_DIR"
echo "PROJECT_ROOT: $PROJECT_ROOT"

XTOOL_BIN="$PROJECT_ROOT/xtool"
XTOOL_VERSION="1.13.0"
XTOOL_URL="https://github.com/xtool-org/xtool/releases/download/${XTOOL_VERSION}/xtool-x86_64.AppImage"
XTOOL_APPIMAGE_DL="$PROJECT_ROOT/xtool-x86_64.AppImage"

# 既にバイナリが存在する場合はスキップ
if [ -x "$XTOOL_BIN" ]; then
  echo "xtool binary already exists at $XTOOL_BIN. Skipping download."
  exit 0
fi

echo "Downloading xtool v$XTOOL_VERSION AppImage..."
curl -L -o "$XTOOL_APPIMAGE_DL" "$XTOOL_URL"
mv "$XTOOL_APPIMAGE_DL" "$XTOOL_BIN"
chmod +x "$XTOOL_BIN"
echo "xtool AppImage downloaded to $XTOOL_BIN."