package com.khadamati.api.auth.service;

import java.time.Duration;
import java.time.Instant;
import java.util.UUID;

import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.khadamati.api.auth.AuthenticationFailedException;
import com.khadamati.api.auth.VerificationAttemptsExceededException;
import com.khadamati.api.auth.VerificationCodeEntity;
import com.khadamati.api.auth.VerificationCodeExpiredException;
import com.khadamati.api.auth.VerificationCodeRepository;
import com.khadamati.api.auth.VerificationPurpose;
import com.khadamati.api.config.AppProperties;
import com.khadamati.api.integration.ports.SmsSenderPort;

@Service
public class VerificationCodeService {

    private static final int MAX_ATTEMPTS = 5;
    private static final Duration CODE_TTL = Duration.ofMinutes(10);

    private final VerificationCodeRepository verificationCodeRepository;
    private final PasswordEncoder passwordEncoder;
    private final SmsSenderPort smsSenderPort;
    private final AppProperties appProperties;

    public VerificationCodeService(
            VerificationCodeRepository verificationCodeRepository,
            PasswordEncoder passwordEncoder,
            SmsSenderPort smsSenderPort,
            AppProperties appProperties) {
        this.verificationCodeRepository = verificationCodeRepository;
        this.passwordEncoder = passwordEncoder;
        this.smsSenderPort = smsSenderPort;
        this.appProperties = appProperties;
    }

    @Transactional
    public void sendRegistrationCode(String phoneE164) {
        sendCode(phoneE164, VerificationPurpose.REGISTRATION);
    }

    @Transactional
    public void verifyRegistrationCode(String phoneE164, String otpCode) {
        verifyCode(phoneE164, VerificationPurpose.REGISTRATION, otpCode);
    }

    @Transactional
    public void sendCode(String phoneE164, VerificationPurpose purpose) {
        String normalizedPhone = normalizePhone(phoneE164);
        consumeActiveCodes(normalizedPhone, purpose);

        String code = generateCode();
        Instant now = Instant.now();

        VerificationCodeEntity entity = new VerificationCodeEntity();
        entity.setId(UUID.randomUUID());
        entity.setPhoneE164(normalizedPhone);
        entity.setPurpose(purpose);
        entity.setCodeHash(passwordEncoder.encode(code));
        entity.setExpiresAt(now.plus(CODE_TTL));
        entity.setAttemptCount(0);
        entity.setMaxAttempts(MAX_ATTEMPTS);
        entity.setCreatedAt(now);
        verificationCodeRepository.save(entity);

        smsSenderPort.sendOtp(normalizedPhone, code);
    }

    @Transactional
    public void verifyCode(String phoneE164, VerificationPurpose purpose, String otpCode) {
        String normalizedPhone = normalizePhone(phoneE164);
        VerificationCodeEntity active = verificationCodeRepository
                .findFirstByPhoneE164AndPurposeAndConsumedAtIsNullOrderByCreatedAtDesc(
                        normalizedPhone, purpose)
                .orElseThrow(() -> new AuthenticationFailedException("Invalid verification code"));

        Instant now = Instant.now();
        if (active.getExpiresAt().isBefore(now)) {
            throw new VerificationCodeExpiredException();
        }
        if (active.getAttemptCount() >= active.getMaxAttempts()) {
            throw new VerificationAttemptsExceededException();
        }

        if (!passwordEncoder.matches(otpCode.trim(), active.getCodeHash())) {
            active.setAttemptCount(active.getAttemptCount() + 1);
            verificationCodeRepository.save(active);
            if (active.getAttemptCount() >= active.getMaxAttempts()) {
                throw new VerificationAttemptsExceededException();
            }
            throw new AuthenticationFailedException("Invalid verification code");
        }

        active.setConsumedAt(now);
        verificationCodeRepository.save(active);
    }

    private void consumeActiveCodes(String phoneE164, VerificationPurpose purpose) {
        verificationCodeRepository
                .findFirstByPhoneE164AndPurposeAndConsumedAtIsNullOrderByCreatedAtDesc(phoneE164, purpose)
                .ifPresent(existing -> {
                    existing.setConsumedAt(Instant.now());
                    verificationCodeRepository.save(existing);
                });
    }

    private String generateCode() {
        if (appProperties.devMode()) {
            return appProperties.mockOtpCode();
        }
        return String.format("%06d", (int) (Math.random() * 1_000_000));
    }

    private static String normalizePhone(String phone) {
        String trimmed = phone.trim();
        if (trimmed.startsWith("+")) {
            return trimmed;
        }
        return "+" + trimmed.replaceAll("\\D", "");
    }
}
