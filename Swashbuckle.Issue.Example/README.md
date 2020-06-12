# swashbuckle-issue-example Micro-service

## Development Requirements
.net core sdk 3.1

ASP.NET core runtime 3.1


[Download](https://www.microsoft.com/net/download/)

To determine which version of the dotnet sdk is currently installed run

```bash
dotnet --version
```

## Development Setup

### Development  Configuration

## Build and run

From a command line:
```
dotnet clean
dotnet restore
dotnet build
dotnet run --project Swashbuckle.Issue.Example.Web
```
or simply `dotnet run --project Swashbuckle.Issue.Example.Web`, `./run.sh` or `run.bat` will work

## Build and test
```
dotnet test
```

## Environment variables
### ASPNET_URLS
A semicolon delimited list of urls for which the host application will listen.

The development instance will default to `http://locahost:5000`.   The deployed version is configured for `http://*:5000` (all host names)

### ASPNETCORE_ENVIRONMENT
The running environment.

Values: `Development` (Default) | `Production`

## Generating and running the deployable version
From a command line:
```
dotnet publish --configuration (Debug|Release) Swashbuckle.Issue.Example.Web
dotnet Swashbuckle.Issue.Example.Web/bin/(Debug|Release)/netcoreapp3.1/publish/Swashbuckle.Issue.Example.Web.dll
```
The ASPNETCORE_ENVIRONMENT for the published application defaults to `Production`.
