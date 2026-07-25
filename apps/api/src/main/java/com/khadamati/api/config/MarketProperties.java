package com.khadamati.api.config;

import org.springframework.boot.context.properties.ConfigurationProperties;

@ConfigurationProperties(prefix = "khadamati.market")
public record MarketProperties(
        String code,
        String currency,
        String defaultLocale,
        String timezone,
        String phoneCountryCode
) {}
