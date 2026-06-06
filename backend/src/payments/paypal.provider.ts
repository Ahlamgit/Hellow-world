import { Injectable } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { randomBytes } from 'crypto';
import {
  PaymentIntentResult,
  PaymentProvider,
} from './payment-providers.interface';

@Injectable()
export class PayPalProvider implements PaymentProvider {
  constructor(private config: ConfigService) {}

  createPayment(
    amount: number,
    _currency: string,
    _metadata: Record<string, string>,
  ): Promise<PaymentIntentResult> {
    const transactionId = `PAYPAL_${randomBytes(16).toString('hex')}`;
    const clientId = this.config.get<string>('PAYPAL_CLIENT_ID', 'placeholder');
    return Promise.resolve({
      transactionId,
      approvalUrl: `https://www.sandbox.paypal.com/checkoutnow?token=${transactionId}&client_id=${clientId}&amount=${amount}`,
    });
  }

  confirmPayment(_transactionId: string): Promise<boolean> {
    return Promise.resolve(true);
  }
}
