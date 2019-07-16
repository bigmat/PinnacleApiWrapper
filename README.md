# PinnacleApiWrapper
Wrapper of Pinnacle (PS3838) API in .Net Standard

## Continuous Integration/Deployment
[![Build Status](https://dev.azure.com/bigmat/PinnacleApiWrapper/_apis/build/status/PinnacleApiWrapper-CI)](https://dev.azure.com/bigmat/PinnacleApiWrapper/_build/latest?definitionId=1)

## Quality
[![Sonarcloud Status](https://sonarcloud.io/api/project_badges/measure?project=bigmat_PinnacleApiWrapper&metric=alert_status)](https://sonarcloud.io/dashboard?id=bigmat_PinnacleApiWrapper)
[![Sonarcloud Status](https://sonarcloud.io/api/project_badges/measure?project=bigmat_PinnacleApiWrapper&metric=coverage)](https://sonarcloud.io/dashboard?id=bigmat_PinnacleApiWrapper)

## Nuget
[![NuGet Badge](https://buildstats.info/nuget/PinnacleApiWrapper)](https://www.nuget.org/packages/PinnacleApiWrapper/)

## How to update
- The wrapper is base on https://github.com/ps3838api/api-spec files.<br>
- The solution contains 3 Nswag config files with direct link to the api description.<br>
- Run Nswag for the project(s) you want to update, it will automatically update the corresponding .cs file(s).<br><br>

:exclamation:
Be careful, on the BetsClient, some data annotations on some properties has been modified manually because of mistake on ps3838 side (at the time this update was done).