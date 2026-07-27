package com.khadamati.api.auth;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.khadamati.api.auth.dto.AuthTokensResponse;
import com.khadamati.api.auth.dto.ForgotPasswordRequest;
import com.khadamati.api.auth.dto.LoginCompleteRequest;
import com.khadamati.api.auth.dto.LoginRequest;
import com.khadamati.api.auth.dto.LoginResponse;
import com.khadamati.api.auth.dto.OtpRequest;
import com.khadamati.api.auth.dto.OtpVerifyRequest;
import com.khadamati.api.auth.dto.RegisterPendingResponse;
import com.khadamati.api.auth.dto.RegisterRequest;
import com.khadamati.api.auth.dto.RegisterVerifyOtpRequest;
import com.khadamati.api.auth.service.AuthService;

import jakarta.validation.Valid;

@RestController
@RequestMapping("/api/v1/auth")
public class AuthController {

    private final AuthService authService;

    public AuthController(AuthService authService) {
        this.authService = authService;
    }

    @PostMapping("/register")
    public ResponseEntity<RegisterPendingResponse> register(@Valid @RequestBody RegisterRequest request) {
        if (!request.passwordsMatch()) {
            throw new IllegalArgumentException("Password and confirm password must match");
        }
        return ResponseEntity.ok(authService.register(request));
    }

    @PostMapping("/register/verify-otp")
    public ResponseEntity<AuthTokensResponse> verifyRegistrationOtp(@Valid @RequestBody RegisterVerifyOtpRequest request) {
        return ResponseEntity.ok(authService.verifyRegistrationOtp(request));
    }

    @PostMapping("/login")
    public ResponseEntity<LoginResponse> login(@Valid @RequestBody LoginRequest request) {
        return ResponseEntity.ok(authService.login(request));
    }

    @PostMapping("/login/complete")
    public ResponseEntity<AuthTokensResponse> completeLogin(@Valid @RequestBody LoginCompleteRequest request) {
        return ResponseEntity.ok(authService.completeLogin(request));
    }

    @PostMapping("/admin/login")
    public ResponseEntity<AuthTokensResponse> adminLogin(@Valid @RequestBody LoginRequest request) {
        return ResponseEntity.ok(authService.adminLogin(request));
    }

    @PostMapping("/password/forgot")
    public ResponseEntity<Void> forgotPassword(@Valid @RequestBody ForgotPasswordRequest request) {
        authService.forgotPassword(request);
        return ResponseEntity.accepted().build();
    }

    @PostMapping("/otp/request")
    public ResponseEntity<Void> requestOtp(@Valid @RequestBody OtpRequest request) {
        authService.requestOtp(request.phoneE164());
        return ResponseEntity.accepted().build();
    }

    @PostMapping("/otp/verify")
    public ResponseEntity<MapResponse> verifyOtp(@Valid @RequestBody OtpVerifyRequest request) {
        boolean valid = authService.verifyOtp(request);
        return ResponseEntity.ok(new MapResponse("valid", valid));
    }

    public record MapResponse(String key, boolean value) {}
}
