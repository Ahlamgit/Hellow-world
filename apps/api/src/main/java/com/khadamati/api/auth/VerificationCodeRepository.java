package com.khadamati.api.auth;

import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;

public interface VerificationCodeRepository extends JpaRepository<VerificationCodeEntity, UUID> {

    Optional<VerificationCodeEntity> findFirstByPhoneE164AndPurposeAndConsumedAtIsNullOrderByCreatedAtDesc(
            String phoneE164, VerificationPurpose purpose);
}
