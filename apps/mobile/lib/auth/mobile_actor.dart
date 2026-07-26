import 'package:flutter/material.dart' show IconData, Icons;

enum MobileActor { customer, craftsman, store }

extension MobileActorX on MobileActor {
  String get routeName => name;

  String label(bool isArabic) {
    switch (this) {
      case MobileActor.customer:
        return isArabic ? 'عميل' : 'Customer';
      case MobileActor.craftsman:
        return isArabic ? 'حرفي' : 'Craftsman';
      case MobileActor.store:
        return isArabic ? 'متجر' : 'Store';
    }
  }

  String description(bool isArabic) {
    switch (this) {
      case MobileActor.customer:
        return isArabic ? 'احجز الخدمات وأدرها' : 'Book and manage services';
      case MobileActor.craftsman:
        return isArabic ? 'مزود خدمة فردي' : 'Individual service provider';
      case MobileActor.store:
        return isArabic ? 'إدارة أعمال الخدمات' : 'Manage your service business';
    }
  }

  IconData get icon {
    switch (this) {
      case MobileActor.customer:
        return Icons.person_outline;
      case MobileActor.craftsman:
        return Icons.handyman_outlined;
      case MobileActor.store:
        return Icons.storefront_outlined;
    }
  }
}
