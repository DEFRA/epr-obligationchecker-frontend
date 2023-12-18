#!/usr/bin/env bash

dotnet test src/FrontendObligationChecker.UnitTests/FrontendObligationChecker.UnitTests.csproj --logger "trx;logfilename=testResults.trx"