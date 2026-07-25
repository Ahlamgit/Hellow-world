package com.khadamati.api.audit;

public interface AuditEventPublisher {
    void publish(AuditEvent event);
}
