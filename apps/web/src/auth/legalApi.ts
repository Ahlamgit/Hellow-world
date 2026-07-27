export type LegalDocumentType = 'TERMS' | 'PRIVACY';

export type LegalDocument = {
  id: string;
  documentType: LegalDocumentType;
  version: string;
  title: string;
  content: string;
  language: string;
  active: boolean;
  updatedAt: string;
};

export type PendingLegalDocument = {
  documentType: LegalDocumentType;
  requiredVersion: string;
  title: string;
};

export type LegalCompliance = {
  compliant: boolean;
  pending: PendingLegalDocument[];
};

const API_BASE = import.meta.env.VITE_API_URL ?? '/api/v1';

function authHeaders(accessToken: string): HeadersInit {
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${accessToken}`,
  };
}

export async function fetchLegalDocument(
  type: 'terms' | 'privacy',
  lang: string,
): Promise<LegalDocument> {
  const response = await fetch(`${API_BASE}/legal/${type}?lang=${encodeURIComponent(lang)}`);
  if (!response.ok) {
    throw new Error('Failed to load legal document');
  }
  return (await response.json()) as LegalDocument;
}

export async function fetchLegalCompliance(accessToken: string, lang: string): Promise<LegalCompliance> {
  const response = await fetch(`${API_BASE}/legal/compliance?lang=${encodeURIComponent(lang)}`, {
    headers: authHeaders(accessToken),
  });
  if (!response.ok) {
    throw new Error('Failed to check legal compliance');
  }
  return (await response.json()) as LegalCompliance;
}

export async function acceptLegalDocuments(
  accessToken: string,
  termsVersion: string,
  privacyVersion: string,
  language: string,
): Promise<LegalCompliance> {
  const response = await fetch(`${API_BASE}/legal/accept`, {
    method: 'POST',
    headers: authHeaders(accessToken),
    body: JSON.stringify({ termsVersion, privacyVersion, language }),
  });
  if (!response.ok) {
    const body = (await response.json().catch(() => ({}))) as { error?: string };
    throw new Error(body.error ?? 'Failed to record legal acceptance');
  }
  return (await response.json()) as LegalCompliance;
}

export async function listAdminLegalDocuments(accessToken: string): Promise<LegalDocument[]> {
  const response = await fetch(`${API_BASE}/admin/legal/documents`, {
    headers: authHeaders(accessToken),
  });
  if (!response.ok) {
    throw new Error('Failed to load legal documents');
  }
  return (await response.json()) as LegalDocument[];
}

export async function publishLegalDocument(
  accessToken: string,
  payload: {
    documentType: LegalDocumentType;
    version: string;
    title: string;
    content: string;
    language: string;
  },
): Promise<LegalDocument> {
  const response = await fetch(`${API_BASE}/admin/legal/documents`, {
    method: 'POST',
    headers: authHeaders(accessToken),
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    const body = (await response.json().catch(() => ({}))) as { error?: string };
    throw new Error(body.error ?? 'Failed to publish legal document');
  }
  return (await response.json()) as LegalDocument;
}

