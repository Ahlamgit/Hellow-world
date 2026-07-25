package com.khadamati.api.integration.ports;

public interface EmailSenderPort {
    void sendVerificationEmail(String email, String verificationToken);
}
