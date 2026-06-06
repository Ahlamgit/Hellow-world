import { IsEnum, IsUUID } from 'class-validator';
import { PaymentMethod } from '@prisma/client';

export class CreatePaymentDto {
  @IsUUID()
  quotationId: string;

  @IsEnum(PaymentMethod)
  paymentMethod: PaymentMethod;
}

export class UpdatePaymentDto {
  @IsEnum(PaymentMethod)
  paymentMethod?: PaymentMethod;
}
