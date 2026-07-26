import 'package:flutter/material.dart';
import 'shell_page.dart';

class CraftsmanShell extends StatefulWidget {
  const CraftsmanShell({super.key, required this.isArabic, required this.onSignOut});

  final bool isArabic;
  final VoidCallback onSignOut;

  @override
  State<CraftsmanShell> createState() => _CraftsmanShellState();
}

class _CraftsmanShellState extends State<CraftsmanShell> {
  int _tabIndex = 0;

  static const _tabKeys = ['dashboard', 'schedule', 'messages', 'profile'];

  List<String> _labels(bool ar) {
    if (ar) {
      return ['لوحة التحكم', 'الجدول', 'الرسائل', 'الملف'];
    }
    return ['Dashboard', 'Schedule', 'Messages', 'Profile'];
  }

  @override
  Widget build(BuildContext context) {
    final labels = _labels(widget.isArabic);
    return Directionality(
      textDirection: widget.isArabic ? TextDirection.rtl : TextDirection.ltr,
      child: Scaffold(
        appBar: AppBar(
          title: Text(widget.isArabic ? 'خدماتي — حرفي' : 'KHADAMATI — Craftsman'),
          actions: [
            IconButton(onPressed: widget.onSignOut, icon: const Icon(Icons.logout), tooltip: 'Sign out'),
          ],
        ),
        body: ShellPage(
          title: labels[_tabIndex],
          subtitle: widget.isArabic ? 'هيكل Sprint 0 — مسار مؤقت' : 'Sprint 0 shell — placeholder route',
        ),
        bottomNavigationBar: NavigationBar(
          selectedIndex: _tabIndex,
          onDestinationSelected: (index) => setState(() => _tabIndex = index),
          destinations: List.generate(
            labels.length,
            (i) => NavigationDestination(
              icon: Icon(_iconFor(_tabKeys[i])),
              label: labels[i],
            ),
          ),
        ),
      ),
    );
  }

  IconData _iconFor(String key) {
    return switch (key) {
      'dashboard' => Icons.dashboard_outlined,
      'schedule' => Icons.calendar_month_outlined,
      'messages' => Icons.chat_bubble_outline,
      _ => Icons.person_outline,
    };
  }
}
