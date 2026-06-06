export interface UploadedFileResult {
  key: string;
  url: string;
  thumbnailUrl?: string;
}

export interface StorageProvider {
  upload(
    file: Express.Multer.File,
    folder: string,
  ): Promise<UploadedFileResult>;
  delete(key: string): Promise<void>;
  getUrl(key: string): string;
}

export const STORAGE_PROVIDER = 'STORAGE_PROVIDER';
