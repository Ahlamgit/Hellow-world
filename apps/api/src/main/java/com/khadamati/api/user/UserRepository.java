package com.khadamati.api.user;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;

import com.khadamati.api.rbac.Role;

public interface UserRepository extends JpaRepository<UserEntity, UUID> {

    Optional<UserEntity> findByEmailAndRole(String email, Role role);

    List<UserEntity> findByEmail(String email);

    List<UserEntity> findByPhoneE164(String phoneE164);
}
