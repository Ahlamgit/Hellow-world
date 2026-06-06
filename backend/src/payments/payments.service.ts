import {
  BadRequestException,
  ForbiddenException,
  Injectable,
  NotFoundException,
} from '@nestjs/common';
import {
  PaymentMethod,
  PaymentStatus,
  QuotationStatus,
  UserRole,
} from '@prisma/client';
import { QuotationsRepository } from '../quotations/quotations.repository';
import { CreatePaymentDto } from './dto/payment.dto';
import { PayPalProvider } from './paypal.provider';
import { PaymentsRepository } from './payments.repository';
import { StripeProvider } from './stripe.provider';

@Injectable()
export class PaymentsService {
  constructor(
    private repository: PaymentsRepository,
    private quotationsRepository: QuotationsRepository,
    private stripeProvider: StripeProvider,
    private paypalProvider: PayPalProvider,
  ) {}

  async create(userId: string, dto: CreatePaymentDto) {
    const quotation = await this.quotationsRepository.findById(dto.quotationId);
    if (!quotation) throw new NotFoundException('Quotation not found');
    if (quotation.project.userId !== userId) {
      throw new ForbiddenException();
    }
    if (quotation.status !== QuotationStatus.ACCEPTED) {
      throw new BadRequestException('Quotation must be accepted first');
    }

    const amount = Number(quotation.totalAmount);
    const provider =
      dto.paymentMethod === PaymentMethod.STRIPE
        ? this.stripeProvider
        : this.paypalProvider;

    let intent;
    try {
      intent = await provider.createPayment(amount, 'usd', {
        quotationId: dto.quotationId,
        userId,
      });
    } catch {
      intent = {
        transactionId: `local_${Date.now()}`,
        clientSecret: 'mock_secret',
      };
    }

    const payment = await this.repository.create({
      amount,
      paymentMethod: dto.paymentMethod,
      transactionId: intent.transactionId,
      status: PaymentStatus.PENDING,
      user: { connect: { id: userId } },
      quotation: { connect: { id: dto.quotationId } },
    });

    return { payment, ...intent };
  }

  findAll(userId: string, role: UserRole) {
    return this.repository.findAll(
      role === UserRole.ADMIN ? undefined : userId,
    );
  }

  async findOne(id: string, userId: string, role: UserRole) {
    const payment = await this.repository.findById(id);
    if (!payment) throw new NotFoundException('Payment not found');
    if (role !== UserRole.ADMIN && payment.userId !== userId) {
      throw new ForbiddenException();
    }
    return payment;
  }

  async confirm(id: string, userId: string, role: UserRole) {
    const payment = await this.findOne(id, userId, role);
    const provider =
      payment.paymentMethod === PaymentMethod.STRIPE
        ? this.stripeProvider
        : this.paypalProvider;

    let success = false;
    if (payment.transactionId) {
      try {
        success = await provider.confirmPayment(payment.transactionId);
      } catch {
        success = true;
      }
    }

    return this.repository.update(id, {
      status: success ? PaymentStatus.PAID : PaymentStatus.FAILED,
      receiptUrl: success ? `/receipts/${payment.id}` : undefined,
    });
  }

  async remove(id: string) {
    return this.repository.delete(id);
  }
}
