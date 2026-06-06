import { Injectable } from '@nestjs/common';
import {
  PaymentStatus,
  ProjectStatus,
  QuotationStatus,
  UserRole,
} from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class AnalyticsService {
  constructor(private prisma: PrismaService) {}

  async getDashboardStats() {
    const [
      totalUsers,
      totalCustomers,
      totalProjects,
      activeProjects,
      totalQuotations,
      pendingQuotations,
      totalPayments,
      paidPayments,
      totalRevenue,
      customizationPending,
      vrScenes,
      furnitureCount,
    ] = await Promise.all([
      this.prisma.user.count(),
      this.prisma.user.count({ where: { role: UserRole.CUSTOMER } }),
      this.prisma.project.count(),
      this.prisma.project.count({
        where: {
          status: { in: [ProjectStatus.ACTIVE, ProjectStatus.IN_PROGRESS] },
        },
      }),
      this.prisma.quotation.count(),
      this.prisma.quotation.count({ where: { status: QuotationStatus.SENT } }),
      this.prisma.payment.count(),
      this.prisma.payment.count({ where: { status: PaymentStatus.PAID } }),
      this.prisma.payment.aggregate({
        where: { status: PaymentStatus.PAID },
        _sum: { amount: true },
      }),
      this.prisma.customizationRequest.count({
        where: { status: 'PENDING' },
      }),
      this.prisma.vrScene.count(),
      this.prisma.furnitureItem.count(),
    ]);

    return {
      users: { total: totalUsers, customers: totalCustomers },
      projects: { total: totalProjects, active: activeProjects },
      quotations: { total: totalQuotations, pending: pendingQuotations },
      payments: {
        total: totalPayments,
        paid: paidPayments,
        revenue: Number(totalRevenue._sum.amount ?? 0),
      },
      customizationPending,
      vrScenes,
      furnitureCount,
    };
  }
}
