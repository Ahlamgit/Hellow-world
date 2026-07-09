#!/usr/bin/env bash
# Build and start the staging Docker stack, then run smoke tests.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-$ROOT/.env.staging}"
COMPOSE_FILE="$ROOT/docker-compose.staging.yml"
BASE_URL="${BASE_URL:-http://localhost:8080}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing $ENV_FILE — run: cp .env.staging.example .env.staging" >&2
  exit 1
fi

cd "$ROOT"

echo "Building and starting staging stack..."
docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" up -d --build

echo "Waiting for API readiness..."
deadline=$((SECONDS + 180))
until curl -sf "${BASE_URL%/}/api/v1/health/ready" >/dev/null 2>&1; do
  if (( SECONDS >= deadline )); then
    echo "Timed out waiting for ${BASE_URL}/api/v1/health/ready" >&2
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" logs api --tail 80
    exit 1
  fi
  sleep 3
done

"$ROOT/scripts/smoke-test.sh" "$BASE_URL"
echo "Staging deploy complete. Web: $BASE_URL  API: http://localhost:5001"
