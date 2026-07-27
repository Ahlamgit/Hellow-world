package com.khadamati.api.legal;

import java.util.List;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;

import com.khadamati.api.user.UserEntity;

public interface UserLegalAcceptanceRepository extends JpaRepository<UserLegalAcceptanceEntity, UUID> {

    List<UserLegalAcceptanceEntity> findByUserOrderByAcceptedAtDesc(UserEntity user);
}
