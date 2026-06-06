import {
  ForbiddenException,
  Injectable,
  NotFoundException,
} from '@nestjs/common';
import { UserRole } from '@prisma/client';
import { UpdateUserDto } from './dto/update-user.dto';
import { UsersRepository } from './users.repository';

@Injectable()
export class UsersService {
  constructor(private repository: UsersRepository) {}

  findAll() {
    return this.repository.findAll();
  }

  async findById(id: string, requesterId: string, requesterRole: UserRole) {
    if (requesterRole !== UserRole.ADMIN && requesterId !== id) {
      throw new ForbiddenException();
    }
    const user = await this.repository.findById(id);
    if (!user) throw new NotFoundException('User not found');
    const { passwordHash: _pw, ...safe } = user;
    return safe;
  }

  async update(
    id: string,
    dto: UpdateUserDto,
    requesterId: string,
    requesterRole: UserRole,
  ) {
    if (requesterRole !== UserRole.ADMIN && requesterId !== id) {
      throw new ForbiddenException();
    }
    const user = await this.repository.update(id, dto);
    const { passwordHash: _pw, ...safe } = user;
    return safe;
  }

  async remove(id: string) {
    await this.repository.delete(id);
    return { deleted: true };
  }
}
