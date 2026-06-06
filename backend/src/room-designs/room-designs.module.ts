import { Module } from '@nestjs/common';
import { RoomDesignsController } from './room-designs.controller';
import { RoomDesignsRepository } from './room-designs.repository';
import { RoomDesignsService } from './room-designs.service';

@Module({
  controllers: [RoomDesignsController],
  providers: [RoomDesignsService, RoomDesignsRepository],
  exports: [RoomDesignsRepository, RoomDesignsService],
})
export class RoomDesignsModule {}
