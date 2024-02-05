#!/bin/bash -x
EXEC_DIR=$(dirname $(realpath $0))
cd $EXEC_DIR

cd UnitTests/
dotnet test --collect:"XPlat Code Coverage" --settings:../TestSettings.runsettings

cd ../IntegrationTests/
dotnet test --collect:"XPlat Code Coverage" --settings:../TestSettings.runsettings
