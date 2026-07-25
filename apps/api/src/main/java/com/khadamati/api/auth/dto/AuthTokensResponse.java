package com.khadamati.api.auth.dto;

import com.khadamati.api.rbac.Role;

public record AuthTokensResponse(
        String accessToken,
        String refreshToken,
        String tokenType,
        Role role
) {
    public static AuthTokensResponse bearer(String accessToken, String refreshToken, Role role) {
        return new AuthTokensResponse(accessToken, refreshToken, "Bearer", role);
    }
}
