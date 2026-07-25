package com.khadamati.api.audit;

import java.time.Instant;
import java.util.Map;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.khadamati.api.rbac.Role;
import com.khadamati.api.rbac.RolesAllowed;

@RestController
@RequestMapping("/api/v1/admin/audit")
@RolesAllowed(Role.ADMIN)
public class AdminAuditController {

    private final AuditEventPublisher auditEventPublisher;

    public AdminAuditController(AuditEventPublisher auditEventPublisher) {
        this.auditEventPublisher = auditEventPublisher;
    }

    @PostMapping("/test")
    public ResponseEntity<Map<String, String>> emitTestEvent() {
        auditEventPublisher.publish(new AuditEvent(
                "ADMIN_TEST_EVENT",
                "system",
                "audit",
                "test",
                Map.of("source", "sprint-0"),
                Instant.now()));
        return ResponseEntity.ok(Map.of("status", "emitted"));
    }
}
