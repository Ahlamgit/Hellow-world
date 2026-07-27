package com.khadamati.api.auth.service;

import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.UUID;

import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.khadamati.api.auth.AdminPortalRequiredException;
import com.khadamati.api.auth.AuthenticationFailedException;
import com.khadamati.api.auth.dto.AuthTokensResponse;
import com.khadamati.api.auth.dto.ForgotPasswordRequest;
import com.khadamati.api.auth.dto.LoginCompleteRequest;
import com.khadamati.api.auth.dto.LoginRequest;
import com.khadamati.api.auth.dto.LoginResponse;
import com.khadamati.api.auth.dto.OtpVerifyRequest;
import com.khadamati.api.auth.dto.RegisterPendingResponse;
import com.khadamati.api.auth.dto.RegisterRequest;
import com.khadamati.api.auth.dto.RegisterVerifyOtpRequest;
import com.khadamati.api.config.AppProperties;
import com.khadamati.api.integration.ports.SmsSenderPort;
import com.khadamati.api.legal.LegalDocumentService;
import com.khadamati.api.rbac.Role;
import com.khadamati.api.user.UserEntity;
import com.khadamati.api.user.UserRepository;

@Service
public class AuthService {

    private static final List<Role> PUBLIC_ROLES = List.of(Role.CUSTOMER, Role.CRAFTSMAN, Role.STORE);

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtService jwtService;
    private final SmsSenderPort smsSenderPort;
    private final AppProperties appProperties;
    private final LegalDocumentService legalDocumentService;

    public AuthService(
            UserRepository userRepository,
            PasswordEncoder passwordEncoder,
            JwtService jwtService,
            SmsSenderPort smsSenderPort,
            AppProperties appProperties,
            LegalDocumentService legalDocumentService) {
        this.userRepository = userRepository;
        this.passwordEncoder = passwordEncoder;
        this.jwtService = jwtService;
        this.smsSenderPort = smsSenderPort;
        this.appProperties = appProperties;
        this.legalDocumentService = legalDocumentService;
    }

    @Transactional
    public RegisterPendingResponse register(RegisterRequest request) {
        validatePublicRole(request.role());
        if (!request.passwordsMatch()) {
            throw new IllegalArgumentException("Password and confirm password must match");
        }
        if (!request.acceptTerms() || !request.acceptPrivacy()) {
            throw new IllegalArgumentException("Legal acceptance is required");
        }

        String normalizedEmail = normalizeEmail(request.email());
        String normalizedPhone = normalizePhone(request.phoneE164());
        ensureUniqueForRole(normalizedEmail, normalizedPhone, request.role());

        legalDocumentService.validateRegistrationVersions(
                request.termsVersion(), request.privacyVersion(), request.language());

        Instant now = Instant.now();
        UserEntity user = new UserEntity();
        user.setId(UUID.randomUUID());
        user.setEmail(normalizedEmail);
        user.setPhoneE164(normalizedPhone);
        user.setRole(request.role());
        user.setFirstName(request.firstName().trim());
        user.setLastName(request.lastName().trim());
        user.setPasswordHash(passwordEncoder.encode(request.password()));
        user.setPhoneVerified(false);
        user.setActive(false);
        user.setCreatedAt(now);
        user.setUpdatedAt(now);
        userRepository.save(user);

        legalDocumentService.recordAcceptances(
                user,
                request.termsVersion(),
                request.privacyVersion(),
                request.language());

        requestOtp(normalizedPhone);

        return RegisterPendingResponse.otpRequired(normalizedEmail, request.role(), normalizedPhone);
    }

    @Transactional
    public AuthTokensResponse verifyRegistrationOtp(RegisterVerifyOtpRequest request) {
        String email = normalizeEmail(request.email());
        UserEntity user = userRepository.findByEmailAndRole(email, request.role())
                .orElseThrow(() -> new AuthenticationFailedException("Invalid credentials"));

        if (!verifyOtp(user.getPhoneE164(), request.otpCode())) {
            throw new AuthenticationFailedException("Invalid verification code");
        }

        user.setPhoneVerified(true);
        user.setActive(true);
        user.setUpdatedAt(Instant.now());
        userRepository.save(user);

        return tokensFor(user);
    }

    public LoginResponse login(LoginRequest request) {
        List<UserEntity> matches = authenticatePublicUsers(request.identifier(), request.password());
        if (matches.isEmpty()) {
            if (hasValidAdminCredentials(request.identifier(), request.password())) {
                throw new AdminPortalRequiredException();
            }
            throw new AuthenticationFailedException("Invalid credentials");
        }
        if (matches.size() == 1) {
            UserEntity user = matches.getFirst();
            ensureActive(user);
            return LoginResponse.success(tokensFor(user));
        }
        List<Role> roles = matches.stream().map(UserEntity::getRole).distinct().toList();
        return LoginResponse.roleSelectionRequired(roles);
    }

    public AuthTokensResponse completeLogin(LoginCompleteRequest request) {
        validatePublicRole(request.role());
        UserEntity user = authenticatePublicUsers(request.identifier(), request.password()).stream()
                .filter(u -> u.getRole() == request.role())
                .findFirst()
                .orElseThrow(() -> new AuthenticationFailedException("Invalid credentials"));
        ensureActive(user);
        return tokensFor(user);
    }

    public AuthTokensResponse adminLogin(LoginRequest request) {
        UserEntity user = findByIdentifier(request.identifier()).stream()
                .filter(u -> isAdminRole(u.getRole()))
                .filter(u -> passwordEncoder.matches(request.password(), u.getPasswordHash()))
                .findFirst()
                .orElseThrow(() -> new AuthenticationFailedException("Invalid credentials"));
        ensureActive(user);
        return tokensFor(user);
    }

    public void forgotPassword(ForgotPasswordRequest request) {
        if (findByIdentifier(request.identifier()).isEmpty()) {
            throw new AuthenticationFailedException("No account found for this identifier");
        }
    }

    public void requestOtp(String phoneE164) {
        String otp = appProperties.devMode() ? appProperties.mockOtpCode() : UUID.randomUUID().toString().substring(0, 6);
        smsSenderPort.sendOtp(normalizePhone(phoneE164), otp);
    }

    public boolean verifyOtp(OtpVerifyRequest request) {
        return verifyOtp(request.phoneE164(), request.otpCode());
    }

    private boolean verifyOtp(String phoneE164, String otpCode) {
        if (appProperties.devMode()) {
            return appProperties.mockOtpCode().equals(otpCode);
        }
        return false;
    }

    private List<UserEntity> authenticatePublicUsers(String identifier, String password) {
        List<UserEntity> candidates = findByIdentifier(identifier);
        List<UserEntity> matches = new ArrayList<>();
        for (UserEntity user : candidates) {
            if (isAdminRole(user.getRole())) {
                continue;
            }
            if (!user.isActive()) {
                continue;
            }
            if (passwordEncoder.matches(password, user.getPasswordHash())) {
                matches.add(user);
            }
        }
        return matches;
    }

    private boolean hasValidAdminCredentials(String identifier, String password) {
        return findByIdentifier(identifier).stream()
                .filter(u -> isAdminRole(u.getRole()))
                .anyMatch(u -> passwordEncoder.matches(password, u.getPasswordHash()));
    }

    private List<UserEntity> findByIdentifier(String identifier) {
        String trimmed = identifier.trim();
        if (trimmed.contains("@")) {
            return userRepository.findByEmail(normalizeEmail(trimmed));
        }
        return userRepository.findByPhoneE164(normalizePhone(trimmed));
    }

    private void ensureUniqueForRole(String email, String phone, Role role) {
        if (userRepository.findByEmailAndRole(email, role).isPresent()) {
            throw new IllegalArgumentException("An account of this type already exists for this email");
        }
        userRepository.findByPhoneE164(phone).stream()
                .filter(u -> u.getRole() == role)
                .findFirst()
                .ifPresent(u -> {
                    throw new IllegalArgumentException("An account of this type already exists for this phone number");
                });
    }

    private void ensureActive(UserEntity user) {
        if (!user.isActive()) {
            throw new AuthenticationFailedException("Account is not activated. Complete phone verification.");
        }
    }

    private AuthTokensResponse tokensFor(UserEntity user) {
        return AuthTokensResponse.bearer(
                jwtService.createAccessToken(user.getEmail(), user.getRole(), user.getId()),
                jwtService.createRefreshToken(user.getEmail(), user.getRole(), user.getId()),
                user.getRole());
    }

    private static String normalizeEmail(String email) {
        return email.trim().toLowerCase(Locale.ROOT);
    }

    private static String normalizePhone(String phone) {
        String trimmed = phone.trim();
        if (trimmed.startsWith("+")) {
            return trimmed;
        }
        return "+" + trimmed.replaceAll("\\D", "");
    }

    private static void validatePublicRole(Role role) {
        if (!PUBLIC_ROLES.contains(role)) {
            throw new IllegalArgumentException("Registration is not allowed for this account type");
        }
    }

    private static boolean isAdminRole(Role role) {
        return role == Role.ADMIN || role == Role.FINANCE_ADMIN || role == Role.SUPER_ADMIN;
    }
}
