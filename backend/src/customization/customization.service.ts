import {
  ForbiddenException,
  Injectable,
  NotFoundException,
} from '@nestjs/common';
import { UserRole } from '@prisma/client';
import {
  CreateCustomizationDto,
  UpdateCustomizationDto,
} from './dto/customization.dto';
import { CustomizationRepository } from './customization.repository';

@Injectable()
export class CustomizationService {
  constructor(private repository: CustomizationRepository) {}

  create(userId: string, dto: CreateCustomizationDto) {
    const { furnitureId, ...rest } = dto;
    return this.repository.create({
      ...rest,
      user: { connect: { id: userId } },
      furniture: { connect: { id: furnitureId } },
    });
  }

  findAll(userId: string, role: UserRole) {
    return this.repository.findAll(
      role === UserRole.ADMIN ? undefined : userId,
    );
  }

  async findOne(id: string, userId: string, role: UserRole) {
    const request = await this.repository.findById(id);
    if (!request) throw new NotFoundException('Request not found');
    if (role !== UserRole.ADMIN && request.userId !== userId) {
      throw new ForbiddenException();
    }
    return request;
  }

  async update(
    id: string,
    dto: UpdateCustomizationDto,
    userId: string,
    role: UserRole,
  ) {
    await this.findOne(id, userId, role);
    if (role !== UserRole.ADMIN && dto.status) {
      throw new ForbiddenException('Only admins can update status');
    }
    return this.repository.update(id, dto);
  }

  async remove(id: string, userId: string, role: UserRole) {
    await this.findOne(id, userId, role);
    return this.repository.delete(id);
  }
}
