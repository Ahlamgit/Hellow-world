package com.khadamati.api.auth;

public class VerificationCodeExpiredException extends RuntimeException {

    public VerificationCodeExpiredException() {
        super("Verification code expired.");
    }
}
