package com.khadamati.api.legal;

import java.util.List;

import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.khadamati.api.legal.dto.AcceptLegalDocumentsRequest;
import com.khadamati.api.legal.dto.LegalComplianceResponse;
import com.khadamati.api.legal.dto.LegalDocumentResponse;
import com.khadamati.api.legal.dto.PublishLegalDocumentRequest;
import com.khadamati.api.rbac.Role;
import com.khadamati.api.rbac.RolesAllowed;
import com.khadamati.api.user.UserEntity;
import com.khadamati.api.legal.dto.AcceptLegalDocumentsRequest;
import com.khadamati.api.user.UserEntity;
import com.khadamati.api.user.UserRepository;

import jakarta.validation.Valid;

@RestController
@RequestMapping("/api/v1")
public class LegalComplianceController {

    private final LegalDocumentService legalDocumentService;
    private final UserRepository userRepository;

    public LegalComplianceController(LegalDocumentService legalDocumentService, UserRepository userRepository) {
        this.legalDocumentService = legalDocumentService;
        this.userRepository = userRepository;
    }

    @GetMapping("/legal/compliance")
    public LegalComplianceResponse compliance(Authentication authentication, @RequestParam(defaultValue = "en") String lang) {
        UserEntity user = requireAuthenticatedUser(authentication);
        return legalDocumentService.checkCompliance(user, lang);
    }

    @PostMapping("/legal/accept")
    public LegalComplianceResponse accept(
            Authentication authentication,
            @Valid @RequestBody AcceptLegalDocumentsRequest request) {
        UserEntity user = requireAuthenticatedUser(authentication);
        return legalDocumentService.acceptDocuments(
                user, request.termsVersion(), request.privacyVersion(), request.language());
    }

    @GetMapping("/admin/legal/documents")
    @RolesAllowed(Role.ADMIN)
    public List<LegalDocumentResponse> listDocuments() {
        return legalDocumentService.listAllDocuments();
    }

    @PostMapping("/admin/legal/documents")
    @RolesAllowed(Role.ADMIN)
    public ResponseEntity<LegalDocumentResponse> publish(@Valid @RequestBody PublishLegalDocumentRequest request) {
        return ResponseEntity.ok(legalDocumentService.publishDocument(request));
    }

    private UserEntity requireAuthenticatedUser(Authentication authentication) {
        String email = authentication.getName();
        Role role = authentication.getAuthorities().stream()
                .map(GrantedAuthority::getAuthority)
                .filter(a -> a.startsWith("ROLE_"))
                .map(a -> a.substring("ROLE_".length()))
                .map(Role::valueOf)
                .findFirst()
                .orElseThrow(() -> new IllegalArgumentException("Missing role"));
        return userRepository.findByEmailAndRole(email, role)
                .orElseThrow(() -> new IllegalArgumentException("User not found"));
    }
}
