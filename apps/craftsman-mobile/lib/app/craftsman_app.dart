import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import '../theme/khadamati_theme.dart';
import 'shell_page.dart';

class CraftsmanApp extends StatefulWidget {
  const CraftsmanApp({super.key});

  @override
  State<CraftsmanApp> createState() => _CraftsmanAppState();
}

class _CraftsmanAppState extends State<CraftsmanApp> {
  Locale _locale = const Locale('ar');
  int _tabIndex = 0;

  static const _tabs = [
    (icon: Icons.dashboard_outlined, labelAr: 'لوحة التحكم', labelEn: 'Dashboard'),
    (icon: Icons.calendar_month_outlined, labelAr: 'الجدول', labelEn: 'Schedule'),
    (icon: Icons.chat_bubble_outline, labelAr: 'الرسائل', labelEn: 'Messages'),
    (icon: Icons.person_outline, labelAr: 'الملف', labelEn: 'Profile'),
  ];

  void _toggleLocale() {
    setState(() {
      _locale = _locale.languageCode == 'ar' ? const Locale('en') : const Locale('ar');
    });
  }

  @override
  Widget build(BuildContext context) {
    final isArabic = _locale.languageCode == 'ar';
    return MaterialApp(
      title: isArabic ? 'خدماتي' : 'KHADAMATI',
      theme: KhadamatiTheme.light(rtl: isArabic),
      locale: _locale,
      supportedLocales: const [Locale('ar'), Locale('en')],
      localizationsDelegates: const [
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      builder: (context, child) {
        return Directionality(
          textDirection: isArabic ? TextDirection.rtl : TextDirection.ltr,
          child: child ?? const SizedBox.shrink(),
        );
      },
      home: Scaffold(
        appBar: AppBar(
          title: Text(isArabic ? 'خدماتي — حرفي' : 'KHADAMATI — Craftsman'),
          actions: [
            IconButton(
              onPressed: _toggleLocale,
              icon: const Icon(Icons.language),
              tooltip: isArabic ? 'English' : 'العربية',
            ),
          ],
        ),
        body: ShellPage(
          title: isArabic ? _tabs[_tabIndex].labelAr : _tabs[_tabIndex].labelEn,
          subtitle: isArabic ? 'هيكل Sprint 0 — مسار مؤقت' : 'Sprint 0 shell — placeholder route',
        ),
        bottomNavigationBar: NavigationBar(
          selectedIndex: _tabIndex,
          onDestinationSelected: (index) => setState(() => _tabIndex = index),
          destinations: _tabs
              .map(
                (tab) => NavigationDestination(
                  icon: Icon(tab.icon),
                  label: isArabic ? tab.labelAr : tab.labelEn,
                ),
              )
              .toList(),
        ),
      ),
    );
  }
}
