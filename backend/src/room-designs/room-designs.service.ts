import { Injectable, NotFoundException } from '@nestjs/common';
import {
  CreateRoomDesignDto,
  UpdateRoomDesignDto,
} from './dto/room-design.dto';
import { RoomDesignsRepository } from './room-designs.repository';

@Injectable()
export class RoomDesignsService {
  constructor(private repository: RoomDesignsRepository) {}

  create(dto: CreateRoomDesignDto) {
    const { categoryId, ...rest } = dto;
    return this.repository.create({
      ...rest,
      category: { connect: { id: categoryId } },
    });
  }

  findAll(categoryId?: string) {
    return this.repository.findAll(categoryId);
  }

  async findOne(id: string) {
    const design = await this.repository.findById(id);
    if (!design) throw new NotFoundException('Room design not found');
    return design;
  }

  async update(id: string, dto: UpdateRoomDesignDto) {
    await this.findOne(id);
    return this.repository.update(id, dto);
  }

  async remove(id: string) {
    await this.findOne(id);
    return this.repository.delete(id);
  }
}
