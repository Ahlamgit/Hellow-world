'use client';

import Link from 'next/link';
import { FormEvent, useCallback, useEffect, useState } from 'react';
import { api } from '@/lib/api';
import { useAuthStore } from '@/lib/auth-store';

interface Project {
  id: string;
  title: string;
  status: string;
  projectRooms: unknown[];
  projectItems: unknown[];
  quotations: unknown[];
}

export default function ProjectsPage() {
  const { user, hydrate } = useAuthStore();
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const { data } = await api.get('/projects');
      setProjects(data);
    } catch {
      setProjects([]);
    } finally {
      setLoading(false);
    }
  }, [user]);

  useEffect(() => {
    hydrate();
  }, [hydrate]);

  useEffect(() => {
    load();
  }, [load]);

  async function createProject(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const title = new FormData(e.currentTarget).get('title') as string;
    await api.post('/projects', { title });
    e.currentTarget.reset();
    load();
  }

  if (!user) {
    return (
      <div className="mx-auto max-w-7xl px-6 py-20 text-center">
        <h1 className="font-serif text-3xl">Project Builder</h1>
        <p className="mt-4 text-stone-600">
          Please{' '}
          <Link href="/auth/login" className="text-amber-700">
            sign in
          </Link>{' '}
          to create and manage projects.
        </p>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-7xl px-6 py-12">
      <h1 className="font-serif text-4xl">My Projects</h1>
      <p className="mt-2 text-stone-600">
        Create projects, add room designs and furniture, then request quotations.
      </p>

      <form
        onSubmit={createProject}
        className="mt-8 flex flex-wrap gap-3 rounded-2xl border border-stone-200 bg-white p-6"
      >
        <input
          name="title"
          required
          placeholder="New project title"
          className="min-w-[240px] flex-1 rounded-xl border border-stone-300 px-4 py-2"
        />
        <button
          type="submit"
          className="rounded-full bg-amber-600 px-6 py-2 text-white hover:bg-amber-500"
        >
          Create Project
        </button>
      </form>

      {loading ? (
        <p className="mt-10 text-stone-500">Loading projects...</p>
      ) : (
        <div className="mt-10 grid gap-4">
          {projects.map((p) => (
            <article
              key={p.id}
              className="rounded-2xl border border-stone-200 bg-white p-6"
            >
              <div className="flex flex-wrap items-center justify-between gap-2">
                <h2 className="text-xl font-semibold">{p.title}</h2>
                <span className="rounded-full bg-stone-100 px-3 py-1 text-xs uppercase">
                  {p.status}
                </span>
              </div>
              <p className="mt-2 text-sm text-stone-500">
                {p.projectRooms?.length ?? 0} rooms ·{' '}
                {p.projectItems?.length ?? 0} items ·{' '}
                {p.quotations?.length ?? 0} quotations
              </p>
            </article>
          ))}
          {!projects.length && (
            <p className="text-stone-500">No projects yet. Create your first one above.</p>
          )}
        </div>
      )}
    </div>
  );
}
