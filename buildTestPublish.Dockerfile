#This target builds the API server and runs unit tests
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS base
LABEL maintainer="maintainer@acme.com"
LABEL type="build"
#The environment value is case sensitive
ENV ASPNETCORE_ENVIRONMENT=Production
RUN apk add --update --no-cache git bash curl zip
RUN export PATH="$PATH:/root/.dotnet/tools"

#Copy files - Copy in just the solution and csproj files which change infrequently
# so that we can do dotnet restore and have that layer cached.
FROM base as copy-files
LABEL type="build"
ARG SOLUTION_NAME="Swashbuckle.Issue.Example"
WORKDIR /app_build
WORKDIR /app_build/$SOLUTION_NAME
COPY ./$SOLUTION_NAME/NuGet.config.tmpl ./NuGet.config
COPY ./$SOLUTION_NAME/$SOLUTION_NAME.sln .

COPY ./$SOLUTION_NAME/$SOLUTION_NAME.HttpClient/$SOLUTION_NAME.HttpClient.csproj ./$SOLUTION_NAME.HttpClient/$SOLUTION_NAME.HttpClient.csproj
COPY ./$SOLUTION_NAME/$SOLUTION_NAME.HttpClient.Test/$SOLUTION_NAME.HttpClient.Test.csproj ./$SOLUTION_NAME.HttpClient.Test/$SOLUTION_NAME.HttpClient.Test.csproj

COPY ./$SOLUTION_NAME/$SOLUTION_NAME.Repository/$SOLUTION_NAME.Repository.csproj ./$SOLUTION_NAME.Repository/$SOLUTION_NAME.Repository.csproj
COPY ./$SOLUTION_NAME/$SOLUTION_NAME.Repository.Test/$SOLUTION_NAME.Repository.Test.csproj ./$SOLUTION_NAME.Repository.Test/$SOLUTION_NAME.Repository.Test.csproj

COPY ./$SOLUTION_NAME/$SOLUTION_NAME.Service/$SOLUTION_NAME.Service.csproj ./$SOLUTION_NAME.Service/$SOLUTION_NAME.Service.csproj
COPY ./$SOLUTION_NAME/$SOLUTION_NAME.Service.Test/$SOLUTION_NAME.Service.Test.csproj ./$SOLUTION_NAME.Service.Test/$SOLUTION_NAME.Service.Test.csproj

COPY ./$SOLUTION_NAME/$SOLUTION_NAME.Web/$SOLUTION_NAME.Web.csproj ./$SOLUTION_NAME.Web/$SOLUTION_NAME.Web.csproj
COPY ./$SOLUTION_NAME/$SOLUTION_NAME.Web.Test/$SOLUTION_NAME.Web.Test.csproj ./$SOLUTION_NAME.Web.Test/$SOLUTION_NAME.Web.Test.csproj

#Restore
FROM copy-files as restore
LABEL type="build"
ARG SOLUTION_NAME="Swashbuckle.Issue.Example"
ARG NEXUS_USERNAME
ARG NEXUS_PASSWORD
WORKDIR /app_build/$SOLUTION_NAME
RUN export NEXUS_USERNAME=$NEXUS_USERNAME; \
    export NEXUS_PASSWORD=$NEXUS_PASSWORD; \
    dotnet restore --no-cache --configfile NuGet.config
WORKDIR /app_build/$SOLUTION_NAME
COPY $SOLUTION_NAME .

#With restore completed, now copy over the entire application.
FROM restore as build-and-test
LABEL type="build"
ARG SOLUTION_NAME="Swashbuckle.Issue.Example"
ARG VERSION="0.0.0"
WORKDIR /app_build/$SOLUTION_NAME
#Unfortunately, there isn't a merge function as of the moment
RUN dotnet test --configuration Release --no-restore --results-directory ../unit-tests --logger "junit;LogFilePath=unit-tests/junit.xml"
RUN dotnet publish $SOLUTION_NAME.Web --output /dist --configuration Release --no-restore /p:Version=$VERSION
#$HOME/.dotnet/tools/dotnet-xunit-to-junit

#This target creates the runnable .NET Core based API server
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine as application
LABEL type="host"
ARG PRODUCT_NAME="PRODUCT_NAME"
ARG SERVICE_NAME="SERVICE_NAME"
ARG SOLUTION_NAME="Swashbuckle.Issue.Example"
ENV SOLUTION_NAME=$SOLUTION_NAME
RUN apk add --update --no-cache icu-libs
#This particular environment var does not require the double underscore.
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:5000
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
#host.docker.internal does not currently work on linux.  See: https://github.com/docker/for-linux/issues/264
# Set Connection Strings or other needed local environment variables here.
#ENV ConnectionStrings__SomePropertyName=server=host.docker.internal;port=3306;database=uam;user=dbuser;password=password;charset=utf8
WORKDIR /app
COPY --from=build-and-test /dist .
LABEL product=$PRODUCT_NAME
LABEL service=$SERVICE_NAME
EXPOSE 5000
#If the dll name is mis-spellled, you will get a message about installing the SDK.
ENTRYPOINT dotnet ${SOLUTION_NAME}.Web.dll


