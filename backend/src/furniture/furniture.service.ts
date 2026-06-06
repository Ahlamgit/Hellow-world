import { Injectable, NotFoundException } from '@nestjs/common';
import { CreateFurnitureDto, UpdateFurnitureDto } from './dto/furniture.dto';
import { FurnitureRepository } from './furniture.repository';

@Injectable()
export class FurnitureService {
  constructor(private repository: FurnitureRepository) {}

  create(dto: CreateFurnitureDto) {
    const { categoryId, price, ...rest } = dto;
    return this.repository.create({
      ...rest,
      price,
      category: { connect: { id: categoryId } },
    });
  }

  findAll(categoryId?: string) {
    return this.repository.findAll(categoryId);
  }

  async findOne(id: string) {
    const item = await this.repository.findById(id);
    if (!item) throw new NotFoundException('Furniture not found');
    return item;
  }

  async update(id: string, dto: UpdateFurnitureDto) {
    await this.findOne(id);
    return this.repository.update(id, dto);
  }

  async remove(id: string) {
    await this.findOne(id);
    return this.repository.delete(id);
  }
}
