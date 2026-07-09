#!/usr/bin/env bash
# Run staging API + web on the host against the SQL Server container (port 1434).
# Prerequisite: docker compose -f docker-compose.staging.yml --env-file .env.staging up -d sqlserver
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-$ROOT/.env.staging}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing $ENV_FILE" >&2
  exit 1
fi

set +H
set -a
# shellcheck disable=SC1090
source "$ENV_FILE"
set +a
set -H

# Host-native connection (container hostname sqlserver is not reachable from host)
export ConnectionStrings__DefaultConnection="Server=127.0.0.1,1434;Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=true"
export ASPNETCORE_ENVIRONMENT=Staging
export ASPNETCORE_URLS="${ASPNETCORE_URLS:-http://+:5001}"

cd "$ROOT"
dotnet run --project src/backend/Khadamati.API/Khadamati.API.csproj --no-launch-profile -c Release
