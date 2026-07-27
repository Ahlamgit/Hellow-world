package com.khadamati.api.auth.dto;

import com.khadamati.api.rbac.Role;

import jakarta.validation.constraints.AssertTrue;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;

public record RegisterRequest(
        @NotBlank @Size(max = 80) String firstName,
        @NotBlank @Size(max = 80) String lastName,
        @NotBlank
        @Pattern(regexp = "^\\+[1-9]\\d{6,14}$", message = "Phone must be E.164 format")
        String phoneE164,
        @NotBlank @Email String email,
        @NotBlank @Size(min = 8, max = 128) String password,
        @NotBlank String confirmPassword,
        @NotNull Role role,
        @AssertTrue(message = "Terms must be accepted") boolean acceptTerms,
        @AssertTrue(message = "Privacy policy must be accepted") boolean acceptPrivacy,
        @NotBlank String termsVersion,
        @NotBlank String privacyVersion,
        @NotBlank String language
) {
    public boolean passwordsMatch() {
        return password.equals(confirmPassword);
    }
}
