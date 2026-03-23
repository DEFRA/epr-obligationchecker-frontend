# EPR Obligation Checker Frontend - Outside-In Integration Tests

This test project demonstrates an outside-in testing approach for the EPR Obligation Checker Frontend microservice.

By testing only the inputs and outputs of the microservice we can isolate the tests from internal implementation changes and prove behaviour of the service as a whole while controlling behaviour of all external dependencies.

## Structure

- WebApplicationFactory builds and runs real application for testing
- Stub services replace Azure Blob Storage dependencies, controllable per-test
- Return HTML asserted on via "page model" classes allowing us to decouple tests from changes in HTML structures

## Key differences from epr-regulator-service integration tests

The regulator service depends on backing microservices (HTTP APIs) and uses WireMock to fake them. This service depends on Azure Blob Storage, so instead of WireMock we replace `IBlobStorageService` and `ILargeProducerRegisterService` at the DI boundary with controllable stub implementations. The principle is the same: fake external dependencies at their boundary while exercising the full HTTP pipeline for real.

No authentication stubbing is needed as all endpoints are public.

## Debugging

To see the logs from the captive web host enable verbose logging as follows:

```sh
dotnet test --filter LargeProducerRegisterTests --logger "console;verbosity=detailed"
```
