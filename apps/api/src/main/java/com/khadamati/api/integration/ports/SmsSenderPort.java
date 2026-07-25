package com.khadamati.api.integration.ports;

public interface SmsSenderPort {
    void sendOtp(String phoneE164, String otpCode);
}
