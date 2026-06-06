import { IsBoolean, IsOptional, IsString, IsUUID } from 'class-validator';

export class CreateRoomDesignDto {
  @IsUUID()
  categoryId: string;

  @IsString()
  title: string;

  @IsOptional()
  @IsString()
  description?: string;

  @IsOptional()
  @IsString()
  coverImage?: string;

  @IsOptional()
  @IsBoolean()
  vrEnabled?: boolean;

  @IsOptional()
  @IsString()
  vrSceneId?: string;
}

export class UpdateRoomDesignDto {
  @IsOptional()
  @IsString()
  title?: string;

  @IsOptional()
  @IsString()
  description?: string;

  @IsOptional()
  @IsString()
  coverImage?: string;

  @IsOptional()
  @IsBoolean()
  vrEnabled?: boolean;
}
