# 39. Configuration Management

**Document ID:** KHAD-V1-CFG  
**Status:** Draft for Approval  

---

## 39.1 Layers of Configuration

| Layer | Changed by | Examples |
|-------|------------|----------|
| Build-time | Engineers | App IDs, compile flavors |
| Deploy-time env | DevOps | DB URL, API keys |
| Runtime DB settings | Admins | Commission rules, templates, feature flags, thresholds |
| Client remote config (optional) | Ops | Force upgrade flags |

## 39.2 Global Settings Module

Key/value JSON settings with:

- typed schema validation  
- caching with invalidation  
- audit on change  
- optional environment scope  

Examples: booking payment window minutes, reminder windows, maintenance mode.

## 39.3 Feature Flags

Boolean/percentage flags to decouple deploy from release:

- `payments.enabled`  
- `ads.serving.enabled`  
- `quality.auto_restrictions.enabled`  

## 39.4 Secrets Management

- Never store secrets in `global_settings` UI  
- Rotate IXOPAY and JWT secrets with documented runbooks  
- Dual-key overlap windows when rotating JWT  

## 39.5 Config Change Process

1. Staging validation  
2. Production change with audit  
3. Monitoring for error spikes  
4. Rollback via previous value  

## 39.6 Multi-region / Multi-currency Readiness

Settings should allow:

- `default_locale`, `supported_locales`  
- `default_currency`, `supported_currencies`  
- `default_region`  

Activation of multiple values is business-controlled (Q-LOC-*).
