'use client';

import Link from 'next/link';
import { useCallback, useEffect, useState } from 'react';
import { api } from '@/lib/api';
import { useAuthStore } from '@/lib/auth-store';

interface DashboardStats {
  users: { total: number; customers: number };
  projects: { total: number; active: number };
  quotations: { total: number; pending: number };
  payments: { total: number; paid: number; revenue: number };
  customizationPending: number;
  vrScenes: number;
  furnitureCount: number;
}

export default function AdminPage() {
  const { user, hydrate } = useAuthStore();
  const [stats, setStats] = useState<DashboardStats | null>(null);

  const load = useCallback(async () => {
    if (user?.role !== 'ADMIN') return;
    try {
      const { data } = await api.get('/analytics/dashboard');
      setStats(data);
    } catch {
      setStats(null);
    }
  }, [user]);

  useEffect(() => {
    hydrate();
  }, [hydrate]);

  useEffect(() => {
    load();
  }, [load]);

  if (!user) {
    return (
      <div className="mx-auto max-w-7xl px-6 py-20 text-center">
        <Link href="/auth/login" className="text-amber-700">
          Sign in as admin
        </Link>
      </div>
    );
  }

  if (user.role !== 'ADMIN') {
    return (
      <div className="mx-auto max-w-7xl px-6 py-20 text-center">
        <h1 className="text-2xl">Access Denied</h1>
        <p className="mt-2 text-stone-600">Admin privileges required.</p>
      </div>
    );
  }

  const cards = stats
    ? [
        { label: 'Total Users', value: stats.users.total },
        { label: 'Customers', value: stats.users.customers },
        { label: 'Projects', value: stats.projects.total },
        { label: 'Active Projects', value: stats.projects.active },
        { label: 'Quotations', value: stats.quotations.total },
        { label: 'Pending Quotes', value: stats.quotations.pending },
        { label: 'Payments', value: stats.payments.total },
        { label: 'Revenue', value: `$${stats.payments.revenue.toLocaleString()}` },
        { label: 'Customization Queue', value: stats.customizationPending },
        { label: 'VR Scenes', value: stats.vrScenes },
        { label: 'Furniture Items', value: stats.furnitureCount },
      ]
    : [];

  return (
    <div className="mx-auto max-w-7xl px-6 py-12">
      <h1 className="font-serif text-4xl">Admin Dashboard</h1>
      <p className="mt-2 text-stone-600">
        Manage users, catalog, quotations, payments, and VR content.
      </p>
      <p className="mt-2 text-sm text-stone-500">
        Demo admin: admin@trendyinteriors.com / Admin@123456
      </p>

      <div className="mt-10 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {cards.map((c) => (
          <div
            key={c.label}
            className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm"
          >
            <p className="text-sm text-stone-500">{c.label}</p>
            <p className="mt-2 text-3xl font-semibold text-stone-900">{c.value}</p>
          </div>
        ))}
      </div>

      <div className="mt-12 grid gap-4 md:grid-cols-3">
        {[
          'Users',
          'Categories',
          'Room Designs',
          'Furniture',
          'Projects',
          'Quotations',
          'Payments',
          'VR Content',
        ].map((section) => (
          <div
            key={section}
            className="rounded-xl border border-dashed border-stone-300 p-6 text-stone-600"
          >
            <h3 className="font-medium text-stone-900">{section}</h3>
            <p className="mt-2 text-sm">
              Manage via REST API at{' '}
              <code className="rounded bg-stone-100 px-1">/api/{section.toLowerCase().replace(' ', '-')}</code>
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}
