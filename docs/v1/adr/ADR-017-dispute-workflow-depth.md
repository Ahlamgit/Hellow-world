# ADR-017: Dispute Workflow Depth in V1

## Status
Open — Product must confirm V1 depth.

## Context
Booking lifecycle mentions disputes; Master Prompt emphasizes completion confirmation and quality follow-up. Full dispute center (evidence, escrow freeze, split refunds) may be V1.1/V2.

## Options
1. **V1 Minimal:** `DISPUTED` status + admin manual resolution + refund via admin refund rules  
2. **V1 Full:** Customer open dispute, evidence upload, timed escrow hold, structured outcomes  

## Decision
**Not assumed.** Recommend Option 1 for V1 unless Product mandates Option 2. Recorded as Blocked pending Product choice.
