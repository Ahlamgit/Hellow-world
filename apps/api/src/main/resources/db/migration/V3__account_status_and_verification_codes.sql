-- Account lifecycle status and persisted verification codes

ALTER TABLE khadamati.users
    ADD COLUMN account_status VARCHAR(32) NOT NULL DEFAULT 'PENDING_VERIFICATION';

UPDATE khadamati.users SET account_status = 'ACTIVE' WHERE active = TRUE;
UPDATE khadamati.users SET account_status = 'PENDING_VERIFICATION' WHERE active = FALSE;

CREATE TABLE khadamati.verification_codes (
    id UUID PRIMARY KEY,
    phone_e164 VARCHAR(20) NOT NULL,
    purpose VARCHAR(32) NOT NULL,
    code_hash VARCHAR(255) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    attempt_count INT NOT NULL DEFAULT 0,
    max_attempts INT NOT NULL DEFAULT 5,
    consumed_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_verification_phone_purpose ON khadamati.verification_codes(phone_e164, purpose);
