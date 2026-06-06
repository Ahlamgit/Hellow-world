import { Injectable } from '@nestjs/common';
import { CustomizationStatus, Prisma } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class CustomizationRepository {
  constructor(private prisma: PrismaService) {}

  create(data: Prisma.CustomizationRequestCreateInput) {
    return this.prisma.customizationRequest.create({
      data,
      include: { furniture: true },
    });
  }

  findAll(userId?: string) {
    return this.prisma.customizationRequest.findMany({
      where: userId ? { userId } : undefined,
      include: {
        furniture: true,
        user: { select: { id: true, name: true, email: true } },
      },
      orderBy: { createdAt: 'desc' },
    });
  }

  findById(id: string) {
    return this.prisma.customizationRequest.findUnique({
      where: { id },
      include: { furniture: true, user: true },
    });
  }

  update(id: string, data: Prisma.CustomizationRequestUpdateInput) {
    return this.prisma.customizationRequest.update({
      where: { id },
      data,
      include: { furniture: true },
    });
  }

  delete(id: string) {
    return this.prisma.customizationRequest.delete({ where: { id } });
  }
}
