# ADR-027: Store Product Catalog Advertising (Non-Transactional)

**Status:** Accepted — 2026-07-24  
**Amends:** ADR-002  

---

## Decision

Stores **may** create a **product catalog for advertising / showcase** purposes only.

Stores are **not** e-commerce sellers in V1.

### Supported (promotional catalog)

- Product name  
- Product images  
- Descriptions  
- Categories  
- Brands / models  
- Promotional information  
- Contact / inquiry call-to-action  

### Purpose
Showcase products · advertise · generate customer inquiries · promote store business  

### Forbidden
Cart · checkout · online product transaction · inventory tracking · order management · product payment flow · delivery/warehouse  

---

## Related Store Capabilities (Confirmed)

| Area | Included |
|------|----------|
| Store profile (logo, business info, location, service areas, contact, verification, status) | Yes |
| Service management (listings, categories, pricing, areas, availability) | Yes |
| Product catalog advertising | Yes (non-transactional) |
| Store subscriptions (Admin-managed plans: visibility, featured, ad limits, promoted services/catalog caps) | Yes |
| Advertise services + catalog items (subject to subscription + admin promotion rules) | Yes |
| Analytics (profile/service/catalog views, ad impressions, inquiries, booking conversions) | Yes |

---

## Database Impact

| Entity | Purpose |
|--------|---------|
| `catalog_items` | Promotional product entries (store_provider_id, name, description, brand, model, category_id, active) |
| `catalog_item_media` | Images |
| `catalog_inquiries` | Customer inquiry/lead (no order) |
| **Forbidden tables** | `carts`, `cart_items`, `product_orders`, `inventory_levels`, `shipments`, `warehouses` |

Clear naming: `catalog_items` / `promotional_products` — **never** `orders` for products.

---

## API Impact

Store-scoped (own data only):

- `/store/me` profile  
- `/store/listings` services  
- `/store/catalog-items` promotional catalog CRUD  
- `/store/promotions` / ad placements eligibility  
- `/store/subscriptions` status (Admin defines plans)  
- `/store/analytics`  

Customer (read/inquiry):

- Browse store catalog showcase  
- `POST` inquiry on catalog item (lead) — **not** payment  

---

## Authorization

Store operators manage **only their own** store profile, services, catalog, promotions. Admin manages subscription plans and promotion rules.

---

## Consequences

FTM/Master Prompt/DB/API must distinguish **catalog advertising** from **e-commerce**. Q-PRD-001 = promotional catalog model; Q-PAY-001 remains N/A for products; Q-SUB-001 = store subscriptions **in scope** (Admin-managed).
