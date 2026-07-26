import 'package:flutter/material.dart';
import 'shell_page.dart';

class StoreShell extends StatefulWidget {
  const StoreShell({super.key, required this.isArabic, required this.onSignOut});

  final bool isArabic;
  final VoidCallback onSignOut;

  @override
  State<StoreShell> createState() => _StoreShellState();
}

class _StoreShellState extends State<StoreShell> {
  int _tabIndex = 0;

  static const _tabKeys = ['dashboard', 'services', 'analytics', 'settings'];

  List<String> _labels(bool ar) {
    if (ar) {
      return ['لوحة التحكم', 'الخدمات', 'التحليلات', 'الإعدادات'];
    }
    return ['Dashboard', 'Services', 'Analytics', 'Settings'];
  }

  @override
  Widget build(BuildContext context) {
    final labels = _labels(widget.isArabic);
    return Directionality(
      textDirection: widget.isArabic ? TextDirection.rtl : TextDirection.ltr,
      child: Scaffold(
        appBar: AppBar(
          title: Text(widget.isArabic ? 'خدماتي — متجر' : 'KHADAMATI — Store'),
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
      'services' => Icons.build_outlined,
      'analytics' => Icons.bar_chart_outlined,
      _ => Icons.settings_outlined,
    };
  }
}
