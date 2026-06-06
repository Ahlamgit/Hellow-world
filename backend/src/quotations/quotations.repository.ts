import { Injectable } from '@nestjs/common';
import { Prisma } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class QuotationsRepository {
  constructor(private prisma: PrismaService) {}

  create(data: Prisma.QuotationCreateInput) {
    return this.prisma.quotation.create({
      data,
      include: { items: true, project: true },
    });
  }

  findAll(projectIds?: string[]) {
    return this.prisma.quotation.findMany({
      where: projectIds ? { projectId: { in: projectIds } } : undefined,
      include: { items: true, project: true },
      orderBy: { createdAt: 'desc' },
    });
  }

  findById(id: string) {
    return this.prisma.quotation.findUnique({
      where: { id },
      include: { items: true, project: { include: { user: true } } },
    });
  }

  update(id: string, data: Prisma.QuotationUpdateInput) {
    return this.prisma.quotation.update({
      where: { id },
      data,
      include: { items: true },
    });
  }

  delete(id: string) {
    return this.prisma.quotation.delete({ where: { id } });
  }

  addItem(
    quotationId: string,
    item: { description: string; quantity: number; price: number },
  ) {
    return this.prisma.quotationItem.create({
      data: { quotationId, ...item },
    });
  }

  async recalculateTotal(quotationId: string) {
    const items = await this.prisma.quotationItem.findMany({
      where: { quotationId },
    });
    const total = items.reduce(
      (sum, i) => sum + Number(i.price) * i.quantity,
      0,
    );
    return this.prisma.quotation.update({
      where: { id: quotationId },
      data: { totalAmount: total },
      include: { items: true },
    });
  }
}
