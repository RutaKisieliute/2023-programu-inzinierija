#!/bin/bash -x
EXEC_DIR=$(dirname $(realpath $0))
cd $EXEC_DIR

dotnet test --collect:"XPlat Code Coverage" --settings:./TestSettings.runsettings
