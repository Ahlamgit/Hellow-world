import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:craftsman_mobile/app/craftsman_app.dart';

void main() {
  testWidgets('Craftsman app shell renders', (WidgetTester tester) async {
    await tester.pumpWidget(const CraftsmanApp());
    expect(find.byType(NavigationBar), findsOneWidget);
  });
}
