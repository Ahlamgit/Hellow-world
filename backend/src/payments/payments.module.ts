import { Module } from '@nestjs/common';
import { QuotationsModule } from '../quotations/quotations.module';
import { PayPalProvider } from './paypal.provider';
import { PaymentsController } from './payments.controller';
import { PaymentsRepository } from './payments.repository';
import { PaymentsService } from './payments.service';
import { StripeProvider } from './stripe.provider';

@Module({
  imports: [QuotationsModule],
  controllers: [PaymentsController],
  providers: [
    PaymentsService,
    PaymentsRepository,
    StripeProvider,
    PayPalProvider,
  ],
  exports: [PaymentsRepository, PaymentsService],
})
export class PaymentsModule {}
