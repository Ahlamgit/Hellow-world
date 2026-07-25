package com.khadamati.api.integration.adapters.mock;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import com.khadamati.api.integration.ports.EmailSenderPort;

@Component
public class MockEmailSenderAdapter implements EmailSenderPort {

    private static final Logger log = LoggerFactory.getLogger(MockEmailSenderAdapter.class);

    @Override
    public void sendVerificationEmail(String email, String verificationToken) {
        log.info("[DEV_MODE EMAIL] Verification for {} token={}", email, verificationToken);
    }
}
