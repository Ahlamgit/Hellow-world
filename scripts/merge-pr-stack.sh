#!/usr/bin/env bash
# Merge the open feature PR stack into main (oldest first).
# Requires: gh CLI authenticated, all PR branches pushed.
set -euo pipefail

PRS=(17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32)

echo "Merging PR stack into main (squash merge, oldest first)..."
for pr in "${PRS[@]}"; do
  if gh pr view "$pr" --json state,mergeable -q '.state' 2>/dev/null | grep -q OPEN; then
    echo "Merging PR #$pr..."
    gh pr merge "$pr" --squash --delete-branch || {
      echo "PR #$pr could not be merged — resolve conflicts and re-run." >&2
      exit 1
    }
  else
    echo "Skipping PR #$pr (not open or not found)."
  fi
done

echo "Done. Verify with: git fetch origin && git log --oneline origin/main -5"
