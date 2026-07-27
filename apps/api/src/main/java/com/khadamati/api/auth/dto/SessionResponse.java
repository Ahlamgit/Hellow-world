package com.khadamati.api.auth.dto;

import com.khadamati.api.rbac.Role;
import com.khadamati.api.user.AccountStatus;

public record SessionResponse(
        String email,
        Role role,
        AccountStatus accountStatus,
        String firstName,
        String lastName,
        String phoneE164
) {}
