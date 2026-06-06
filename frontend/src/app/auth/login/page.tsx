'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { FormEvent, useState } from 'react';
import { useAuthStore } from '@/lib/auth-store';

export default function LoginPage() {
  const router = useRouter();
  const { login, loading } = useAuthStore();
  const [error, setError] = useState('');

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError('');
    const form = new FormData(e.currentTarget);
    try {
      await login(
        form.get('email') as string,
        form.get('password') as string,
      );
      router.push('/projects');
    } catch {
      setError('Invalid email or password');
    }
  }

  return (
    <div className="mx-auto flex min-h-[70vh] max-w-md flex-col justify-center px-6 py-12">
      <h1 className="font-serif text-3xl">Welcome back</h1>
      <p className="mt-2 text-stone-600">Sign in to your Trendy Interiors account</p>
      <form onSubmit={handleSubmit} className="mt-8 space-y-4">
        <input
          name="email"
          type="email"
          required
          placeholder="Email"
          className="w-full rounded-xl border border-stone-300 px-4 py-3"
        />
        <input
          name="password"
          type="password"
          required
          placeholder="Password"
          className="w-full rounded-xl border border-stone-300 px-4 py-3"
        />
        {error && <p className="text-sm text-red-600">{error}</p>}
        <button
          type="submit"
          disabled={loading}
          className="w-full rounded-full bg-stone-900 py-3 text-white hover:bg-stone-800 disabled:opacity-50"
        >
          {loading ? 'Signing in...' : 'Sign In'}
        </button>
      </form>
      <p className="mt-6 text-center text-sm text-stone-500">
        Demo: customer@example.com / Customer@123
      </p>
      <p className="mt-2 text-center text-sm">
        No account?{' '}
        <Link href="/auth/register" className="text-amber-700">
          Register
        </Link>
      </p>
    </div>
  );
}
