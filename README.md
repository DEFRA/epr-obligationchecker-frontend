# Introduction 

This repository hosts the ObligationChecker service that helps producers determine their obligations towards reporting packaging.
This service is available to the public at: https://www.gov.uk/guidance/check-if-you-need-to-report-packaging-data

## Notes

ObligationChecker becomes a public portal that will serve all EPR-related pages that can be accessed by the general public.
The register of large producers is the first of those pages.

Original function of ObligationChecker has been disabled by feature toggle called `FeatureManagement:ObligationCheckerEnabled`.
This functionality will be restored in the future roadmap.

# Getting Started

## Dependencies

You can either run the application locally or through docker.

### Prerequisites for Docker

You will need docker desktop, please download that [here](https://www.docker.com/products/docker-desktop/)

### Prerequisites for Local

To run the application locally, you will need:
Dotnet 8.0 SDK which you can install from [Microsoft Dotnet Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
Npm which you can install through [Node.Js Downloads](https://nodejs.org/en/download)

## Running the Application Through Docker

This is as simple as:
`docker-compose build`
`docker-compose up`

You can then navigate to the project on http://localhost

Most IDEs will support attaching a debugger through the IDE Build Configuration options against the docker container.
Please refer to your IDE documentation on how to do this.

## Running the Application Locally

To run the application,

First, open src/FrontendObligationChecker/appsettings.json and search for
`"UseLocalSession": false,`
Change that into
`"UseLocalSession": true,`

Now navigate to the /src folder and run:
`dotnet run --project FrontendObligationChecker`

You should then be able to navigate to the project on https://localhost:7022

# Build and Test

To run unit tests, navigate to the /src folder and run:

`dotnet test`

# Deploy

To deploy this code run the following pipelines in order within the ObligationChecker folder under Azure DevOps Pipelines:
- frontend-obligationchecker-microservice
- deploy-obligation-checker-code

Infrastructure deployment should be requested through CCoE (see confluence).

# Contributing to this project

Please read the [contribution guidelines](CONTRIBUTING.md) before submitting a pull request.

# Licence

[Licence information](LICENCE.md).