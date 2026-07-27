package com.khadamati.api.legal.dto;

import jakarta.validation.constraints.NotBlank;

public record AcceptLegalDocumentsRequest(
        @NotBlank String termsVersion,
        @NotBlank String privacyVersion,
        @NotBlank String language
) {}
