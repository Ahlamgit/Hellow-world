import { Injectable } from '@nestjs/common';
import { Prisma } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class ProjectsRepository {
  constructor(private prisma: PrismaService) {}

  create(data: Prisma.ProjectCreateInput) {
    return this.prisma.project.create({
      data,
      include: {
        projectRooms: { include: { roomDesign: true } },
        projectItems: { include: { furniture: true } },
      },
    });
  }

  findAll(userId?: string) {
    return this.prisma.project.findMany({
      where: userId ? { userId } : undefined,
      include: {
        projectRooms: { include: { roomDesign: true } },
        projectItems: { include: { furniture: true } },
        quotations: true,
      },
      orderBy: { updatedAt: 'desc' },
    });
  }

  findById(id: string) {
    return this.prisma.project.findUnique({
      where: { id },
      include: {
        projectRooms: { include: { roomDesign: true } },
        projectItems: { include: { furniture: true } },
        quotations: { include: { items: true } },
        user: { select: { id: true, name: true, email: true } },
      },
    });
  }

  update(id: string, data: Prisma.ProjectUpdateInput) {
    return this.prisma.project.update({
      where: { id },
      data,
      include: {
        projectRooms: { include: { roomDesign: true } },
        projectItems: { include: { furniture: true } },
      },
    });
  }

  delete(id: string) {
    return this.prisma.project.delete({ where: { id } });
  }

  addRoom(projectId: string, roomDesignId: string) {
    return this.prisma.projectRoom.create({
      data: { projectId, roomDesignId },
      include: { roomDesign: true },
    });
  }

  addItem(projectId: string, furnitureId: string) {
    return this.prisma.projectItem.create({
      data: { projectId, furnitureId },
      include: { furniture: true },
    });
  }
}
