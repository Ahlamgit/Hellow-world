package com.khadamati.api.integration.adapters.mock;

import org.springframework.stereotype.Component;

import com.khadamati.api.integration.ports.PaymentGatewayPort;

@Component
public class MockPaymentGatewayAdapter implements PaymentGatewayPort {

    @Override
    public boolean isAvailable() {
        return false;
    }
}
