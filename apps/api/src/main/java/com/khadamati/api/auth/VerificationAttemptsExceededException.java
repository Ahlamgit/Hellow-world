package com.khadamati.api.auth;

public class VerificationAttemptsExceededException extends RuntimeException {

    public VerificationAttemptsExceededException() {
        super("Too many verification attempts. Request a new code.");
    }
}
