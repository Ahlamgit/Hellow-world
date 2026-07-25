# ADR-010: Design Inspiration, Not Pixel Clone

## Status
Accepted — 2026-07-24 (Master Prompt v1.0)

## Decision
Uploaded UI/design assets are **visual inspiration**.

Engineering must:
1. Analyze UI/UX  
2. Improve where justified (accessibility, consistency, performance)  
3. Produce design tokens, component inventory, screen hierarchy, navigation map, a11y + responsive specs  
4. Implement a reusable design system (modern, premium, minimal, professional)

Do **not** blindly pixel-clone. Do **not** invent an unrelated generic template look.

Visual system targets: 12–20px radii, soft shadows, 8-pt spacing, 150–250ms motion, large touch targets, accessible contrast; AR-RTL + EN-LTR.

## Consequences
UI work gated on design analysis deliverables, not raw frame copying.
