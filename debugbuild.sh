#!/bin/bash
cd $(dirname $(which $0))
dotnet build AALKis.sln -c Debug
