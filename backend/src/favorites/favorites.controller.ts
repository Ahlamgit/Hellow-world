import {
  Controller,
  Delete,
  Get,
  Param,
  Post,
  UseGuards,
} from '@nestjs/common';
import { ApiBearerAuth, ApiTags } from '@nestjs/swagger';
import { CurrentUser } from '../common/decorators/current-user.decorator';
import { JwtAuthGuard } from '../common/guards/jwt-auth.guard';
import { FavoritesService } from './favorites.service';

@ApiTags('favorites')
@ApiBearerAuth()
@UseGuards(JwtAuthGuard)
@Controller('favorites')
export class FavoritesController {
  constructor(private service: FavoritesService) {}

  @Get()
  findAll(@CurrentUser() user: { id: string }) {
    return this.service.findAll(user.id);
  }

  @Post(':furnitureId')
  add(
    @Param('furnitureId') furnitureId: string,
    @CurrentUser() user: { id: string },
  ) {
    return this.service.add(user.id, furnitureId);
  }

  @Delete(':furnitureId')
  remove(
    @Param('furnitureId') furnitureId: string,
    @CurrentUser() user: { id: string },
  ) {
    return this.service.remove(user.id, furnitureId);
  }
}
