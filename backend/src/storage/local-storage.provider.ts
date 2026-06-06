import { Injectable } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { mkdir, unlink, writeFile } from 'fs/promises';
import { join } from 'path';
import { v4 as uuidv4 } from 'uuid';
import { StorageProvider, UploadedFileResult } from './storage.interface';

@Injectable()
export class LocalStorageProvider implements StorageProvider {
  private readonly basePath: string;

  constructor(private config: ConfigService) {
    this.basePath = this.config.get<string>('STORAGE_LOCAL_PATH', './uploads');
  }

  async upload(
    file: Express.Multer.File,
    folder: string,
  ): Promise<UploadedFileResult> {
    const ext = file.originalname.split('.').pop() ?? 'bin';
    const key = `${folder}/${uuidv4()}.${ext}`;
    const fullPath = join(this.basePath, key);
    await mkdir(join(this.basePath, folder), { recursive: true });
    await writeFile(fullPath, file.buffer);
    return { key, url: this.getUrl(key) };
  }

  async delete(key: string): Promise<void> {
    try {
      await unlink(join(this.basePath, key));
    } catch {
      // ignore missing files
    }
  }

  getUrl(key: string): string {
    return `/uploads/${key}`;
  }
}
