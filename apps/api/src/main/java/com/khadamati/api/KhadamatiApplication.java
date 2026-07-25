package com.khadamati.api;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.properties.EnableConfigurationProperties;

import com.khadamati.api.config.AppProperties;
import com.khadamati.api.config.MarketProperties;

@SpringBootApplication
@EnableConfigurationProperties({AppProperties.class, MarketProperties.class})
public class KhadamatiApplication {

    public static void main(String[] args) {
        SpringApplication.run(KhadamatiApplication.class, args);
    }
}
