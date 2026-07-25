package com.khadamati.api.integration.adapters.local;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardCopyOption;

import org.springframework.stereotype.Component;

import com.khadamati.api.config.AppProperties;
import com.khadamati.api.integration.ports.StoragePort;

@Component
public class LocalFileStorageAdapter implements StoragePort {

    private final Path root;

    public LocalFileStorageAdapter(AppProperties appProperties) {
        this.root = Path.of(appProperties.storage().path()).toAbsolutePath().normalize();
    }

    @Override
    public String store(String relativePath, InputStream content, String contentType, long sizeBytes) {
        try {
            Path target = resolve(relativePath);
            Files.createDirectories(target.getParent());
            Files.copy(content, target, StandardCopyOption.REPLACE_EXISTING);
            return relativePath;
        } catch (IOException ex) {
            throw new IllegalStateException("Failed to store file: " + relativePath, ex);
        }
    }

    @Override
    public InputStream retrieve(String relativePath) {
        try {
            return Files.newInputStream(resolve(relativePath));
        } catch (IOException ex) {
            throw new IllegalStateException("Failed to read file: " + relativePath, ex);
        }
    }

    private Path resolve(String relativePath) {
        Path resolved = root.resolve(relativePath).normalize();
        if (!resolved.startsWith(root)) {
            throw new IllegalArgumentException("Invalid storage path");
        }
        return resolved;
    }
}
