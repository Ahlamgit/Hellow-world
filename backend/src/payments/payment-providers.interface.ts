export interface PaymentIntentResult {
  clientSecret?: string;
  approvalUrl?: string;
  transactionId: string;
}

export interface PaymentProvider {
  createPayment(
    amount: number,
    currency: string,
    metadata: Record<string, string>,
  ): Promise<PaymentIntentResult>;
  confirmPayment(transactionId: string): Promise<boolean>;
}
