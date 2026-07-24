# 19. Naming Conventions

**Document ID:** KHAD-V1-NAMES  
**Status:** Draft for Approval  

---

## 19.1 General

| Asset | Convention | Example |
|-------|------------|---------|
| Repo folders | kebab-case | `admin-portal` |
| Java packages | reverse-DNS + module | `com.khadamati.booking` |
| Java classes | UpperCamelCase | `BookingService` |
| Java methods/fields | lowerCamelCase | `createBooking` |
| DB tables | snake_case plural | `booking_status_history` |
| DB columns | snake_case | `scheduled_start` |
| DB enums | UPPER_SNAKE stored as varchar/enum | `PENDING_PAYMENT` |
| REST paths | kebab-case plural resources | `/booking-reminders` (prefer nouns) |
| JSON fields | camelCase | `scheduledStart` |
| Permissions | `resource:action` | `onboarding:approve` |
| Config keys | kebab or relaxed bind | `khadamati.payment.ixopay.api-url` |
| Flyway files | `VyyyyMMddHHmm__description.sql` | `V202607241200__booking_init.sql` |
| React components | UpperCamelCase | `SettlementOverviewPage` |
| React files | match component or kebab feature | `settlement-overview-page.tsx` |
| Dart files | snake_case | `booking_repository.dart` |
| Dart classes | UpperCamelCase | `BookingRepository` |
| Feature flags | dot or kebab | `booking.reminders.enabled` |
| Domain events | past tense | `BookingCompleted` |
| Outbox payload type | same as event | `booking.completed` |
| Correlation IDs | UUID string | |
| Error codes | UPPER_SNAKE | `PAYMENT_GATEWAY_TIMEOUT` |
| Open questions | `Q-AREA-###` | `Q-COM-001` |

## 19.2 Module Artifact IDs (Maven)

`khadamati-<module>` e.g. `khadamati-payment`

## 19.3 Gateway Adapter Naming

- Port: `PaymentGatewayPort`  
- Adapter: `IxopayPaymentGatewayAdapter`  
- Do not name ports after Areeba  

## 19.4 Test Naming

`methodUnderTest_condition_expectedResult`  
Example: `debit_whenGatewayTimeout_thenPaymentRemainsPending`

## 19.5 Branch Naming (Cloud Agent Constraint)

`cursor/<descriptive-name>-f98a` lowercase
