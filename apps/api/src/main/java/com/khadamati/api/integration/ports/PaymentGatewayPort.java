package com.khadamati.api.integration.ports;

/**
 * Payment gateway port — Sprint 0 mock only. IXOPAY sandbox integration is post–Sprint 0 (BLOCKER-007).
 */
public interface PaymentGatewayPort {
    boolean isAvailable();
}
