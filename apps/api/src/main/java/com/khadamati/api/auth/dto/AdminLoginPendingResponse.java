package com.khadamati.api.auth.dto;

public record AdminLoginPendingResponse(
        String status,
        String email,
        String phoneE164,
        String message
) {
    public static AdminLoginPendingResponse mfaRequired(String email, String phoneE164) {
        return new AdminLoginPendingResponse(
                "MFA_REQUIRED",
                email,
                phoneE164,
                "Enter the verification code sent to your admin phone.");
    }
}
