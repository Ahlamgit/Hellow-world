# Trendy Interiors Digital Showroom Platform

Production-ready full-stack platform for an interior design and furniture company. Customers browse showrooms, explore VR experiences, build projects, request customization, receive quotations, and make payments.

## Stack

| Layer | Technology |
|-------|------------|
| Frontend | Next.js 15, TypeScript, Tailwind CSS, React Three Fiber |
| Backend | NestJS, TypeScript, Prisma ORM |
| Database | PostgreSQL |
| Auth | JWT + refresh tokens, bcrypt |
| Payments | Stripe + PayPal (provider abstraction) |
| Storage | Local + AWS S3-compatible abstraction |
| Deployment | Docker Compose |

## Quick Start (Local)

### Prerequisites

- Node.js 20+
- PostgreSQL 16+

### 1. Database

```bash
# Create database user and DB (or use Docker Compose)
createdb trendy_interiors
```

Copy environment files:

```bash
cp .env.example backend/.env
cp .env.example frontend/.env.local
# Set DATABASE_URL and NEXT_PUBLIC_API_URL in each
```

### 2. Backend

```bash
cd backend
npm install
npx prisma migrate dev
npm run prisma:seed
npm run start:dev
```

API: http://localhost:4000/api  
Swagger: http://localhost:4000/api/docs

### 3. Frontend

```bash
cd frontend
npm install
npm run dev
```

App: http://localhost:3000

### Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@trendyinteriors.com | Admin@123456 |
| Customer | customer@example.com | Customer@123 |

## Docker

```bash
docker compose up --build
```

## Project Structure

```
backend/          NestJS API (modules, repositories, services, guards)
frontend/         Next.js App Router UI + VR showroom
docker-compose.yml
```

## Key Routes

| Route | Description |
|-------|-------------|
| `/showroom` | Category browser |
| `/showroom/kitchens/vr` | Kitchen VR showroom (Three.js) |
| `/furniture` | Furniture catalog |
| `/projects` | Customer project builder |
| `/admin` | Admin analytics dashboard |
| `/api/docs` | Swagger API documentation |

## License

Private — Trendy Interiors
