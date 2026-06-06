import Link from 'next/link';
import { api, Category } from '@/lib/api';

async function getCategories(): Promise<Category[]> {
  try {
    const { data } = await api.get('/categories');
    return data;
  } catch {
    return [];
  }
}

export default async function ShowroomPage() {
  const categories = await getCategories();

  return (
    <div className="mx-auto max-w-7xl px-6 py-12">
      <h1 className="font-serif text-4xl text-stone-900">Digital Showroom</h1>
      <p className="mt-3 max-w-2xl text-stone-600">
        Explore complete room designs and furniture collections by category.
      </p>
      <div className="mt-12 grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
        {categories.map((cat) => (
          <Link
            key={cat.id}
            href={`/showroom/${cat.slug}`}
            className="group overflow-hidden rounded-2xl border border-stone-200 bg-white shadow-sm transition hover:shadow-lg"
          >
            <div className="flex h-40 items-end bg-gradient-to-br from-stone-200 to-amber-100 p-6">
              <h2 className="font-serif text-2xl text-stone-900 group-hover:text-amber-800">
                {cat.name}
              </h2>
            </div>
            <div className="p-4 text-sm text-stone-500">
              <p>{cat.description}</p>
              <p className="mt-2">
                {cat._count?.roomDesigns ?? 0} designs ·{' '}
                {cat._count?.furnitureItems ?? 0} items
              </p>
              {cat.slug === 'kitchens' && (
                <span className="mt-2 inline-block text-amber-700">
                  VR available →
                </span>
              )}
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
