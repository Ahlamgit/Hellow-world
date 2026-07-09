FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Khadamati.sln ./
COPY Khadamati.Domain/ Khadamati.Domain/
COPY Khadamati.Application/ Khadamati.Application/
COPY Khadamati.Infrastructure/ Khadamati.Infrastructure/
COPY Khadamati.API/ Khadamati.API/
RUN dotnet restore
RUN dotnet publish Khadamati.API/Khadamati.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/api/v1/health || exit 1
ENTRYPOINT ["dotnet", "Khadamati.API.dll"]
