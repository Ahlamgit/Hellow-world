'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { FormEvent, useState } from 'react';
import { useAuthStore } from '@/lib/auth-store';

export default function RegisterPage() {
  const router = useRouter();
  const { register, loading } = useAuthStore();
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError('');
    const form = new FormData(e.currentTarget);
    try {
      await register(
        form.get('name') as string,
        form.get('email') as string,
        form.get('password') as string,
      );
      setSuccess(true);
      setTimeout(() => router.push('/auth/login'), 2000);
    } catch {
      setError('Registration failed. Email may already be in use.');
    }
  }

  return (
    <div className="mx-auto flex min-h-[70vh] max-w-md flex-col justify-center px-6 py-12">
      <h1 className="font-serif text-3xl">Create account</h1>
      <p className="mt-2 text-stone-600">Join Trendy Interiors digital showroom</p>
      {success ? (
        <p className="mt-8 rounded-xl bg-green-50 p-4 text-green-800">
          Registration successful! Redirecting to login...
        </p>
      ) : (
        <form onSubmit={handleSubmit} className="mt-8 space-y-4">
          <input
            name="name"
            required
            placeholder="Full name"
            className="w-full rounded-xl border border-stone-300 px-4 py-3"
          />
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
            minLength={8}
            placeholder="Password (8+ chars, upper, lower, number)"
            className="w-full rounded-xl border border-stone-300 px-4 py-3"
          />
          {error && <p className="text-sm text-red-600">{error}</p>}
          <button
            type="submit"
            disabled={loading}
            className="w-full rounded-full bg-stone-900 py-3 text-white hover:bg-stone-800"
          >
            {loading ? 'Creating...' : 'Create Account'}
          </button>
        </form>
      )}
      <p className="mt-6 text-center text-sm">
        Already have an account?{' '}
        <Link href="/auth/login" className="text-amber-700">
          Login
        </Link>
      </p>
    </div>
  );
}
