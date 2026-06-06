import { api, FurnitureItem } from '@/lib/api';

async function getFurniture(): Promise<FurnitureItem[]> {
  try {
    const { data } = await api.get('/furniture');
    return data;
  } catch {
    return [];
  }
}

export default async function FurniturePage() {
  const items = await getFurniture();

  return (
    <div className="mx-auto max-w-7xl px-6 py-12">
      <h1 className="font-serif text-4xl">Furniture Catalog</h1>
      <p className="mt-3 text-stone-600">
        Browse our curated furniture collection. Log in to save favorites and
        request customization.
      </p>
      <div className="mt-12 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {items.map((item) => (
          <article
            key={item.id}
            className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm"
          >
            <div className="mb-4 h-32 rounded-xl bg-gradient-to-br from-stone-100 to-amber-50" />
            <h2 className="text-lg font-semibold">{item.name}</h2>
            <p className="mt-1 text-sm text-stone-600">{item.description}</p>
            <dl className="mt-4 space-y-1 text-sm text-stone-500">
              {item.material && (
                <div className="flex justify-between">
                  <dt>Material</dt>
                  <dd>{item.material}</dd>
                </div>
              )}
              {item.dimensions && (
                <div className="flex justify-between">
                  <dt>Dimensions</dt>
                  <dd>{item.dimensions}</dd>
                </div>
              )}
            </dl>
            <p className="mt-4 text-xl font-semibold text-amber-800">
              ${Number(item.price).toLocaleString()}
            </p>
            {item.vrInteractive && (
              <span className="mt-2 inline-block text-xs text-amber-700">
                Interactive in VR
              </span>
            )}
          </article>
        ))}
      </div>
    </div>
  );
}
