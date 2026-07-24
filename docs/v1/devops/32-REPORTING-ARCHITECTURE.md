# 32. Reporting Architecture

**Document ID:** KHAD-V1-RPT  
**Status:** Draft for Approval  

---

## 32.1 Goals

Provide operational analytics for Admin and Store without overloading OLTP transactions.

## 32.2 V1 Approach

**Hybrid:**

1. **Online transactional reports** — simple aggregates via indexed SQL for near-real-time ops  
2. **Materialized read models** — scheduled refresh for dashboards  
3. **Exports** — CSV/XLSX generated async for large ranges  

Full warehouse/BI (ClickHouse/BigQuery/etc.) is V2 candidate unless Q-RPT-001 mandates earlier.

## 32.3 Report Domains

| Audience | Reports |
|----------|---------|
| Admin | GMV, payments success, commissions, active users, onboarding funnel, quality KPIs, ads performance |
| Store | Orders, bookings, revenue, top products/services, customers count |
| Craftsman | Earnings, completed jobs, ratings (via mobile KPIs APIs) |

## 32.4 Architecture

```text
OLTP (Postgres)
  ├─ reporting views / SQL functions
  ├─ materialized tables refreshed by jobs
  └─ export worker → object storage download link
Admin/Store UI → Reporting API → read replica (preferred) / primary (acceptable early)
```

## 32.5 Consistency Expectations

- Dashboard metrics may lag (e.g., 5–15 minutes)  
- Financial settlement overview should reconcile to payment/commission tables with clear “as of” timestamp  

## 32.6 Security

- Permission-gated  
- Store reports scoped to store_id  
- PII minimization in exports; audited downloads  

## 32.7 Questions Requiring Business Decision

`Q-RPT-001` warehouse in V1?, KPI targets for dashboards, export retention
