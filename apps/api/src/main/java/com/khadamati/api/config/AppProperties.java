package com.khadamati.api.config;

import org.springframework.boot.context.properties.ConfigurationProperties;

@ConfigurationProperties(prefix = "khadamati")
public record AppProperties(
        boolean devMode,
        String mockOtpCode,
        Jwt jwt,
        Storage storage
) {
    public record Jwt(String secret, int accessTtlMinutes, int refreshTtlDays) {}
    public record Storage(String path) {}
}
