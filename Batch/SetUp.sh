#!/bin/zsh

# Windows bash場合 OS = Windows_NT が格納されている
if [ "$(uname)" == 'Darwin' ]; then
  OS='Mac'
fi

# ProjectSettings/ProjectVersion.txt からバージョンを抽出
# OS毎に異なるUnity実行ファイルのパスを取得 todo:インストール先の取得 UnityHubの設定を参照すれば取れそう
if [ $OS == 'Mac' ]; then
	# need $brew install gnu-sed
	UNITY_VERSION=`grep -E 'm_EditorVersion:' ProjectSettings/ProjectVersion.txt | sed -E 's/m_EditorVersion: (.*)$/\1/'`
	UNITY_PATH="/Applications/Unity/Hub/Editor/$UNITY_VERSION/Unity.app/Contents/MacOS/Unity"
elif [ $OS == 'Windows_NT' ]; then
	UNITY_VERSION=`grep -E 'm_EditorVersion:' ProjectSettings/ProjectVersion.txt | sed -r 's/m_EditorVersion: (.*)$/\1/'`
	UNITY_PATH="/f/Program Files/Unity/$UNITY_VERSION/Editor/Unity.exe"
else
	echo "Not supported OS."
	exit 1
fi
