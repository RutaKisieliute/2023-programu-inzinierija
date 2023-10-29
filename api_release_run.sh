#!/bin/bash -x
EXEC_DIR=$(dirname $(realpath $0))
cd $EXEC_DIR

dotnet run --project ./AALKisAPI/AALKisAPI.csproj --configuration Release | tee api_release.log
