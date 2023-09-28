#!/bin/bash -x
export ASPNETCORE_ENVIRONMENT='Development'

EXEC_DIR=$(dirname $(realpath $0))
cd $EXEC_DIR

dotnet run --project ./AALKisMVCUI/AALKisMVCUI.csproj --configuration Debug --launch-profile 'https'

dotnet clean ./AALKisMVCUI/AALKisMVCUI.csproj
