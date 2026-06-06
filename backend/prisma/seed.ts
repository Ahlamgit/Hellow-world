import 'dotenv/config';
import { PrismaPg } from '@prisma/adapter-pg';
import { PrismaClient, UserRole, UserStatus } from '@prisma/client';
import * as bcrypt from 'bcrypt';
import { Pool } from 'pg';

const pool = new Pool({ connectionString: process.env.DATABASE_URL });
const adapter = new PrismaPg(pool);
const prisma = new PrismaClient({ adapter });

const CATEGORIES = [
  { name: 'Kitchens', slug: 'kitchens', description: 'Modern kitchen designs' },
  { name: 'Living Rooms', slug: 'living-rooms', description: 'Elegant living spaces' },
  { name: 'Salons', slug: 'salons', description: 'Luxury salon interiors' },
  { name: 'Bedrooms', slug: 'bedrooms', description: 'Comfortable bedroom designs' },
  { name: 'Bathrooms', slug: 'bathrooms', description: 'Spa-inspired bathrooms' },
  { name: 'Offices', slug: 'offices', description: 'Productive office spaces' },
  { name: 'Hotels', slug: 'hotels', description: 'Hospitality interiors' },
  { name: 'Restaurants', slug: 'restaurants', description: 'Dining experience design' },
];

async function main() {
  const adminHash = await bcrypt.hash('Admin@123456', 12);
  const customerHash = await bcrypt.hash('Customer@123', 12);

  await prisma.user.upsert({
    where: { email: 'admin@trendyinteriors.com' },
    update: {},
    create: {
      name: 'Platform Admin',
      email: 'admin@trendyinteriors.com',
      passwordHash: adminHash,
      role: UserRole.ADMIN,
      status: UserStatus.ACTIVE,
      emailVerified: true,
    },
  });

  await prisma.user.upsert({
    where: { email: 'customer@example.com' },
    update: {},
    create: {
      name: 'Demo Customer',
      email: 'customer@example.com',
      passwordHash: customerHash,
      role: UserRole.CUSTOMER,
      status: UserStatus.ACTIVE,
      emailVerified: true,
    },
  });

  for (const cat of CATEGORIES) {
    await prisma.category.upsert({
      where: { slug: cat.slug },
      update: cat,
      create: cat,
    });
  }

  const kitchen = await prisma.category.findUnique({ where: { slug: 'kitchens' } });
  if (kitchen) {
    const island = await prisma.furnitureItem.upsert({
      where: { id: '00000000-0000-4000-8000-000000000001' },
      update: {},
      create: {
        id: '00000000-0000-4000-8000-000000000001',
        categoryId: kitchen.id,
        name: 'Marble Kitchen Island',
        description: 'Premium marble-top island with storage',
        material: 'Marble & Oak',
        dimensions: '240x90x90 cm',
        price: 4500,
        vrInteractive: true,
      },
    });

    const cabinets = await prisma.furnitureItem.upsert({
      where: { id: '00000000-0000-4000-8000-000000000002' },
      update: {},
      create: {
        id: '00000000-0000-4000-8000-000000000002',
        categoryId: kitchen.id,
        name: 'Handleless Wall Cabinets',
        description: 'Matte finish wall-mounted cabinetry',
        material: 'MDF Matte',
        dimensions: '300x60x40 cm',
        price: 3200,
        vrInteractive: true,
      },
    });

    const pendant = await prisma.furnitureItem.upsert({
      where: { id: '00000000-0000-4000-8000-000000000003' },
      update: {},
      create: {
        id: '00000000-0000-4000-8000-000000000003',
        categoryId: kitchen.id,
        name: 'Brass Pendant Lights',
        description: 'Set of 3 brass pendant lights',
        material: 'Brass & Glass',
        dimensions: '30cm diameter each',
        price: 890,
        vrInteractive: true,
      },
    });

    await prisma.roomDesign.upsert({
      where: { id: '00000000-0000-4000-8000-000000000010' },
      update: {},
      create: {
        id: '00000000-0000-4000-8000-000000000010',
        categoryId: kitchen.id,
        title: 'Contemporary Open Kitchen',
        description: 'Open-plan kitchen with island and premium finishes',
        coverImage: '/images/kitchen-cover.jpg',
        vrEnabled: true,
        vrSceneId: 'kitchen-modern',
      },
    });

    const vrScene = await prisma.vrScene.upsert({
      where: { slug: 'kitchen-modern' },
      update: {},
      create: {
        slug: 'kitchen-modern',
        name: 'Modern Kitchen VR Showroom',
        categorySlug: 'kitchens',
        hdrUrl: 'https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/1k/studio_small_09_1k.hdr',
      },
    });

    for (const [furniture, pos] of [
      [island, { x: 0, y: 0, z: 0 }],
      [cabinets, { x: -3, y: 1.5, z: -2 }],
      [pendant, { x: 0, y: 2.5, z: 0 }],
    ] as const) {
      await prisma.vrSceneFurniture.upsert({
        where: {
          vrSceneId_furnitureId: {
            vrSceneId: vrScene.id,
            furnitureId: furniture.id,
          },
        },
        update: {},
        create: {
          vrSceneId: vrScene.id,
          furnitureId: furniture.id,
          positionX: pos.x,
          positionY: pos.y,
          positionZ: pos.z,
        },
      });
    }
  }

  console.log('Seed completed: admin@trendyinteriors.com / Admin@123456');
  console.log('Demo customer: customer@example.com / Customer@123');
}

main()
  .catch(console.error)
  .finally(async () => {
    await prisma.$disconnect();
    await pool.end();
  });
