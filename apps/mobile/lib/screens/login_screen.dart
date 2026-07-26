import 'package:flutter/material.dart';
import '../auth/mobile_actor.dart';
import '../theme/khadamati_theme.dart';
import 'customer_shell.dart';
import 'craftsman_shell.dart';
import 'store_shell.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  bool _isArabic = true;

  void _toggleLanguage() {
    setState(() => _isArabic = !_isArabic);
  }

  void _openActor(MobileActor actor) {
    final Widget shell = switch (actor) {
      MobileActor.customer => CustomerShell(isArabic: _isArabic, onSignOut: _popToLogin),
      MobileActor.craftsman => CraftsmanShell(isArabic: _isArabic, onSignOut: _popToLogin),
      MobileActor.store => StoreShell(isArabic: _isArabic, onSignOut: _popToLogin),
    };
    Navigator.of(context).push(MaterialPageRoute(builder: (_) => shell));
  }

  void _popToLogin() {
    Navigator.of(context).popUntil((route) => route.isFirst);
  }

  @override
  Widget build(BuildContext context) {
    final title = _isArabic ? 'تسجيل الدخول إلى خدماتي' : 'Sign in to KHADAMATI';
    final subtitle = _isArabic ? 'اختر نوع حسابك' : 'Choose your account type';
    final continueLabel = _isArabic ? 'متابعة' : 'Continue';

    return Directionality(
      textDirection: _isArabic ? TextDirection.rtl : TextDirection.ltr,
      child: Scaffold(
        appBar: AppBar(
          title: Text(_isArabic ? 'خدماتي' : 'KHADAMATI'),
          actions: [
            IconButton(
              onPressed: _toggleLanguage,
              icon: const Icon(Icons.language),
              tooltip: _isArabic ? 'English' : 'العربية',
            ),
          ],
        ),
        body: ListView(
          padding: const EdgeInsets.all(24),
          children: [
            Text(title, style: Theme.of(context).textTheme.headlineSmall, textAlign: TextAlign.center),
            const SizedBox(height: 8),
            Text(
              subtitle,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(color: Colors.grey[700]),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 24),
            for (final actor in MobileActor.values) ...[
              Card(
                child: InkWell(
                  onTap: () => _openActor(actor),
                  borderRadius: BorderRadius.circular(12),
                  child: Padding(
                    padding: const EdgeInsets.all(20),
                    child: Column(
                      children: [
                        Icon(actor.icon, size: 40, color: KhadamatiTheme.brandPrimary),
                        const SizedBox(height: 12),
                        Text(actor.label(_isArabic), style: Theme.of(context).textTheme.titleLarge),
                        const SizedBox(height: 4),
                        Text(actor.description(_isArabic), textAlign: TextAlign.center),
                        const SizedBox(height: 12),
                        FilledButton(onPressed: () => _openActor(actor), child: Text(continueLabel)),
                      ],
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 12),
            ],
            Text(
              _isArabic ? 'المسؤول — تسجيل الدخول عبر الويب فقط' : 'Admin — web sign-in only',
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.grey[600]),
            ),
          ],
        ),
      ),
    );
  }
}
