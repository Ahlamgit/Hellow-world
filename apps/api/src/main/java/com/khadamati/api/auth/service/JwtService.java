package com.khadamati.api.auth.service;

import java.time.Instant;
import java.util.Date;

import javax.crypto.SecretKey;

import org.springframework.stereotype.Service;

import com.khadamati.api.config.AppProperties;
import com.khadamati.api.rbac.Role;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.JwtException;
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

    public String createAccessToken(String subject, Role role, java.util.UUID userId) {
        return buildToken(subject, role, userId, accessTtlMinutes * 60_000L, "access");
    }

    public String createRefreshToken(String subject, Role role, java.util.UUID userId) {
        return buildToken(subject, role, userId, refreshTtlDays * 86_400_000L, "refresh");
    }

    /** @deprecated use overload with userId */
    public String createAccessToken(String subject, Role role) {
        return buildToken(subject, role, null, accessTtlMinutes * 60_000L, "access");
    }

    /** @deprecated use overload with userId */
    public String createRefreshToken(String subject, Role role) {
        return buildToken(subject, role, null, refreshTtlDays * 86_400_000L, "refresh");
    }

    public Claims parseAccessToken(String token) {
        Claims claims = Jwts.parser()
                .verifyWith(secretKey)
                .build()
                .parseSignedClaims(token)
                .getPayload();
        if (!"access".equals(claims.get("type", String.class))) {
            throw new JwtException("Invalid token type");
        }
        return claims;
    }

    public Role roleFromClaims(Claims claims) {
        return Role.valueOf(claims.get("role", String.class));
    }

    private String buildToken(String subject, Role role, java.util.UUID userId, long ttlMillis, String tokenType) {
        Instant now = Instant.now();
        var claims = new java.util.HashMap<String, Object>();
        claims.put("role", role.name());
        claims.put("type", tokenType);
        if (userId != null) {
            claims.put("userId", userId.toString());
        }
        return Jwts.builder()
                .subject(subject)
                .claims(claims)
                .issuedAt(Date.from(now))
                .expiration(Date.from(now.plusMillis(ttlMillis)))
                .signWith(secretKey)
                .compact();
    }
}
