import {
  BadRequestException,
  Injectable,
  UnauthorizedException,
} from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { JwtService } from '@nestjs/jwt';
import { UserRole, UserStatus } from '@prisma/client';
import * as bcrypt from 'bcrypt';
import { randomBytes } from 'crypto';
import { UsersRepository } from '../users/users.repository';
import { AuthRepository } from './auth.repository';
import { LoginDto } from './dto/login.dto';
import { RegisterDto } from './dto/register.dto';
import { ResetPasswordDto } from './dto/password-reset.dto';

const MAX_FAILED_ATTEMPTS = 5;
const LOCK_DURATION_MS = 15 * 60 * 1000;

@Injectable()
export class AuthService {
  constructor(
    private usersRepository: UsersRepository,
    private authRepository: AuthRepository,
    private jwtService: JwtService,
    private config: ConfigService,
  ) {}

  async register(dto: RegisterDto) {
    const existing = await this.usersRepository.findByEmail(dto.email);
    if (existing) {
      throw new BadRequestException('Email already registered');
    }

    const passwordHash = await bcrypt.hash(dto.password, 12);
    const emailVerificationToken = randomBytes(32).toString('hex');

    const user = await this.usersRepository.create({
      name: dto.name,
      email: dto.email,
      passwordHash,
      role: UserRole.CUSTOMER,
      status: UserStatus.PENDING_VERIFICATION,
      emailVerificationToken,
    });

    return {
      message: 'Registration successful. Please verify your email.',
      userId: user.id,
      verificationToken: emailVerificationToken,
    };
  }

  async verifyEmail(token: string) {
    const users = await this.usersRepository.findAll();
    const user = users.find((u) => u.emailVerificationToken === token);
    if (!user) {
      throw new BadRequestException('Invalid verification token');
    }
    await this.usersRepository.update(user.id, {
      emailVerified: true,
      status: UserStatus.ACTIVE,
      emailVerificationToken: null,
    });
    return { message: 'Email verified successfully' };
  }

  async login(dto: LoginDto) {
    const user = await this.usersRepository.findByEmail(dto.email);
    if (!user) {
      throw new UnauthorizedException('Invalid credentials');
    }

    if (user.lockedUntil && user.lockedUntil > new Date()) {
      throw new UnauthorizedException('Account temporarily locked');
    }

    const valid = await bcrypt.compare(dto.password, user.passwordHash);
    if (!valid) {
      const attempts = user.failedLoginAttempts + 1;
      const update: Record<string, unknown> = { failedLoginAttempts: attempts };
      if (attempts >= MAX_FAILED_ATTEMPTS) {
        update.lockedUntil = new Date(Date.now() + LOCK_DURATION_MS);
        update.failedLoginAttempts = 0;
      }
      await this.usersRepository.update(user.id, update);
      throw new UnauthorizedException('Invalid credentials');
    }

    await this.usersRepository.update(user.id, {
      failedLoginAttempts: 0,
      lockedUntil: null,
    });

    return this.issueTokens(user.id, user.email, user.role);
  }

  async refresh(refreshToken: string) {
    const session = await this.authRepository.findSessionByToken(refreshToken);
    if (!session || session.expiresAt < new Date()) {
      throw new UnauthorizedException('Invalid refresh token');
    }
    const user = await this.usersRepository.findById(session.userId);
    if (!user) {
      throw new UnauthorizedException('User not found');
    }
    await this.authRepository.deleteSession(session.id);
    return this.issueTokens(user.id, user.email, user.role);
  }

  async logout(refreshToken: string) {
    try {
      await this.authRepository.deleteSessionByToken(refreshToken);
    } catch {
      // session may already be gone
    }
    return { message: 'Logged out' };
  }

  async logoutAll(userId: string) {
    await this.authRepository.deleteAllUserSessions(userId);
    return { message: 'Logged out from all sessions' };
  }

  async requestPasswordReset(email: string) {
    const user = await this.usersRepository.findByEmail(email);
    if (!user) {
      return { message: 'If the email exists, a reset link was sent' };
    }
    const token = randomBytes(32).toString('hex');
    await this.usersRepository.update(user.id, {
      passwordResetToken: token,
      passwordResetExpires: new Date(Date.now() + 3600000),
    });
    return { message: 'If the email exists, a reset link was sent', token };
  }

  async resetPassword(dto: ResetPasswordDto) {
    const users = await this.usersRepository.findAll();
    const user = users.find(
      (u) =>
        u.passwordResetToken === dto.token &&
        u.passwordResetExpires &&
        u.passwordResetExpires > new Date(),
    );
    if (!user) {
      throw new BadRequestException('Invalid or expired reset token');
    }
    const passwordHash = await bcrypt.hash(dto.password, 12);
    await this.usersRepository.update(user.id, {
      passwordHash,
      passwordResetToken: null,
      passwordResetExpires: null,
    });
    await this.authRepository.deleteAllUserSessions(user.id);
    return { message: 'Password reset successful' };
  }

  private async issueTokens(userId: string, email: string, role: UserRole) {
    const payload = { sub: userId, email, role };
    const accessToken = await this.jwtService.signAsync(payload, {
      secret: this.config.get('JWT_ACCESS_SECRET'),
      expiresIn: this.config.get('JWT_ACCESS_EXPIRES', '15m'),
    });
    const refreshToken = randomBytes(48).toString('hex');
    const refreshExpires = this.config.get('JWT_REFRESH_EXPIRES', '7d');
    const days = parseInt(refreshExpires, 10) || 7;
    const expiresAt = new Date(Date.now() + days * 24 * 60 * 60 * 1000);

    await this.authRepository.createSession({
      userId,
      refreshToken,
      expiresAt,
    });

    return {
      accessToken,
      refreshToken,
      user: { id: userId, email, role },
    };
  }
}
