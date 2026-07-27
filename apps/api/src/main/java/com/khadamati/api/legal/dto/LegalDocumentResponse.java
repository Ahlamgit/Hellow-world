package com.khadamati.api.legal.dto;

import java.time.Instant;
import java.util.UUID;

import com.khadamati.api.legal.DocumentType;

public record LegalDocumentResponse(
        UUID id,
        DocumentType documentType,
        String version,
        String title,
        String content,
        String language,
        boolean active,
        Instant updatedAt
) {}
