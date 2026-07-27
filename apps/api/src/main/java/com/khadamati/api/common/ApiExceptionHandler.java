package com.khadamati.api.common;

import com.khadamati.api.auth.AccountPendingVerificationException;
import com.khadamati.api.auth.AdminPortalRequiredException;
import com.khadamati.api.auth.AuthenticationFailedException;
import com.khadamati.api.auth.VerificationAttemptsExceededException;
import com.khadamati.api.auth.VerificationCodeExpiredException;

import java.util.Map;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

@RestControllerAdvice
public class ApiExceptionHandler {

    @ExceptionHandler(AdminPortalRequiredException.class)
    public ResponseEntity<Map<String, String>> handleAdminPortalRequired(AdminPortalRequiredException ex) {
        return ResponseEntity.status(HttpStatus.FORBIDDEN).body(Map.of("error", "ADMIN_PORTAL_REQUIRED"));
    }

    @ExceptionHandler(AuthenticationFailedException.class)
    public ResponseEntity<Map<String, String>> handleAuthenticationFailed(AuthenticationFailedException ex) {
        return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(Map.of("error", ex.getMessage()));
    }

    @ExceptionHandler(AccountPendingVerificationException.class)
    public ResponseEntity<Map<String, Object>> handlePendingVerification(AccountPendingVerificationException ex) {
        return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(Map.of(
                "error", ex.getMessage(),
                "code", "ACCOUNT_PENDING_VERIFICATION",
                "email", ex.email(),
                "role", ex.role().name(),
                "phoneE164", ex.phoneE164()));
    }

    @ExceptionHandler(VerificationCodeExpiredException.class)
    public ResponseEntity<Map<String, String>> handleCodeExpired(VerificationCodeExpiredException ex) {
        return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(Map.of(
                "error", ex.getMessage(),
                "code", "VERIFICATION_CODE_EXPIRED"));
    }

    @ExceptionHandler(VerificationAttemptsExceededException.class)
    public ResponseEntity<Map<String, String>> handleAttemptsExceeded(VerificationAttemptsExceededException ex) {
        return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(Map.of(
                "error", ex.getMessage(),
                "code", "VERIFICATION_ATTEMPTS_EXCEEDED"));
    }

    @ExceptionHandler(IllegalArgumentException.class)
    public ResponseEntity<Map<String, String>> handleIllegalArgument(IllegalArgumentException ex) {
        return ResponseEntity.badRequest().body(Map.of("error", ex.getMessage()));
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<Map<String, String>> handleValidation(MethodArgumentNotValidException ex) {
        String message = ex.getBindingResult().getFieldErrors().stream()
                .findFirst()
                .map(error -> error.getField() + ": " + error.getDefaultMessage())
                .orElse("Validation failed");
        return ResponseEntity.badRequest().body(Map.of("error", message));
    }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<Map<String, String>> handleGeneric(Exception ex) {
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "Internal server error"));
    }
}
