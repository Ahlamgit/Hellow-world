package com.khadamati.api.auth.dto;

import com.khadamati.api.rbac.Role;

import jakarta.validation.constraints.NotBlank;

public record LoginRequest(
        @NotBlank String identifier,
        @NotBlank String password
) {
    public boolean looksLikeEmail() {
        return identifier.contains("@");
    }
}
