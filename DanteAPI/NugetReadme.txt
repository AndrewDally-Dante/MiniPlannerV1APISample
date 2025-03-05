Update version in ths project file
Open CMD/Terminal in this projects folder
dotnet pack -c Release
dotnet nuget push bin/Release/Dante.Api.1.0.*.nupkg --api-key *** --source https://api.nuget.org/v3/index.json
