package com.khadamati.api.config;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

import org.springframework.boot.ApplicationRunner;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class IntegrationConfig {

    @Bean
    ApplicationRunner ensureStorageDirectory(AppProperties appProperties) {
        return args -> {
            Path storagePath = Path.of(appProperties.storage().path()).toAbsolutePath().normalize();
            Files.createDirectories(storagePath);
        };
    }
}
