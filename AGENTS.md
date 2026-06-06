# AGENTS.md

## Cursor Cloud specific instructions

Full-stack monorepo: **Trendy Interiors Digital Showroom Platform**.

### Services (must run for E2E)

| Service | Port | Command |
|---------|------|---------|
| PostgreSQL | 5432 | `sudo pg_ctlcluster 16 main start` (if not running) |
| Backend (NestJS) | 4000 | `cd backend && npm run start:dev` |
| Frontend (Next.js) | 3000 | `cd frontend && npm run dev` |

Docker is not available in all Cloud Agent VMs; PostgreSQL was installed via apt when needed.

### First-time / after schema changes

```bash
cd backend
npx prisma migrate dev
npm run prisma:seed
```

### Lint / test / build

```bash
cd backend && npm run lint && npm run build
cd frontend && npm run lint && npm run build
```

### Environment

- `backend/.env` — copy from root `.env.example`; requires `DATABASE_URL`, JWT secrets
- `frontend/.env.local` — `NEXT_PUBLIC_API_URL=http://localhost:4000/api`
- Prisma 7 uses `@prisma/adapter-pg` — `PrismaClient` must be constructed with a `pg` Pool adapter (see `backend/src/prisma/prisma.service.ts`)

### Demo credentials

- Admin: `admin@trendyinteriors.com` / `Admin@123456`
- Customer: `customer@example.com` / `Customer@123`

### VR showroom

Kitchen VR at `/showroom/kitchens/vr` loads scene data from `GET /api/vr/category/kitchens`. Requires backend running and seeded.

### API docs

Swagger at http://localhost:4000/api/docs when backend is up.
