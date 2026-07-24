# ADR-018: Dark Theme in V1 Design System

## Status
Open — depends on approved design assets.

## Context
Readiness request asks to confirm light + dark theme support. Master Prompt design principles do not explicitly mandate dark mode. ADR-010 requires design analysis from uploaded assets.

## Decision
1. Design system **architecture** must allow theme tokens (light required).  
2. **Dark theme** ships in V1 **only if** approved design assets include dark variants; otherwise deferred to V1.1 without redesign (token slots reserved).  
3. Do not invent a dark palette before assets.

## Consequences
UI spec lists dark theme as **conditional**. Not a backend blocker.
