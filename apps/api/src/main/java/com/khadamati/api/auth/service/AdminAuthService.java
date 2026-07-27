package com.khadamati.api.auth.service;

import java.util.Locale;

import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import com.khadamati.api.auth.AuthenticationFailedException;
import com.khadamati.api.auth.VerificationPurpose;
import com.khadamati.api.auth.dto.AdminLoginPendingResponse;
import com.khadamati.api.auth.dto.AdminSessionResponse;
import com.khadamati.api.auth.dto.AdminVerifyMfaRequest;
import com.khadamati.api.auth.dto.AuthTokensResponse;
import com.khadamati.api.auth.dto.LoginRequest;
import com.khadamati.api.rbac.Role;
import com.khadamati.api.user.AccountStatus;
import com.khadamati.api.user.UserEntity;
import com.khadamati.api.user.UserRepository;

import io.jsonwebtoken.Claims;

@Service
public class AdminAuthService {

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtService jwtService;
    private final VerificationCodeService verificationCodeService;

    public AdminAuthService(
            UserRepository userRepository,
            PasswordEncoder passwordEncoder,
            JwtService jwtService,
            VerificationCodeService verificationCodeService) {
        this.userRepository = userRepository;
        this.passwordEncoder = passwordEncoder;
        this.jwtService = jwtService;
        this.verificationCodeService = verificationCodeService;
    }

    public AdminLoginPendingResponse login(LoginRequest request) {
        UserEntity admin = userRepository.findByEmail(normalizeEmail(request.identifier())).stream()
                .filter(user -> user.getRole() == Role.ADMIN)
                .filter(user -> passwordEncoder.matches(request.password(), user.getPasswordHash()))
                .findFirst()
                .orElseThrow(() -> new AuthenticationFailedException("Invalid credentials"));

        ensureActiveAdmin(admin);

        verificationCodeService.sendCode(admin.getPhoneE164(), VerificationPurpose.ADMIN_MFA);

        return AdminLoginPendingResponse.mfaRequired(admin.getEmail(), admin.getPhoneE164());
    }

    public AuthTokensResponse verifyMfa(AdminVerifyMfaRequest request) {
        String email = normalizeEmail(request.email());
        UserEntity admin = userRepository.findByEmailAndRole(email, Role.ADMIN)
                .orElseThrow(() -> new AuthenticationFailedException("Invalid credentials"));

        ensureActiveAdmin(admin);

        verificationCodeService.verifyCode(admin.getPhoneE164(), VerificationPurpose.ADMIN_MFA, request.otpCode());

        return AuthTokensResponse.bearer(
                jwtService.createAccessToken(admin.getEmail(), admin.getRole(), admin.getId(), true),
                jwtService.createRefreshToken(admin.getEmail(), admin.getRole(), admin.getId(), true),
                admin.getRole());
    }

    public AdminSessionResponse getSession(Claims claims) {
        Role role = jwtService.roleFromClaims(claims);
        if (role != Role.ADMIN) {
            throw new AuthenticationFailedException("Authentication required.");
        }
        if (!jwtService.isMfaVerified(claims)) {
            throw new AuthenticationFailedException("MFA verification required.");
        }

        UserEntity admin = userRepository.findByEmailAndRole(claims.getSubject(), Role.ADMIN)
                .orElseThrow(() -> new AuthenticationFailedException("Authentication required."));

        ensureActiveAdmin(admin);

        return new AdminSessionResponse(
                admin.getEmail(),
                admin.getRole(),
                admin.getAccountStatus(),
                admin.getFirstName(),
                admin.getLastName(),
                admin.getPhoneE164(),
                true);
    }

    private void ensureActiveAdmin(UserEntity admin) {
        if (admin.getAccountStatus() != AccountStatus.ACTIVE || !admin.isActive()) {
            throw new AuthenticationFailedException("Authentication required.");
        }
    }

    private static String normalizeEmail(String email) {
        return email.trim().toLowerCase(Locale.ROOT);
    }
}
