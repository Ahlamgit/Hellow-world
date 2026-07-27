package com.khadamati.api.legal.dto;

import com.khadamati.api.legal.DocumentType;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

public record PublishLegalDocumentRequest(
        @NotNull DocumentType documentType,
        @NotBlank String version,
        @NotBlank String title,
        @NotBlank String content,
        @NotBlank String language
) {}
