import { Injectable } from '@nestjs/common';
import { Prisma } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class PaymentsRepository {
  constructor(private prisma: PrismaService) {}

  create(data: Prisma.PaymentCreateInput) {
    return this.prisma.payment.create({
      data,
      include: { quotation: true },
    });
  }

  findAll(userId?: string) {
    return this.prisma.payment.findMany({
      where: userId ? { userId } : undefined,
      include: { quotation: true },
      orderBy: { createdAt: 'desc' },
    });
  }

  findById(id: string) {
    return this.prisma.payment.findUnique({
      where: { id },
      include: { quotation: { include: { project: true } } },
    });
  }

  update(id: string, data: Prisma.PaymentUpdateInput) {
    return this.prisma.payment.update({ where: { id }, data });
  }

  delete(id: string) {
    return this.prisma.payment.delete({ where: { id } });
  }
}
