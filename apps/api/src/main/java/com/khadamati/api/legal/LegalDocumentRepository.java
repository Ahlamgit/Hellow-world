package com.khadamati.api.legal;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;

public interface LegalDocumentRepository extends JpaRepository<LegalDocumentEntity, UUID> {

    Optional<LegalDocumentEntity> findByDocumentTypeAndLanguageAndActiveTrue(DocumentType documentType, String language);

    Optional<LegalDocumentEntity> findByDocumentTypeAndVersionAndLanguage(
            DocumentType documentType, String version, String language);

    List<LegalDocumentEntity> findByDocumentTypeAndLanguageOrderByCreatedAtDesc(
            DocumentType documentType, String language);
}
