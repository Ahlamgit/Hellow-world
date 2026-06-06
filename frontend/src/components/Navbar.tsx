'use client';

import Link from 'next/link';
import { useEffect } from 'react';
import { useAuthStore } from '@/lib/auth-store';

export function Navbar() {
  const { user, logout, hydrate } = useAuthStore();

  useEffect(() => {
    hydrate();
  }, [hydrate]);

  return (
    <header className="sticky top-0 z-50 border-b border-stone-200/80 bg-white/90 backdrop-blur-md">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-6 py-4">
        <Link href="/" className="font-serif text-2xl tracking-tight text-stone-900">
          Trendy <span className="text-amber-700">Interiors</span>
        </Link>
        <nav className="hidden items-center gap-8 text-sm font-medium text-stone-600 md:flex">
          <Link href="/showroom" className="hover:text-amber-700">
            Showroom
          </Link>
          <Link href="/furniture" className="hover:text-amber-700">
            Furniture
          </Link>
          <Link href="/projects" className="hover:text-amber-700">
            Projects
          </Link>
          <Link href="/showroom/kitchens/vr" className="hover:text-amber-700">
            VR Kitchen
          </Link>
          {user?.role === 'ADMIN' && (
            <Link href="/admin" className="hover:text-amber-700">
              Admin
            </Link>
          )}
        </nav>
        <div className="flex items-center gap-3">
          {user ? (
            <>
              <span className="hidden text-sm text-stone-500 sm:inline">
                {user.email}
              </span>
              <button
                onClick={logout}
                className="rounded-full border border-stone-300 px-4 py-2 text-sm hover:bg-stone-50"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <Link
                href="/auth/login"
                className="rounded-full px-4 py-2 text-sm text-stone-700 hover:bg-stone-100"
              >
                Login
              </Link>
              <Link
                href="/auth/register"
                className="rounded-full bg-stone-900 px-4 py-2 text-sm text-white hover:bg-stone-800"
              >
                Register
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
