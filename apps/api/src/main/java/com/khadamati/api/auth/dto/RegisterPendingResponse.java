package com.khadamati.api.auth.dto;

import com.khadamati.api.rbac.Role;

public record RegisterPendingResponse(
        String status,
        String email,
        Role role,
        String phoneE164,
        String message
) {
    public static RegisterPendingResponse otpRequired(String email, Role role, String phoneE164) {
        return new RegisterPendingResponse(
                "OTP_REQUIRED",
                email,
                role,
                phoneE164,
                "Verification code sent. Enter the code to activate your account.");
    }
}
