package com.khadamati.api.user;

import java.time.Instant;
import java.util.UUID;

import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;

import com.khadamati.api.config.AppProperties;
import com.khadamati.api.legal.LegalDocumentService;
import com.khadamati.api.rbac.Role;

@Component
public class UserDevSeeder implements ApplicationRunner {

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    private final AppProperties appProperties;
    private final LegalDocumentService legalDocumentService;

    public UserDevSeeder(
            UserRepository userRepository,
            PasswordEncoder passwordEncoder,
            AppProperties appProperties,
            LegalDocumentService legalDocumentService) {
        this.userRepository = userRepository;
        this.passwordEncoder = passwordEncoder;
        this.appProperties = appProperties;
        this.legalDocumentService = legalDocumentService;
    }

    @Override
    public void run(ApplicationArguments args) {
        if (!appProperties.devMode()) {
            return;
        }
        seed("customer@khadamati.local", "+96170000001", Role.CUSTOMER);
        seed("provider@khadamati.local", "+96170000002", Role.CRAFTSMAN);
        seed("store@khadamati.local", "+96170000003", Role.STORE);
        seed("multi@khadamati.local", "+96170000099", Role.CUSTOMER);
        seed("multi@khadamati.local", "+96170000099", Role.CRAFTSMAN);
        seed("admin@khadamati.local", "+96170000000", Role.ADMIN);
    }

    private void seed(String email, String phone, Role role) {
        if (userRepository.findByEmailAndRole(email, role).isPresent()) {
            return;
        }
        Instant now = Instant.now();
        UserEntity user = new UserEntity();
        user.setId(UUID.randomUUID());
        user.setEmail(email);
        user.setPhoneE164(phone);
        user.setRole(role);
        user.setFirstName("Dev");
        user.setLastName(role.name());
        user.setPasswordHash(passwordEncoder.encode("password123"));
        user.setPhoneVerified(true);
        user.setActive(true);
        user.setCreatedAt(now);
        user.setUpdatedAt(now);
        userRepository.save(user);

        if (!isAdminRole(role)) {
            legalDocumentService.recordAcceptances(user, "1.0", "1.0", "en");
        }
    }

    private static boolean isAdminRole(Role role) {
        return role == Role.ADMIN || role == Role.FINANCE_ADMIN || role == Role.SUPER_ADMIN;
    }
}
