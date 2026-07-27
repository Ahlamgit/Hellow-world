package com.khadamati.api.auth.dto;

import com.khadamati.api.rbac.Role;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;

public record LoginCompleteRequest(
        @NotBlank String identifier,
        @NotBlank String password,
        @NotNull Role role
) {}
