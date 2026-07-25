import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:customer_mobile/app/customer_app.dart';

void main() {
  testWidgets('Customer app shell renders', (WidgetTester tester) async {
    await tester.pumpWidget(const CustomerApp());
    expect(find.byType(NavigationBar), findsOneWidget);
  });
}
