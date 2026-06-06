import { Injectable } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import Stripe from 'stripe';
import {
  PaymentIntentResult,
  PaymentProvider,
} from './payment-providers.interface';

@Injectable()
export class StripeProvider implements PaymentProvider {
  private stripe: InstanceType<typeof Stripe>;

  constructor(config: ConfigService) {
    this.stripe = new Stripe(
      config.get<string>('STRIPE_SECRET_KEY', 'sk_test_placeholder'),
    );
  }

  async createPayment(
    amount: number,
    currency: string,
    metadata: Record<string, string>,
  ): Promise<PaymentIntentResult> {
    const intent = await this.stripe.paymentIntents.create({
      amount: Math.round(amount * 100),
      currency,
      metadata,
      automatic_payment_methods: { enabled: true },
    });
    return {
      clientSecret: intent.client_secret ?? undefined,
      transactionId: intent.id,
    };
  }

  async confirmPayment(transactionId: string): Promise<boolean> {
    const intent = await this.stripe.paymentIntents.retrieve(transactionId);
    return intent.status === 'succeeded';
  }
}
