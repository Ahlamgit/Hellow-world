import { Controller, Get, Param } from '@nestjs/common';
import { ApiTags } from '@nestjs/swagger';
import { VrService } from './vr.service';

@ApiTags('vr')
@Controller('vr')
export class VrController {
  constructor(private service: VrService) {}

  @Get('scenes')
  findAll() {
    return this.service.findAllScenes();
  }

  @Get('scenes/:slug')
  findBySlug(@Param('slug') slug: string) {
    return this.service.findBySlug(slug);
  }

  @Get('category/:categorySlug')
  findByCategory(@Param('categorySlug') categorySlug: string) {
    return this.service.findByCategorySlug(categorySlug);
  }
}
