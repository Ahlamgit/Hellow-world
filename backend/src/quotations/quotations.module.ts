import { Module } from '@nestjs/common';
import { ProjectsModule } from '../projects/projects.module';
import { QuotationsController } from './quotations.controller';
import { QuotationsRepository } from './quotations.repository';
import { QuotationsService } from './quotations.service';

@Module({
  imports: [ProjectsModule],
  controllers: [QuotationsController],
  providers: [QuotationsService, QuotationsRepository],
  exports: [QuotationsRepository, QuotationsService],
})
export class QuotationsModule {}
