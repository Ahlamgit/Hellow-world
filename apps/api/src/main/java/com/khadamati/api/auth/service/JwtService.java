package com.khadamati.api.auth.service;

import java.time.Instant;
import java.util.Date;
import java.util.Map;

import javax.crypto.SecretKey;

import org.springframework.stereotype.Service;

import com.khadamati.api.config.AppProperties;
import com.khadamati.api.rbac.Role;

import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;

@Service
public class JwtService {

    private final SecretKey secretKey;
    private final long accessTtlMinutes;
    private final long refreshTtlDays;

    public JwtService(AppProperties appProperties) {
        this.secretKey = Keys.hmacShaKeyFor(appProperties.jwt().secret().getBytes());
        this.accessTtlMinutes = appProperties.jwt().accessTtlMinutes();
        this.refreshTtlDays = appProperties.jwt().refreshTtlDays();
    }

    public String createAccessToken(String subject, Role role) {
        return buildToken(subject, role, accessTtlMinutes * 60_000L, "access");
    }

    public String createRefreshToken(String subject, Role role) {
        return buildToken(subject, role, refreshTtlDays * 86_400_000L, "refresh");
    }

    private String buildToken(String subject, Role role, long ttlMillis, String tokenType) {
        Instant now = Instant.now();
        return Jwts.builder()
                .subject(subject)
                .claims(Map.of("role", role.name(), "type", tokenType))
                .issuedAt(Date.from(now))
                .expiration(Date.from(now.plusMillis(ttlMillis)))
                .signWith(secretKey)
                .compact();
    }
}
