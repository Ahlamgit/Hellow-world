package com.khadamati.api.auth.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;

public record OtpRequest(
        @NotBlank
        @Pattern(regexp = "^\\+[1-9]\\d{6,14}$", message = "Phone must be E.164 format")
        String phoneE164
) {}
