FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /project/
EXPOSE 80
COPY wait-for-it.sh /project/
COPY command.sh /project/

FROM microsoft/dotnet:2.1-sdk AS buildApi
WORKDIR /src
COPY . .
COPY NuGet.release.Config ./SplitPackage.Web.Host/NuGet.Config
WORKDIR /src/src/SplitPackage.Web.Host
RUN dotnet publish SplitPackage.Web.Host.csproj -c Release -o /api

FROM microsoft/dotnet:2.1-sdk AS buildMigrator
WORKDIR /src
COPY . .
COPY NuGet.release.Config ./SplitPackage.Migrator/NuGet.Config
WORKDIR /src/src/SplitPackage.Migrator
RUN dotnet publish SplitPackage.Migrator.csproj -c Release -o /migrator

FROM base AS final
COPY --from=buildMigrator /migrator ./migrator
COPY --from=buildApi /api ./api
ENTRYPOINT ["/bin/bash", "/project/wait-for-it.sh"]