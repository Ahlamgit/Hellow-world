package com.khadamati.api.auth;

import com.khadamati.api.rbac.Role;

public class AccountPendingVerificationException extends RuntimeException {

    private final String email;
    private final Role role;
    private final String phoneE164;

    public AccountPendingVerificationException(String email, Role role, String phoneE164) {
        super("Your account needs verification.");
        this.email = email;
        this.role = role;
        this.phoneE164 = phoneE164;
    }

    public String email() {
        return email;
    }

    public Role role() {
        return role;
    }

    public String phoneE164() {
        return phoneE164;
    }
}
