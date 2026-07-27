package com.khadamati.api.legal;

import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.khadamati.api.legal.dto.LegalComplianceResponse;
import com.khadamati.api.legal.dto.LegalComplianceResponse.PendingDocument;
import com.khadamati.api.legal.dto.LegalDocumentResponse;
import com.khadamati.api.legal.dto.PublishLegalDocumentRequest;
import com.khadamati.api.user.UserEntity;

@Service
public class LegalDocumentService {

    private final LegalDocumentRepository legalDocumentRepository;
    private final UserLegalAcceptanceRepository acceptanceRepository;

    public LegalDocumentService(
            LegalDocumentRepository legalDocumentRepository,
            UserLegalAcceptanceRepository acceptanceRepository) {
        this.legalDocumentRepository = legalDocumentRepository;
        this.acceptanceRepository = acceptanceRepository;
    }

    public LegalDocumentResponse getActiveDocument(DocumentType type, String language) {
        return toResponse(requireActive(type, normalizeLanguage(language)));
    }

    public List<LegalDocumentResponse> listAllDocuments() {
        return legalDocumentRepository.findAll().stream().map(this::toResponse).toList();
    }

    @Transactional
    public LegalDocumentResponse publishDocument(PublishLegalDocumentRequest request) {
        String language = normalizeLanguage(request.language());
        legalDocumentRepository.findByDocumentTypeAndLanguageAndActiveTrue(request.documentType(), language)
                .ifPresent(existing -> {
                    existing.setActive(false);
                    existing.setUpdatedAt(Instant.now());
                    legalDocumentRepository.save(existing);
                });

        LegalDocumentEntity entity = new LegalDocumentEntity();
        entity.setId(UUID.randomUUID());
        entity.setDocumentType(request.documentType());
        entity.setVersion(request.version().trim());
        entity.setTitle(request.title().trim());
        entity.setContent(request.content());
        entity.setLanguage(language);
        entity.setActive(true);
        Instant now = Instant.now();
        entity.setCreatedAt(now);
        entity.setUpdatedAt(now);
        return toResponse(legalDocumentRepository.save(entity));
    }

    public void validateRegistrationVersions(String termsVersion, String privacyVersion, String language) {
        requireMatchingVersion(DocumentType.TERMS, termsVersion, language);
        requireMatchingVersion(DocumentType.PRIVACY, privacyVersion, language);
    }

    @Transactional
    public void recordAcceptances(
            UserEntity user, String termsVersion, String privacyVersion, String language) {
        String lang = normalizeLanguage(language);
        LegalDocumentEntity terms = requireMatchingVersion(DocumentType.TERMS, termsVersion, lang);
        LegalDocumentEntity privacy = requireMatchingVersion(DocumentType.PRIVACY, privacyVersion, lang);
        saveAcceptance(user, terms);
        saveAcceptance(user, privacy);
    }

    @Transactional
    public LegalComplianceResponse acceptDocuments(
            UserEntity user, String termsVersion, String privacyVersion, String language) {
        recordAcceptances(user, termsVersion, privacyVersion, language);
        return checkCompliance(user, language);
    }

    public LegalComplianceResponse checkCompliance(UserEntity user, String language) {
        String lang = normalizeLanguage(language);
        List<PendingDocument> pending = new ArrayList<>();

        for (DocumentType type : DocumentType.values()) {
            LegalDocumentEntity active = requireActive(type, lang);
            String acceptedVersion = acceptanceRepository.findByUserOrderByAcceptedAtDesc(user).stream()
                    .filter(a -> a.getDocumentType() == type)
                    .findFirst()
                    .map(UserLegalAcceptanceEntity::getVersionAccepted)
                    .orElse(null);
            if (acceptedVersion == null || !acceptedVersion.equals(active.getVersion())) {
                pending.add(new PendingDocument(type, active.getVersion(), active.getTitle()));
            }
        }

        return new LegalComplianceResponse(pending.isEmpty(), pending);
    }

    private LegalDocumentEntity requireActive(DocumentType type, String language) {
        return legalDocumentRepository.findByDocumentTypeAndLanguageAndActiveTrue(type, language)
                .orElseThrow(() -> new IllegalStateException("No active legal document for " + type + " (" + language + ")"));
    }

    private LegalDocumentEntity requireMatchingVersion(DocumentType type, String version, String language) {
        String lang = normalizeLanguage(language);
        LegalDocumentEntity active = requireActive(type, lang);
        if (!active.getVersion().equals(version.trim())) {
            throw new IllegalArgumentException("Legal document version mismatch for " + type.name());
        }
        return active;
    }

    private void saveAcceptance(UserEntity user, LegalDocumentEntity document) {
        UserLegalAcceptanceEntity acceptance = new UserLegalAcceptanceEntity();
        acceptance.setId(UUID.randomUUID());
        acceptance.setUser(user);
        acceptance.setDocument(document);
        acceptance.setDocumentType(document.getDocumentType());
        acceptance.setVersionAccepted(document.getVersion());
        acceptance.setAcceptedAt(Instant.now());
        acceptanceRepository.save(acceptance);
    }

    private LegalDocumentResponse toResponse(LegalDocumentEntity entity) {
        return new LegalDocumentResponse(
                entity.getId(),
                entity.getDocumentType(),
                entity.getVersion(),
                entity.getTitle(),
                entity.getContent(),
                entity.getLanguage(),
                entity.isActive(),
                entity.getUpdatedAt());
    }

    private static String normalizeLanguage(String language) {
        if (language == null || language.isBlank()) {
            return "en";
        }
        String normalized = language.trim().toLowerCase();
        return normalized.startsWith("ar") ? "ar" : "en";
    }
}
