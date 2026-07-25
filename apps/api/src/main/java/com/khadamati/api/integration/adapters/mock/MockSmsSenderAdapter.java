package com.khadamati.api.integration.adapters.mock;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Component;

import com.khadamati.api.config.AppProperties;
import com.khadamati.api.integration.ports.SmsSenderPort;

@Component
@ConditionalOnProperty(name = "khadamati.dev-mode", havingValue = "true", matchIfMissing = true)
public class MockSmsSenderAdapter implements SmsSenderPort {

    private static final Logger log = LoggerFactory.getLogger(MockSmsSenderAdapter.class);

    private final AppProperties appProperties;

    public MockSmsSenderAdapter(AppProperties appProperties) {
        this.appProperties = appProperties;
    }

    @Override
    public void sendOtp(String phoneE164, String otpCode) {
        String code = appProperties.devMode() ? appProperties.mockOtpCode() : otpCode;
        log.info("[DEV_MODE SMS] OTP for {}: {}", phoneE164, code);
    }
}
