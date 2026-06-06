import { BadRequestException, Inject, Injectable } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import type { StorageProvider, UploadedFileResult } from './storage.interface';
import { STORAGE_PROVIDER } from './storage.interface';

const ALLOWED_MIME = ['image/jpeg', 'image/png', 'image/webp'];

@Injectable()
export class StorageService {
  constructor(
    @Inject(STORAGE_PROVIDER) private provider: StorageProvider,
    private config: ConfigService,
  ) {}

  async uploadImage(
    file: Express.Multer.File,
    folder: string,
  ): Promise<UploadedFileResult> {
    const maxMb = this.config.get<number>('MAX_UPLOAD_SIZE_MB', 10);
    const maxBytes = maxMb * 1024 * 1024;

    if (!ALLOWED_MIME.includes(file.mimetype)) {
      throw new BadRequestException(
        'Only JPG, PNG, and WEBP images are allowed',
      );
    }
    if (file.size > maxBytes) {
      throw new BadRequestException(`File exceeds ${maxMb}MB limit`);
    }

    return this.provider.upload(file, folder);
  }

  async delete(key: string): Promise<void> {
    return this.provider.delete(key);
  }

  getUrl(key: string): string {
    return this.provider.getUrl(key);
  }
}
