# ADR-023: Design Asset Readiness Gate

**Status:** Accepted — 2026-07-24  
**Related:** ADR-010, ADR-018  

---

## Decision

**UI coding remains blocked** until required design inputs exist and are reviewed.

---

## Required Inputs

- Branding assets  
- Logo  
- Colors  
- Design references  
- Videos / screens  
- Final design direction  

---

## After Assets Are Available — Produce Before UI Coding

1. UI analysis  
2. UX analysis  
3. Design tokens  
4. Component inventory  
5. Screen specifications  

(Per Master Prompt §14 and ADR-010: inspiration + improve, not blind pixel clone.)

---

## Dark Theme

Per ADR-018: light theme required; dark theme only if assets include it.

---

## Consequences

Architecture and API work may continue. **No Flutter/React screen implementation** until this gate passes and is recorded in readiness report amendment.
