import 'package:flutter/material.dart';
import 'shell_page.dart';

class CustomerShell extends StatefulWidget {
  const CustomerShell({super.key, required this.isArabic, required this.onSignOut});

  final bool isArabic;
  final VoidCallback onSignOut;

  @override
  State<CustomerShell> createState() => _CustomerShellState();
}

class _CustomerShellState extends State<CustomerShell> {
  int _tabIndex = 0;

  static const _tabKeys = ['home', 'bookings', 'messages', 'profile'];

  List<String> _labels(bool ar) {
    if (ar) {
      return ['الرئيسية', 'الحجوزات', 'الرسائل', 'الملف'];
    }
    return ['Home', 'Bookings', 'Messages', 'Profile'];
  }

  @override
  Widget build(BuildContext context) {
    final labels = _labels(widget.isArabic);
    return Directionality(
      textDirection: widget.isArabic ? TextDirection.rtl : TextDirection.ltr,
      child: Scaffold(
        appBar: AppBar(
          title: Text(widget.isArabic ? 'خدماتي — عميل' : 'KHADAMATI — Customer'),
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
      'home' => Icons.home_outlined,
      'bookings' => Icons.event_note_outlined,
      'messages' => Icons.chat_bubble_outline,
      _ => Icons.person_outline,
    };
  }
}
