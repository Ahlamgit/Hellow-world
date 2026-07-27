package com.khadamati.api.auth.service;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import jakarta.annotation.PostConstruct;

import com.khadamati.api.auth.AdminPortalRequiredException;
import com.khadamati.api.auth.AuthenticationFailedException;
import com.khadamati.api.auth.dto.AuthTokensResponse;
import com.khadamati.api.auth.dto.ForgotPasswordRequest;
import com.khadamati.api.auth.dto.LoginCompleteRequest;
import com.khadamati.api.auth.dto.LoginRequest;
import com.khadamati.api.auth.dto.LoginResponse;
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

    private static final List<Role> PUBLIC_ROLES = List.of(Role.CUSTOMER, Role.CRAFTSMAN, Role.STORE);

    private final Map<String, InMemoryAccount> accountsById = new ConcurrentHashMap<>();
    private final Map<String, String> emailRoleIndex = new ConcurrentHashMap<>();
    private final Map<String, String> phoneRoleIndex = new ConcurrentHashMap<>();
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
        validatePublicRole(request.role());
        if (!request.passwordsMatch()) {
            throw new IllegalArgumentException("Password and confirm password must match");
        }
        String normalizedEmail = normalizeEmail(request.email());
        String normalizedPhone = normalizePhone(request.phoneE164());
        ensureUniqueForRole(normalizedEmail, normalizedPhone, request.role());

        String id = UUID.randomUUID().toString();
        InMemoryAccount account = new InMemoryAccount(
                id,
                normalizedEmail,
                normalizedPhone,
                request.firstName().trim(),
                request.lastName().trim(),
                passwordEncoder.encode(request.password()),
                request.role());
        storeAccount(account);
        return tokensFor(account);
    }

    public LoginResponse login(LoginRequest request) {
        List<InMemoryAccount> matches = authenticatePublicAccounts(request.identifier(), request.password());
        if (matches.isEmpty()) {
            if (hasValidAdminCredentials(request.identifier(), request.password())) {
                throw new AdminPortalRequiredException();
            }
            throw new AuthenticationFailedException("Invalid credentials");
        }
        if (matches.size() == 1) {
            return LoginResponse.success(tokensFor(matches.getFirst()));
        }
        List<Role> roles = matches.stream().map(InMemoryAccount::role).distinct().toList();
        return LoginResponse.roleSelectionRequired(roles);
    }

    public AuthTokensResponse completeLogin(LoginCompleteRequest request) {
        validatePublicRole(request.role());
        List<InMemoryAccount> matches = authenticatePublicAccounts(request.identifier(), request.password());
        InMemoryAccount account = matches.stream()
                .filter(a -> a.role() == request.role())
                .findFirst()
                .orElseThrow(() -> new AuthenticationFailedException("Invalid credentials"));
        return tokensFor(account);
    }

    public AuthTokensResponse adminLogin(LoginRequest request) {
        List<InMemoryAccount> matches = findByIdentifier(request.identifier());
        InMemoryAccount account = matches.stream()
                .filter(a -> isAdminRole(a.role()))
                .filter(a -> passwordEncoder.matches(request.password(), a.passwordHash()))
                .findFirst()
                .orElseThrow(() -> new AuthenticationFailedException("Invalid credentials"));
        return tokensFor(account);
    }

    public void forgotPassword(ForgotPasswordRequest request) {
        // Sprint 0 — acknowledge request; email/SMS reset flow comes with persisted users.
        if (findByIdentifier(request.identifier()).isEmpty()) {
            throw new AuthenticationFailedException("No account found for this identifier");
        }
    }

    public void requestOtp(String phoneE164) {
        String otp = appProperties.devMode() ? appProperties.mockOtpCode() : UUID.randomUUID().toString().substring(0, 6);
        smsSenderPort.sendOtp(normalizePhone(phoneE164), otp);
    }

    public boolean verifyOtp(OtpVerifyRequest request) {
        if (appProperties.devMode()) {
            return appProperties.mockOtpCode().equals(request.otpCode());
        }
        return false;
    }

    @PostConstruct
    void seedDevUsers() {
        if (!appProperties.devMode()) {
            return;
        }
        seedAccount(
                "customer@khadamati.local",
                "+96170000001",
                "Dev",
                "Customer",
                "password123",
                Role.CUSTOMER);
        seedAccount(
                "provider@khadamati.local",
                "+96170000002",
                "Dev",
                "Provider",
                "password123",
                Role.CRAFTSMAN);
        seedAccount(
                "store@khadamati.local",
                "+96170000003",
                "Dev",
                "Store",
                "password123",
                Role.STORE);
        seedAccount(
                "multi@khadamati.local",
                "+96170000099",
                "Dev",
                "Multi",
                "password123",
                Role.CUSTOMER);
        seedAccount(
                "multi@khadamati.local",
                "+96170000099",
                "Dev",
                "Multi",
                "password123",
                Role.CRAFTSMAN);
        seedAccount(
                "admin@khadamati.local",
                "+96170000000",
                "Dev",
                "Admin",
                "password123",
                Role.ADMIN);
    }

    private void seedAccount(
            String email, String phone, String firstName, String lastName, String password, Role role) {
        String normalizedEmail = normalizeEmail(email);
        String normalizedPhone = normalizePhone(phone);
        if (emailRoleIndex.containsKey(roleKey(normalizedEmail, role))) {
            return;
        }
        String id = UUID.randomUUID().toString();
        InMemoryAccount account = new InMemoryAccount(
                id,
                normalizedEmail,
                normalizedPhone,
                firstName,
                lastName,
                passwordEncoder.encode(password),
                role);
        storeAccount(account);
    }

    private List<InMemoryAccount> authenticatePublicAccounts(String identifier, String password) {
        List<InMemoryAccount> candidates = findByIdentifier(identifier);
        List<InMemoryAccount> matches = new ArrayList<>();
        for (InMemoryAccount account : candidates) {
            if (isAdminRole(account.role())) {
                continue;
            }
            if (passwordEncoder.matches(password, account.passwordHash())) {
                matches.add(account);
            }
        }
        return matches;
    }

    private boolean hasValidAdminCredentials(String identifier, String password) {
        return findByIdentifier(identifier).stream()
                .filter(a -> isAdminRole(a.role()))
                .anyMatch(a -> passwordEncoder.matches(password, a.passwordHash()));
    }

    private List<InMemoryAccount> findByIdentifier(String identifier) {
        String trimmed = identifier.trim();
        if (trimmed.contains("@")) {
            String email = normalizeEmail(trimmed);
            return accountsById.values().stream()
                    .filter(a -> a.email().equals(email))
                    .toList();
        }
        String phone = normalizePhone(trimmed);
        return accountsById.values().stream()
                .filter(a -> a.phoneE164().equals(phone))
                .toList();
    }

    private void ensureUniqueForRole(String email, String phone, Role role) {
        if (emailRoleIndex.containsKey(roleKey(email, role))) {
            throw new IllegalArgumentException("An account of this type already exists for this email");
        }
        if (phoneRoleIndex.containsKey(roleKey(phone, role))) {
            throw new IllegalArgumentException("An account of this type already exists for this phone number");
        }
    }

    private void storeAccount(InMemoryAccount account) {
        accountsById.put(account.id(), account);
        emailRoleIndex.put(roleKey(account.email(), account.role()), account.id());
        phoneRoleIndex.put(roleKey(account.phoneE164(), account.role()), account.id());
    }

    private AuthTokensResponse tokensFor(InMemoryAccount account) {
        return AuthTokensResponse.bearer(
                jwtService.createAccessToken(account.email(), account.role()),
                jwtService.createRefreshToken(account.email(), account.role()),
                account.role());
    }

    private static String roleKey(String key, Role role) {
        return key + "|" + role.name();
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

    private record InMemoryAccount(
            String id,
            String email,
            String phoneE164,
            String firstName,
            String lastName,
            String passwordHash,
            Role role) {}
}
