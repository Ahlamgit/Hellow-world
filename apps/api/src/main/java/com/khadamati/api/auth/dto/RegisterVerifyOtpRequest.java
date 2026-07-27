package com.khadamati.api.auth.dto;

import com.khadamati.api.rbac.Role;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

public record RegisterVerifyOtpRequest(
        @NotBlank @Email String email,
        @NotNull Role role,
        @NotBlank String otpCode
) {}
