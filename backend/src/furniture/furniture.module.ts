import { Module } from '@nestjs/common';
import { FurnitureController } from './furniture.controller';
import { FurnitureRepository } from './furniture.repository';
import { FurnitureService } from './furniture.service';

@Module({
  controllers: [FurnitureController],
  providers: [FurnitureService, FurnitureRepository],
  exports: [FurnitureRepository, FurnitureService],
})
export class FurnitureModule {}
