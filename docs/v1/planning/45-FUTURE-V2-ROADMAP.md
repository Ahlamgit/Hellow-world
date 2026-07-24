# 45. Future Version 2 Roadmap

**Document ID:** KHAD-V1-V2  
**Status:** Draft for Approval  

V2 items are **out of V1 scope** unless business explicitly pulls them forward.

---

## 45.1 Platform Evolution

- Extract high-load modules (`payment`, `notification`, `reporting`) to independent services if metrics justify  
- Multi-region active deployments + data residency controls  
- Advanced multi-tenant / white-label modes  

## 45.2 Payments & Finance

- Additional payment gateways behind existing port  
- Stored cards / network tokens / recurring mandates at scale  
- Automated payout rails (bank transfers, wallets)  
- Full accounting exports / ERP connectors  
- Split payments marketplace models  

## 45.3 Marketplace Features

- Real-time chat / voice between customer and craftsman  
- Advanced matching / recommendations / dynamic pricing  
- Instant / ASAP dispatch with bidding  
- Bundled service packages  
- Warranty / spare parts marketplace expansions  

## 45.4 Trust & Safety

- Stronger liveness & device attestation  
- Continuous KYC refresh  
- Fraud scoring ML  
- Dispute center with evidence timelines  

## 45.5 Growth & Engagement

- Referral programs  
- Loyalty points  
- Advanced ad auctioning / CPM billing  
- Marketing automation journeys  

## 45.6 Data & Intelligence

- Warehouse + BI semantic layer  
- Real-time ops anomaly detection  
- Churn prediction for craftsmen/stores  

## 45.7 Client Platforms

- Responsive public web booking portal (non-admin)  
- Tablet-optimized craftsman mode  
- Partner APIs for third-party integrations  

## 45.8 Dependency on V1

V2 assumes V1 delivered clean module boundaries, payment abstraction, outbox events, and auditable ledgers. Skipping those increases V2 cost disproportionately.
