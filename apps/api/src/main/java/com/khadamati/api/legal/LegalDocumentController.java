package com.khadamati.api.legal;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.khadamati.api.legal.dto.LegalDocumentResponse;

@RestController
@RequestMapping("/api/v1/legal")
public class LegalDocumentController {

    private final LegalDocumentService legalDocumentService;

    public LegalDocumentController(LegalDocumentService legalDocumentService) {
        this.legalDocumentService = legalDocumentService;
    }

    @GetMapping("/terms")
    public LegalDocumentResponse terms(@RequestParam(defaultValue = "en") String lang) {
        return legalDocumentService.getActiveDocument(DocumentType.TERMS, lang);
    }

    @GetMapping("/privacy")
    public LegalDocumentResponse privacy(@RequestParam(defaultValue = "en") String lang) {
        return legalDocumentService.getActiveDocument(DocumentType.PRIVACY, lang);
    }
}
