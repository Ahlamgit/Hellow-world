import Link from 'next/link';
import { api, Category, FurnitureItem, RoomDesign } from '@/lib/api';

interface Props {
  params: Promise<{ slug: string }>;
}

export default async function CategoryShowroomPage({ params }: Props) {
  const { slug } = await params;
  let category: Category & {
    roomDesigns?: RoomDesign[];
    furnitureItems?: FurnitureItem[];
  } | null = null;

  try {
    const { data } = await api.get(`/categories/${slug}`);
    category = data;
  } catch {
    category = null;
  }

  if (!category) {
    return (
      <div className="mx-auto max-w-7xl px-6 py-20 text-center">
        <h1 className="text-2xl">Category not found</h1>
        <Link href="/showroom" className="mt-4 inline-block text-amber-700">
          Back to showroom
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-7xl px-6 py-12">
      <div className="flex flex-wrap items-end justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-widest text-amber-700">
            Showroom
          </p>
          <h1 className="font-serif text-4xl">{category.name}</h1>
          <p className="mt-2 text-stone-600">{category.description}</p>
        </div>
        {slug === 'kitchens' && (
          <Link
            href="/showroom/kitchens/vr"
            className="rounded-full bg-amber-600 px-6 py-3 text-white hover:bg-amber-500"
          >
            Enter VR Showroom
          </Link>
        )}
      </div>

      <section className="mt-14">
        <h2 className="text-xl font-semibold">Room Designs</h2>
        <div className="mt-6 grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {(category.roomDesigns ?? []).map((design) => (
            <article
              key={design.id}
              className="rounded-2xl border border-stone-200 bg-white p-6"
            >
              <h3 className="text-lg font-medium">{design.title}</h3>
              <p className="mt-2 text-sm text-stone-600">{design.description}</p>
              {design.vrEnabled && (
                <span className="mt-3 inline-block rounded-full bg-amber-100 px-3 py-1 text-xs text-amber-800">
                  VR Enabled
                </span>
              )}
            </article>
          ))}
          {!category.roomDesigns?.length && (
            <p className="text-stone-500">No room designs yet.</p>
          )}
        </div>
      </section>

      <section className="mt-14">
        <h2 className="text-xl font-semibold">Furniture Collection</h2>
        <div className="mt-6 grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {(category.furnitureItems ?? []).map((item) => (
            <article
              key={item.id}
              className="rounded-2xl border border-stone-200 bg-white p-6"
            >
              <h3 className="text-lg font-medium">{item.name}</h3>
              <p className="mt-1 text-sm text-stone-600">{item.material}</p>
              <p className="mt-3 text-lg font-semibold text-amber-800">
                ${Number(item.price).toLocaleString()}
              </p>
            </article>
          ))}
        </div>
      </section>
    </div>
  );
}
