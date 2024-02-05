#!/bin/bash -x
EXEC_DIR=$(dirname $(realpath $0))
cd $EXEC_DIR

dotnet run --project ./AALKisMVCUI/AALKisMVCUI.csproj --configuration Release --launch-profile 'https'
