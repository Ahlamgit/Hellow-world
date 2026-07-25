-- Sprint 0 baseline migration (A6)
-- Schema bootstrap only — no business domain tables (BLOCKER-006)

CREATE SCHEMA IF NOT EXISTS khadamati;

COMMENT ON SCHEMA khadamati IS 'KHADAMATI application schema — domain tables added post Sprint 0';
