#!/bin/bash

# xtool-build.sh - WSL環境でのiOSビルドスクリプト
# 使用方法: ./xtool-build.sh <output_path> <configuration>

set -e

# WSL環境の検出
echo "=== Environment Detection ==="
echo "OS: $(uname -s)"
echo "Kernel: $(uname -r)"
echo "Architecture: $(uname -m)"

# WSL環境かどうかの判定
if [[ "$(uname -s)" == "Linux" ]]; then
    # Linux環境の場合、WSLかどうかを判定
    if grep -q Microsoft /proc/version 2>/dev/null || grep -q WSL /proc/version 2>/dev/null; then
        echo "✓ WSL environment detected"
        ENVIRONMENT="WSL"
    else
        echo "✓ Linux environment detected (not WSL)"
        ENVIRONMENT="Linux"
    fi
else
    echo "✗ Not a Linux environment"
    echo "This script is designed to run on WSL (Windows Subsystem for Linux)"
    echo "Current environment: $(uname -s)"
    exit 1
fi

echo "Environment: $ENVIRONMENT"

# 引数の取得
OUTPUT_PATH="$1"
CONFIGURATION="$2"

# 引数チェック
if [ -z "$OUTPUT_PATH" ]; then
    echo "Error: Output path is required"
    echo "Usage: $0 <output_path> <configuration>"
    exit 1
fi

if [ -z "$CONFIGURATION" ]; then
    echo "Error: Configuration is required"
    echo "Usage: $0 <output_path> <configuration>"
    exit 1
fi

# ログ出力
echo "=== WSL iOS Build Started ==="
echo "Output Path: $OUTPUT_PATH"
echo "Configuration: $CONFIGURATION"
echo "Current Directory: $(pwd)"
echo "Date: $(date)"

# Xtoolの存在確認とセットアップ
XTOOL_PATH="./xtool"
if [ ! -f "$XTOOL_PATH" ]; then
    echo "Xtool not found at $XTOOL_PATH"
    echo "Attempting to install xtool..."
    
    # xtool-setup.shのパスを取得
    SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    SETUP_SCRIPT="$SCRIPT_DIR/xtool-setup.sh"
    
    if [ ! -f "$SETUP_SCRIPT" ]; then
        echo "Error: xtool-setup.sh not found at $SETUP_SCRIPT"
        exit 1
    fi
    
    echo "Running xtool setup script: $SETUP_SCRIPT"
    if bash "$SETUP_SCRIPT"; then
        echo "✓ xtool setup completed successfully"
    else
        echo "Error: xtool setup failed"
        exit 1
    fi
    
    # セットアップ後の再確認
    if [ ! -f "$XTOOL_PATH" ]; then
        echo "Error: xtool still not found after setup"
        exit 1
    fi
fi

echo "Xtool found at: $XTOOL_PATH"

# 出力ディレクトリの存在確認
if [ ! -d "$OUTPUT_PATH" ]; then
    echo "Error: Output directory does not exist: $OUTPUT_PATH"
    exit 1
fi

echo "Output directory exists: $OUTPUT_PATH"

# Xcodeプロジェクトファイルの確認
XCODEPROJ_PATH=$(find "$OUTPUT_PATH" -name "*.xcodeproj" -type d | head -1)
if [ -z "$XCODEPROJ_PATH" ]; then
    echo "Error: No .xcodeproj file found in $OUTPUT_PATH"
    exit 1
fi

echo "Xcode project found: $XCODEPROJ_PATH"

# プロジェクト名の取得
PROJECT_NAME=$(basename "$XCODEPROJ_PATH" .xcodeproj)
echo "Project name: $PROJECT_NAME"

# 開発者情報の取得（環境変数またはデフォルト値）
TEAM_ID="${TEAM_ID:-$(grep -o 'DEVELOPMENT_TEAM = [^;]*' "$OUTPUT_PATH/project.pbxproj" 2>/dev/null | cut -d' ' -f3 || echo '')}"
BUNDLE_ID="${BUNDLE_ID:-$(grep -o 'PRODUCT_BUNDLE_IDENTIFIER = [^;]*' "$OUTPUT_PATH/project.pbxproj" 2>/dev/null | cut -d' ' -f3 || echo '')}"

# デフォルト値の設定
if [ -z "$TEAM_ID" ]; then
    TEAM_ID="YOUR_TEAM_ID"
    echo "Warning: Team ID not found, using default: $TEAM_ID"
fi

if [ -z "$BUNDLE_ID" ]; then
    BUNDLE_ID="com.example.app"
    echo "Warning: Bundle ID not found, using default: $BUNDLE_ID"
fi

echo "Team ID: $TEAM_ID"
echo "Bundle ID: $BUNDLE_ID"

# ステップ1: ビルド実行
echo "=== Step 1: Building with xtool ==="
#echo "Command: $XTOOL_PATH build \"$OUTPUT_PATH\" --configuration \"$CONFIGURATION\""
echo "Command: $XTOOL_PATH build"

#if "$XTOOL_PATH" build "$OUTPUT_PATH" --configuration "$CONFIGURATION"; then
if "$XTOOL_PATH" build; then
    echo "✓ Build completed successfully"
else
    echo "Error: Build failed"
    exit 1
fi

# ビルド成果物の確認
echo "Checking build artifacts..."
BUILD_ARTIFACT_PATH="$OUTPUT_PATH/build"
if [ -d "$BUILD_ARTIFACT_PATH" ]; then
    echo "Build artifacts found in: $BUILD_ARTIFACT_PATH"
    ls -la "$BUILD_ARTIFACT_PATH"
else
    echo "Warning: Build artifacts directory not found"
fi

# ステップ2: サイン実行
echo "=== Step 2: Signing with xtool ==="
echo "Command: $XTOOL_PATH sign --team-id \"$TEAM_ID\" --bundle-id \"$BUNDLE_ID\" \"$OUTPUT_PATH\""

if "$XTOOL_PATH" sign --team-id "$TEAM_ID" --bundle-id "$BUNDLE_ID" "$OUTPUT_PATH"; then
    echo "✓ Signing completed successfully"
else
    echo "Error: Signing failed"
    exit 1
fi

# 最終成果物の確認
echo "=== Final Build Artifacts ==="
echo "Checking final signed artifacts..."
if [ -d "$BUILD_ARTIFACT_PATH" ]; then
    echo "Final artifacts in: $BUILD_ARTIFACT_PATH"
    ls -la "$BUILD_ARTIFACT_PATH"
    
    # .ipaファイルの検索
    IPA_FILES=$(find "$BUILD_ARTIFACT_PATH" -name "*.ipa" 2>/dev/null)
    if [ -n "$IPA_FILES" ]; then
        echo "✓ IPA files found:"
        echo "$IPA_FILES"
    else
        echo "Warning: No IPA files found"
    fi
else
    echo "Warning: Build artifacts directory not found"
fi

echo "=== WSL iOS Build Completed Successfully ==="
echo "Build and sign completed at: $(date)"
exit 0 