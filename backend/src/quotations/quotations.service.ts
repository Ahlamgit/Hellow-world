import {
  ForbiddenException,
  Injectable,
  NotFoundException,
} from '@nestjs/common';
import { QuotationStatus, UserRole } from '@prisma/client';
import { ProjectsRepository } from '../projects/projects.repository';
import { CreateQuotationDto, UpdateQuotationDto } from './dto/quotation.dto';
import { QuotationsRepository } from './quotations.repository';

@Injectable()
export class QuotationsService {
  constructor(
    private repository: QuotationsRepository,
    private projectsRepository: ProjectsRepository,
  ) {}

  async create(dto: CreateQuotationDto) {
    const project = await this.projectsRepository.findById(dto.projectId);
    if (!project) throw new NotFoundException('Project not found');

    const quotationNumber = `QT-${Date.now()}`;
    const totalAmount = dto.items.reduce(
      (sum, i) => sum + i.price * i.quantity,
      0,
    );

    const quotation = await this.repository.create({
      quotationNumber,
      totalAmount,
      validUntil: dto.validUntil ? new Date(dto.validUntil) : undefined,
      status: QuotationStatus.SENT,
      project: { connect: { id: dto.projectId } },
      items: { create: dto.items },
    });

    return quotation;
  }

  async findAll(userId: string, role: UserRole) {
    if (role === UserRole.ADMIN) {
      return this.repository.findAll();
    }
    const projects = await this.projectsRepository.findAll(userId);
    const projectIds = projects.map((p) => p.id);
    return this.repository.findAll(projectIds);
  }

  async findOne(id: string, userId: string, role: UserRole) {
    const quotation = await this.repository.findById(id);
    if (!quotation) throw new NotFoundException('Quotation not found');
    if (role !== UserRole.ADMIN && quotation.project.userId !== userId) {
      throw new ForbiddenException();
    }
    return quotation;
  }

  async update(
    id: string,
    dto: UpdateQuotationDto,
    userId: string,
    role: UserRole,
  ) {
    await this.findOne(id, userId, role);
    if (
      role !== UserRole.ADMIN &&
      dto.status &&
      dto.status !== QuotationStatus.ACCEPTED
    ) {
      throw new ForbiddenException('Customers can only accept quotations');
    }
    if (role === UserRole.CUSTOMER && dto.status === QuotationStatus.ACCEPTED) {
      return this.repository.update(id, { status: QuotationStatus.ACCEPTED });
    }
    return this.repository.update(id, {
      ...dto,
      validUntil: dto.validUntil ? new Date(dto.validUntil) : undefined,
    });
  }

  async remove(id: string) {
    return this.repository.delete(id);
  }
}
