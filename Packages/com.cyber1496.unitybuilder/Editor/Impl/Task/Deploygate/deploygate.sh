#!/bin/zsh
URL="https://deploygate.com/api/users/$1/apps"
AUTH=$2
FILE=$3
USE_GIT=$4

if [ $USE_GIT = "True" ] ; then
	GIT_BRANCH=`git symbolic-ref --short HEAD`
	GIT_HASH=`git rev-parse HEAD`
	git log -1 --pretty=format:"[%ad] %an : %s" > tmp.txt
	MESSAGE=`cat tmp.txt`
	rm tmp.txt
	curl \
		-H "Authorization: $AUTH" \
		-F "file=@$FILE" \
		-F "message=$MESSAGE" \
		-F "distribution_name=$GIT_BRANCH" \
		-F "release_note=$MESSAGE" \
		$URL
else
	MESSAGE="no message."
	curl \
		-H "Authorization: $AUTH" \
		-F "file=@$FILE" \
		-F "message=$MESSAGE" \
		$URL
fi
