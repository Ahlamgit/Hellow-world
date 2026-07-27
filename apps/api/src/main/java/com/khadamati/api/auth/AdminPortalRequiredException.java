package com.khadamati.api.auth;

public class AdminPortalRequiredException extends RuntimeException {

    public AdminPortalRequiredException() {
        super("Admin accounts must sign in at the admin portal");
    }
}
