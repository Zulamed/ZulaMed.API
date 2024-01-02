#!/bin/bash


# get env file from -f
while getopts f: option
do
  case "${option}"
  in
  f) ENV_FILE=${OPTARG};;
  *) echo "usage: $0 [-f]" >&2
     exit 1 ;;
  esac
done

# if no env file is provided, exit
if [ -z "$ENV_FILE" ]
then
  echo "usage: $0 [-f]" >&2
  exit 1
fi

# Check if env file exists
if [ ! -f "$ENV_FILE" ]
then
  echo "File $ENV_FILE not found"
  exit 1
fi


build_arguments=""

while read -r line; do
  if [[ $line == *"="* ]]; then
    build_arguments="$build_arguments --build-arg $line"
  fi
done < "$ENV_FILE"

if [ -z "$build_arguments" ]
then
  echo "No build arguments found in $ENV_FILE"
  exit 1
fi

echo "Building with arguments: $build_arguments"

echo "docker command: docker build $build_arguments -t zulamed-backend-api ."

docker build $build_arguments -t zulamed-backend-api .








