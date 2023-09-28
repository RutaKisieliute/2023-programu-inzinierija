#!/bin/bash -x
export ASPNETCORE_ENVIRONMENT='Development'

EXEC_DIR=$(dirname $(realpath $0))
cd $EXEC_DIR

dotnet run --project ./AALKisAPI/AALKisAPI.csproj --configuration Debug

dotnet clean ./AALKisAPI/AALKisAPI.csproj
