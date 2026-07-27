package com.khadamati.api.legal.dto;

import java.util.List;

import com.khadamati.api.legal.DocumentType;

public record LegalComplianceResponse(
        boolean compliant,
        List<PendingDocument> pending
) {
    public record PendingDocument(DocumentType documentType, String requiredVersion, String title) {}
}
