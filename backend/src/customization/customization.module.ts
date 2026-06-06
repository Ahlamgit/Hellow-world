import { Module } from '@nestjs/common';
import { CustomizationController } from './customization.controller';
import { CustomizationRepository } from './customization.repository';
import { CustomizationService } from './customization.service';

@Module({
  controllers: [CustomizationController],
  providers: [CustomizationService, CustomizationRepository],
  exports: [CustomizationRepository, CustomizationService],
})
export class CustomizationModule {}
