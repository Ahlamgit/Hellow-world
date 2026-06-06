import { Injectable } from '@nestjs/common';
import { Prisma } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class FurnitureRepository {
  constructor(private prisma: PrismaService) {}

  create(data: Prisma.FurnitureItemCreateInput) {
    return this.prisma.furnitureItem.create({
      data,
      include: { category: true },
    });
  }

  findAll(categoryId?: string) {
    return this.prisma.furnitureItem.findMany({
      where: categoryId ? { categoryId } : undefined,
      include: { category: true },
      orderBy: { name: 'asc' },
    });
  }

  findById(id: string) {
    return this.prisma.furnitureItem.findUnique({
      where: { id },
      include: { category: true },
    });
  }

  update(id: string, data: Prisma.FurnitureItemUpdateInput) {
    return this.prisma.furnitureItem.update({
      where: { id },
      data,
      include: { category: true },
    });
  }

  delete(id: string) {
    return this.prisma.furnitureItem.delete({ where: { id } });
  }
}
