package com.khadamati.api.market;

import java.util.Map;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.khadamati.api.config.MarketProperties;

@RestController
@RequestMapping("/api/v1/market")
public class MarketController {

    private final MarketProperties marketProperties;

    public MarketController(MarketProperties marketProperties) {
        this.marketProperties = marketProperties;
    }

    @GetMapping("/config")
    public Map<String, String> config() {
        return Map.of(
                "code", marketProperties.code(),
                "currency", marketProperties.currency(),
                "defaultLocale", marketProperties.defaultLocale(),
                "timezone", marketProperties.timezone(),
                "phoneCountryCode", marketProperties.phoneCountryCode());
    }
}
