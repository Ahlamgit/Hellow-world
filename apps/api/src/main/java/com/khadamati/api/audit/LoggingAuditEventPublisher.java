package com.khadamati.api.audit;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

@Component
public class LoggingAuditEventPublisher implements AuditEventPublisher {

    private static final Logger auditLog = LoggerFactory.getLogger("AUDIT");

    @Override
    public void publish(AuditEvent event) {
        auditLog.info(
                "type={} actor={} resource={}/{} at={} meta={}",
                event.eventType(),
                event.actorId(),
                event.resourceType(),
                event.resourceId(),
                event.occurredAt(),
                event.metadata());
    }
}
