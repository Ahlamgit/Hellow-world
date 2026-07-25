package com.khadamati.api.integration.ports;

import java.io.InputStream;

public interface StoragePort {
    String store(String relativePath, InputStream content, String contentType, long sizeBytes);

    InputStream retrieve(String relativePath);
}
