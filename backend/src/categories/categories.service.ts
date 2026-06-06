import { Injectable, NotFoundException } from '@nestjs/common';
import { CreateCategoryDto, UpdateCategoryDto } from './dto/category.dto';
import { CategoriesRepository } from './categories.repository';

@Injectable()
export class CategoriesService {
  constructor(private repository: CategoriesRepository) {}

  create(dto: CreateCategoryDto) {
    return this.repository.create(dto);
  }

  findAll() {
    return this.repository.findAll();
  }

  async findBySlug(slug: string) {
    const category = await this.repository.findBySlug(slug);
    if (!category) throw new NotFoundException('Category not found');
    return category;
  }

  async update(id: string, dto: UpdateCategoryDto) {
    await this.ensureExists(id);
    return this.repository.update(id, dto);
  }

  async remove(id: string) {
    await this.ensureExists(id);
    return this.repository.delete(id);
  }

  private async ensureExists(id: string) {
    const cat = await this.repository.findById(id);
    if (!cat) throw new NotFoundException('Category not found');
  }
}
