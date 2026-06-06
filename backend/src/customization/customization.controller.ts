import {
  Body,
  Controller,
  Delete,
  Get,
  Param,
  Patch,
  Post,
  UseGuards,
} from '@nestjs/common';
import { ApiBearerAuth, ApiTags } from '@nestjs/swagger';
import { UserRole } from '@prisma/client';
import { CurrentUser } from '../common/decorators/current-user.decorator';
import { JwtAuthGuard } from '../common/guards/jwt-auth.guard';
import {
  CreateCustomizationDto,
  UpdateCustomizationDto,
} from './dto/customization.dto';
import { CustomizationService } from './customization.service';

@ApiTags('customization')
@ApiBearerAuth()
@UseGuards(JwtAuthGuard)
@Controller('customization')
export class CustomizationController {
  constructor(private service: CustomizationService) {}

  @Post()
  create(
    @Body() dto: CreateCustomizationDto,
    @CurrentUser() user: { id: string },
  ) {
    return this.service.create(user.id, dto);
  }

  @Get()
  findAll(@CurrentUser() user: { id: string; role: UserRole }) {
    return this.service.findAll(user.id, user.role);
  }

  @Get(':id')
  findOne(
    @Param('id') id: string,
    @CurrentUser() user: { id: string; role: UserRole },
  ) {
    return this.service.findOne(id, user.id, user.role);
  }

  @Patch(':id')
  update(
    @Param('id') id: string,
    @Body() dto: UpdateCustomizationDto,
    @CurrentUser() user: { id: string; role: UserRole },
  ) {
    return this.service.update(id, dto, user.id, user.role);
  }

  @Delete(':id')
  remove(
    @Param('id') id: string,
    @CurrentUser() user: { id: string; role: UserRole },
  ) {
    return this.service.remove(id, user.id, user.role);
  }
}
