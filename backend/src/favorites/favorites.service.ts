import { Injectable } from '@nestjs/common';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class FavoritesService {
  constructor(private prisma: PrismaService) {}

  findAll(userId: string) {
    return this.prisma.favorite.findMany({
      where: { userId },
      include: { furniture: { include: { category: true } } },
    });
  }

  add(userId: string, furnitureId: string) {
    return this.prisma.favorite.upsert({
      where: { userId_furnitureId: { userId, furnitureId } },
      create: { userId, furnitureId },
      update: {},
      include: { furniture: true },
    });
  }

  remove(userId: string, furnitureId: string) {
    return this.prisma.favorite.delete({
      where: { userId_furnitureId: { userId, furnitureId } },
    });
  }
}
