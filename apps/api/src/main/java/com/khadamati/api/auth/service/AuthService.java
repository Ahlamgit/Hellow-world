package com.khadamati.api.auth.service;

import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import jakarta.annotation.PostConstruct;

import com.khadamati.api.auth.AuthenticationFailedException;
import com.khadamati.api.auth.dto.AuthTokensResponse;
import com.khadamati.api.auth.dto.LoginRequest;
import com.khadamati.api.auth.dto.OtpVerifyRequest;
import com.khadamati.api.auth.dto.RegisterRequest;
import com.khadamati.api.config.AppProperties;
import com.khadamati.api.integration.ports.SmsSenderPort;
import com.khadamati.api.rbac.Role;

/**
 * In-memory auth foundation for Sprint 0 — replaced with persisted users post domain schema.
 */
@Service
public class AuthService {

    private final Map<String, InMemoryUser> users = new ConcurrentHashMap<>();
    private final PasswordEncoder passwordEncoder;
    private final JwtService jwtService;
    private final SmsSenderPort smsSenderPort;
    private final AppProperties appProperties;

    public AuthService(
            PasswordEncoder passwordEncoder,
            JwtService jwtService,
            SmsSenderPort smsSenderPort,
            AppProperties appProperties) {
        this.passwordEncoder = passwordEncoder;
        this.jwtService = jwtService;
        this.smsSenderPort = smsSenderPort;
        this.appProperties = appProperties;
    }

    public AuthTokensResponse register(RegisterRequest request) {
        if (users.containsKey(request.email())) {
            throw new IllegalArgumentException("Email already registered");
        }
        users.put(request.email(), new InMemoryUser(request.email(), passwordEncoder.encode(request.password()), request.role()));
        return tokensFor(request.email(), request.role());
    }

    @PostConstruct
    void seedDevUsers() {
        if (!appProperties.devMode()) {
            return;
        }
        seedUser("customer@khadamati.local", "password123", Role.CUSTOMER);
        seedUser("provider@khadamati.local", "password123", Role.CRAFTSMAN);
        seedUser("store@khadamati.local", "password123", Role.STORE);
        seedUser("admin@khadamati.local", "password123", Role.ADMIN);
    }

    private void seedUser(String email, String password, Role role) {
        users.put(email, new InMemoryUser(email, passwordEncoder.encode(password), role));
    }

    public AuthTokensResponse login(LoginRequest request) {
        InMemoryUser user = users.get(request.email());
        if (user == null || !passwordEncoder.matches(request.password(), user.passwordHash())) {
            throw new AuthenticationFailedException("Invalid credentials");
        }
        return tokensFor(user.email(), user.role());
    }

    public void requestOtp(String phoneE164) {
        String otp = appProperties.devMode() ? appProperties.mockOtpCode() : UUID.randomUUID().toString().substring(0, 6);
        smsSenderPort.sendOtp(phoneE164, otp);
    }

    public boolean verifyOtp(OtpVerifyRequest request) {
        if (appProperties.devMode()) {
            return appProperties.mockOtpCode().equals(request.otpCode());
        }
        return false;
    }

    private AuthTokensResponse tokensFor(String email, Role role) {
        return AuthTokensResponse.bearer(
                jwtService.createAccessToken(email, role),
                jwtService.createRefreshToken(email, role),
                role);
    }

    private record InMemoryUser(String email, String passwordHash, Role role) {}
}
