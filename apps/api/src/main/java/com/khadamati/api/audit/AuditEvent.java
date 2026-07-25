package com.khadamati.api.audit;

import java.time.Instant;
import java.util.Map;

public record AuditEvent(
        String eventType,
        String actorId,
        String resourceType,
        String resourceId,
        Map<String, Object> metadata,
        Instant occurredAt
) {}
