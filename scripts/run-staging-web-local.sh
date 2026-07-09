#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT/src/web"
VITE_API_URL="${VITE_API_URL:-http://localhost:5001/api/v1}" npm run build
exec npx vite preview --host 0.0.0.0 --port 8080
