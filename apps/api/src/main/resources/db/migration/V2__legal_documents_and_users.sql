-- Legal documents, user accounts, and acceptance audit (V1 foundation)

CREATE TABLE khadamati.users (
    id UUID PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    phone_e164 VARCHAR(20) NOT NULL,
    role VARCHAR(32) NOT NULL,
    first_name VARCHAR(80) NOT NULL,
    last_name VARCHAR(80) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    phone_verified BOOLEAN NOT NULL DEFAULT FALSE,
    active BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uk_users_email_role UNIQUE (email, role),
    CONSTRAINT uk_users_phone_role UNIQUE (phone_e164, role)
);

CREATE TABLE khadamati.legal_documents (
    id UUID PRIMARY KEY,
    document_type VARCHAR(32) NOT NULL,
    version VARCHAR(16) NOT NULL,
    title VARCHAR(255) NOT NULL,
    content TEXT NOT NULL,
    language VARCHAR(8) NOT NULL,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uk_legal_type_version_lang UNIQUE (document_type, version, language)
);

CREATE TABLE khadamati.user_legal_acceptance (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES khadamati.users(id),
    document_id UUID NOT NULL REFERENCES khadamati.legal_documents(id),
    document_type VARCHAR(32) NOT NULL,
    version_accepted VARCHAR(16) NOT NULL,
    accepted_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_user_legal_user_id ON khadamati.user_legal_acceptance(user_id);
CREATE INDEX idx_user_legal_document_id ON khadamati.user_legal_acceptance(document_id);

-- Placeholder legal content v1.0 (English)
INSERT INTO khadamati.legal_documents (id, document_type, version, title, content, language, active, created_at, updated_at) VALUES
('a1000001-0000-4000-8000-000000000001', 'TERMS', '1.0', 'KHADAMATI Terms & Conditions',
 '## 1. Introduction\nPlaceholder terms for localhost development. Replace before production.\n\n## 2. Services\nKHADAMATI connects customers with service providers in Lebanon.\n\n## 3. User obligations\nUsers must provide accurate information and use the platform lawfully.\n\n## 4. Liability\nService delivery is between customer and provider unless otherwise stated.',
 'en', TRUE, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('a1000001-0000-4000-8000-000000000002', 'PRIVACY', '1.0', 'KHADAMATI Privacy Policy',
 '## 1. Overview\nPlaceholder privacy policy for localhost development.\n\n## 2. Data we collect\nAccount details, booking information, and communications.\n\n## 3. How we use data\nTo operate the marketplace, improve services, and meet legal obligations.\n\n## 4. Your rights\nYou may request access or correction of your personal data.',
 'en', TRUE, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Placeholder legal content v1.0 (Arabic)
INSERT INTO khadamati.legal_documents (id, document_type, version, title, content, language, active, created_at, updated_at) VALUES
('a1000001-0000-4000-8000-000000000003', 'TERMS', '1.0', 'شروط وأحكام خدماتي',
 '## 1. المقدمة\nنص مؤقت للتطوير المحلي. يُستبدل قبل الإنتاج.\n\n## 2. الخدمات\nتربط خدماتي العملاء مع مزودي الخدمات في لبنان.\n\n## 3. التزامات المستخدم\nيجب تقديم معلومات صحيحة واستخدام المنصة بشكل قانوني.\n\n## 4. المسؤولية\nتقديم الخدمة بين العميل والمزود unless otherwise stated.',
 'ar', TRUE, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('a1000001-0000-4000-8000-000000000004', 'PRIVACY', '1.0', 'سياسة الخصوصية — خدماتي',
 '## 1. نظرة عامة\nنص مؤقت لسياسة الخصوصية للتطوير المحلي.\n\n## 2. البيانات التي نجمعها\nتفاصيل الحساب، معلومات الحجز، والتواصل.\n\n## 3. كيفية استخدام البيانات\nلتشغيل السوق وتحسين الخدمات والامتثال القانوني.\n\n## 4. حقوقك\nيمكنك طلب الوصول أو تصحيح بياناتك الشخصية.',
 'ar', TRUE, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
