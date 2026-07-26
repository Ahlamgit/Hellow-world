import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:khadamati_mobile/main.dart';

void main() {
  testWidgets('Login screen shows Customer, Craftsman, and Store options', (WidgetTester tester) async {
    tester.view.physicalSize = const Size(400, 1200);
    tester.view.devicePixelRatio = 1.0;
    addTearDown(tester.view.reset);

    await tester.pumpWidget(const KhadamatiMobileApp());
    await tester.pumpAndSettle();

    expect(find.text('عميل'), findsOneWidget);
    expect(find.text('حرفي'), findsOneWidget);
    expect(find.byIcon(Icons.storefront_outlined), findsOneWidget);
    expect(find.byType(NavigationBar), findsNothing);
  });
}
