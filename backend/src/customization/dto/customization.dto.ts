import { IsEnum, IsOptional, IsString, IsUUID } from 'class-validator';
import { CustomizationStatus } from '@prisma/client';

export class CreateCustomizationDto {
  @IsUUID()
  furnitureId: string;

  @IsOptional()
  @IsString()
  material?: string;

  @IsOptional()
  @IsString()
  color?: string;

  @IsOptional()
  @IsString()
  notes?: string;

  @IsOptional()
  @IsString()
  referenceImageUrl?: string;
}

export class UpdateCustomizationDto {
  @IsOptional()
  @IsEnum(CustomizationStatus)
  status?: CustomizationStatus;

  @IsOptional()
  @IsString()
  material?: string;

  @IsOptional()
  @IsString()
  color?: string;

  @IsOptional()
  @IsString()
  notes?: string;
}
