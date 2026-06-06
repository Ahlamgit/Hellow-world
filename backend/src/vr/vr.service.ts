import { Injectable, NotFoundException } from '@nestjs/common';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class VrService {
  constructor(private prisma: PrismaService) {}

  findAllScenes() {
    return this.prisma.vrScene.findMany({
      include: {
        furniture: { include: { furniture: { include: { category: true } } } },
      },
    });
  }

  async findBySlug(slug: string) {
    const scene = await this.prisma.vrScene.findUnique({
      where: { slug },
      include: {
        furniture: {
          include: {
            furniture: {
              include: { category: true },
            },
          },
        },
      },
    });
    if (!scene) throw new NotFoundException('VR scene not found');
    return {
      ...scene,
      furnitureItems: scene.furniture.map((f) => ({
        ...f.furniture,
        position: { x: f.positionX, y: f.positionY, z: f.positionZ },
      })),
    };
  }

  async findByCategorySlug(categorySlug: string) {
    const scene = await this.prisma.vrScene.findFirst({
      where: { categorySlug },
      include: {
        furniture: { include: { furniture: true } },
      },
    });
    if (!scene) throw new NotFoundException('VR scene not found for category');
    return this.findBySlug(scene.slug);
  }
}
