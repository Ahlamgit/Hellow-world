import {
  ForbiddenException,
  Injectable,
  NotFoundException,
} from '@nestjs/common';
import { UserRole } from '@prisma/client';
import {
  AddProjectItemDto,
  AddProjectRoomDto,
  CreateProjectDto,
  UpdateProjectDto,
} from './dto/project.dto';
import { ProjectsRepository } from './projects.repository';

@Injectable()
export class ProjectsService {
  constructor(private repository: ProjectsRepository) {}

  create(userId: string, dto: CreateProjectDto) {
    return this.repository.create({
      title: dto.title,
      user: { connect: { id: userId } },
    });
  }

  findAll(userId: string, role: UserRole) {
    return this.repository.findAll(
      role === UserRole.ADMIN ? undefined : userId,
    );
  }

  async findOne(id: string, userId: string, role: UserRole) {
    const project = await this.repository.findById(id);
    if (!project) throw new NotFoundException('Project not found');
    if (role !== UserRole.ADMIN && project.userId !== userId) {
      throw new ForbiddenException();
    }
    return project;
  }

  async update(
    id: string,
    dto: UpdateProjectDto,
    userId: string,
    role: UserRole,
  ) {
    await this.findOne(id, userId, role);
    return this.repository.update(id, dto);
  }

  async remove(id: string, userId: string, role: UserRole) {
    await this.findOne(id, userId, role);
    return this.repository.delete(id);
  }

  async addRoom(
    id: string,
    dto: AddProjectRoomDto,
    userId: string,
    role: UserRole,
  ) {
    await this.findOne(id, userId, role);
    return this.repository.addRoom(id, dto.roomDesignId);
  }

  async addItem(
    id: string,
    dto: AddProjectItemDto,
    userId: string,
    role: UserRole,
  ) {
    await this.findOne(id, userId, role);
    return this.repository.addItem(id, dto.furnitureId);
  }
}
