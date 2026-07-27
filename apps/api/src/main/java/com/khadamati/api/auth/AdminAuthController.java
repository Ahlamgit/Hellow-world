package com.khadamati.api.auth;

import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.khadamati.api.auth.dto.AdminLoginPendingResponse;
import com.khadamati.api.auth.dto.AdminSessionResponse;
import com.khadamati.api.auth.dto.AdminVerifyMfaRequest;
import com.khadamati.api.auth.dto.AuthTokensResponse;
import com.khadamati.api.auth.dto.LoginRequest;
import com.khadamati.api.auth.service.AdminAuthService;
import com.khadamati.api.auth.service.JwtService;

import io.jsonwebtoken.Claims;
import jakarta.validation.Valid;

@RestController
@RequestMapping("/api/admin")
public class AdminAuthController {

    private final AdminAuthService adminAuthService;
    private final JwtService jwtService;

    public AdminAuthController(AdminAuthService adminAuthService, JwtService jwtService) {
        this.adminAuthService = adminAuthService;
        this.jwtService = jwtService;
    }

    @PostMapping("/login")
    public ResponseEntity<AdminLoginPendingResponse> login(@Valid @RequestBody LoginRequest request) {
        return ResponseEntity.ok(adminAuthService.login(request));
    }

    @PostMapping("/verify-mfa")
    public ResponseEntity<AuthTokensResponse> verifyMfa(@Valid @RequestBody AdminVerifyMfaRequest request) {
        return ResponseEntity.ok(adminAuthService.verifyMfa(request));
    }

    @GetMapping("/session")
    public AdminSessionResponse session(@RequestHeader(HttpHeaders.AUTHORIZATION) String authorizationHeader) {
        Claims claims = jwtService.parseAccessToken(extractBearerToken(authorizationHeader));
        return adminAuthService.getSession(claims);
    }

    private static String extractBearerToken(String authorizationHeader) {
        if (authorizationHeader != null && authorizationHeader.startsWith("Bearer ")) {
            return authorizationHeader.substring(7);
        }
        throw new IllegalArgumentException("Missing bearer token");
    }
}
