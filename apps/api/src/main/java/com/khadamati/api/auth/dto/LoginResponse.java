package com.khadamati.api.auth.dto;

import java.util.List;

import com.khadamati.api.rbac.Role;

public record LoginResponse(
        String status,
        AuthTokensResponse tokens,
        List<Role> availableRoles
) {
    public static LoginResponse success(AuthTokensResponse tokens) {
        return new LoginResponse("SUCCESS", tokens, List.of());
    }

    public static LoginResponse roleSelectionRequired(List<Role> roles) {
        return new LoginResponse("ROLE_SELECTION_REQUIRED", null, roles);
    }
}
