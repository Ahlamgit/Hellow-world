import { Injectable } from '@nestjs/common';
import { Prisma } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class RoomDesignsRepository {
  constructor(private prisma: PrismaService) {}

  create(data: Prisma.RoomDesignCreateInput) {
    return this.prisma.roomDesign.create({ data, include: { category: true } });
  }

  findAll(categoryId?: string) {
    return this.prisma.roomDesign.findMany({
      where: categoryId ? { categoryId } : undefined,
      include: { category: true },
      orderBy: { title: 'asc' },
    });
  }

  findById(id: string) {
    return this.prisma.roomDesign.findUnique({
      where: { id },
      include: { category: true },
    });
  }

  update(id: string, data: Prisma.RoomDesignUpdateInput) {
    return this.prisma.roomDesign.update({
      where: { id },
      data,
      include: { category: true },
    });
  }

  delete(id: string) {
    return this.prisma.roomDesign.delete({ where: { id } });
  }
}
