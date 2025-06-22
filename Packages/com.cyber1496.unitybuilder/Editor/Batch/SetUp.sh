#!/bin/sh

# 環境判定
if [ "$(uname)" = 'Darwin' ]; then
  ENVIRONMENT='Mac'
elif [ -d "/mnt/c" ]; then
  ENVIRONMENT='WSL'
else
  ENVIRONMENT='Windows_NT'
fi

# ProjectSettings/ProjectVersion.txt からバージョンを抽出
# OS毎に異なるUnity実行ファイルのパスを取得 todo:インストール先の取得 UnityHubの設定を参照すれば取れそう
if [ "$ENVIRONMENT" = 'Mac' ]; then
    # need $brew install gnu-sed
    UNITY_VERSION=$(grep -E 'm_EditorVersion:' ProjectSettings/ProjectVersion.txt | sed -E 's/m_EditorVersion: (.*)$/\1/' | tr -d '\r\n')
    UNITY_PATH="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
elif [ "$ENVIRONMENT" = 'WSL' ]; then
    UNITY_VERSION=$(grep -E 'm_EditorVersion:' ProjectSettings/ProjectVersion.txt | sed -r 's/m_EditorVersion: (.*)$/\1/' | tr -d '\r\n')
    # WSL environment - convert Windows path to WSL path
    UNITY_PATH="/mnt/f/Program Files/Unity/${UNITY_VERSION}/Editor/Unity.exe"
elif [ "$ENVIRONMENT" = 'Windows_NT' ]; then
    UNITY_VERSION=$(grep -E 'm_EditorVersion:' ProjectSettings/ProjectVersion.txt | sed -r 's/m_EditorVersion: (.*)$/\1/' | tr -d '\r\n')
    # Native Windows environment
    UNITY_PATH="F:/Program Files/Unity/${UNITY_VERSION}/Editor/Unity.exe"
else
    echo "Not supported environment."
    exit 1
fi

# Debug: Print variables
echo "ENVIRONMENT: $ENVIRONMENT"
echo "UNITY_VERSION: $UNITY_VERSION"
echo "UNITY_PATH: $UNITY_PATH"
