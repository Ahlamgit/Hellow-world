import { Injectable } from '@nestjs/common';
import { Session } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

@Injectable()
export class AuthRepository {
  constructor(private prisma: PrismaService) {}

  createSession(data: {
    userId: string;
    refreshToken: string;
    expiresAt: Date;
  }): Promise<Session> {
    return this.prisma.session.create({ data });
  }

  findSessionByToken(refreshToken: string): Promise<Session | null> {
    return this.prisma.session.findUnique({ where: { refreshToken } });
  }

  deleteSession(id: string): Promise<Session> {
    return this.prisma.session.delete({ where: { id } });
  }

  deleteSessionByToken(refreshToken: string): Promise<Session> {
    return this.prisma.session.delete({ where: { refreshToken } });
  }

  deleteAllUserSessions(userId: string): Promise<{ count: number }> {
    return this.prisma.session.deleteMany({ where: { userId } });
  }
}
