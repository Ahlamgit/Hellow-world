#!/usr/bin/env bash
# Post-deploy smoke checks for staging or production web entrypoint.
set -euo pipefail

BASE_URL="${1:-http://localhost:8080}"
API_DIRECT="${API_DIRECT:-http://localhost:5001}"

pass() { echo "  OK  $*"; }
fail() { echo " FAIL $*" >&2; exit 1; }

echo "Smoke tests against $BASE_URL (API direct: $API_DIRECT)"

# Liveness via nginx proxy (docker web) or direct API fallback (host-native vite preview)
if curl -sf "${BASE_URL%/}/api/v1/health" 2>/dev/null | grep -q healthy; then
  pass "GET /api/v1/health (via web proxy)"
elif curl -sf "${API_DIRECT%/}/api/v1/health" 2>/dev/null | grep -q healthy; then
  pass "GET /api/v1/health (direct API — host-native mode)"
else
  fail "GET /api/v1/health"
fi

# Readiness via direct API port
curl -sf "${API_DIRECT%/}/api/v1/health/ready" | grep -q ready \
  && pass "GET /api/v1/health/ready (direct API)" \
  || fail "GET /api/v1/health/ready (direct API)"

# Integration report (development providers expected in staging)
curl -sf "${API_DIRECT%/}/api/v1/health/integrations" | grep -q productionReady \
  && pass "GET /api/v1/health/integrations" \
  || fail "GET /api/v1/health/integrations"

# SPA shell
curl -sf "${BASE_URL%/}/" | grep -qi html \
  && pass "GET / (SPA shell)" \
  || fail "GET / (SPA shell)"

# Bootstrap admin login (optional — requires .env.staging Bootstrap__* vars)
if [[ -f "$(dirname "${BASH_SOURCE[0]}")/../.env.staging" ]]; then
  set -a
  # shellcheck disable=SC1091
  source "$(dirname "${BASH_SOURCE[0]}")/../.env.staging"
  set +a
  if [[ -n "${Bootstrap__AdminEmail:-}" && -n "${Bootstrap__AdminPassword:-}" ]]; then
    login_response=$(curl -sf -X POST "${API_DIRECT%/}/api/v1/auth/login" \
      -H "Content-Type: application/json" \
      -d "{\"email\":\"${Bootstrap__AdminEmail}\",\"password\":\"${Bootstrap__AdminPassword}\"}") \
      && echo "$login_response" | grep -q accessToken \
      && pass "POST /api/v1/auth/login (bootstrap admin)" \
      || fail "POST /api/v1/auth/login (bootstrap admin)"
  fi
fi

echo "All smoke checks passed."
