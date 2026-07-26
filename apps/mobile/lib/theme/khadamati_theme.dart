import 'package:flutter/material.dart';

class KhadamatiTheme {
  static const Color brandPrimary = Color(0xFFF57C00);
  static const Color brandPrimaryDark = Color(0xFFE65100);
  static const Color brandPrimaryLight = Color(0xFFFFB74D);

  static ThemeData light() {
    return ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: brandPrimary,
        primary: brandPrimary,
        brightness: Brightness.light,
      ),
      appBarTheme: const AppBarTheme(
        backgroundColor: brandPrimary,
        foregroundColor: Colors.white,
      ),
      navigationBarTheme: const NavigationBarThemeData(
        indicatorColor: brandPrimaryLight,
      ),
    );
  }
}
