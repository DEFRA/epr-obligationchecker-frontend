#!/usr/bin/env bash

dotnet test src/FrontendObligationChecker.IntegrationTests/FrontendObligationChecker.IntegrationTests.csproj --logger "trx;logfilename=testResults.trx"