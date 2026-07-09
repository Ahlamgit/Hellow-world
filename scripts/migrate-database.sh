#!/usr/bin/env bash
# Apply EF Core migrations against the database in .env.staging (or pass --env-file).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-$ROOT/.env.staging}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing $ENV_FILE — copy .env.staging.example first." >&2
  exit 1
fi

set -a
# shellcheck disable=SC1090
source "$ENV_FILE"
set +a

if [[ -z "${ConnectionStrings__DefaultConnection:-}" ]]; then
  echo "ConnectionStrings__DefaultConnection is not set in $ENV_FILE" >&2
  exit 1
fi

echo "Applying migrations to ${DB_NAME:-database}..."
dotnet ef database update \
  --project "$ROOT/src/backend/Khadamati.Infrastructure/Khadamati.Infrastructure.csproj" \
  --startup-project "$ROOT/src/backend/Khadamati.API/Khadamati.API.csproj" \
  --connection "$ConnectionStrings__DefaultConnection"

echo "Migrations applied."
