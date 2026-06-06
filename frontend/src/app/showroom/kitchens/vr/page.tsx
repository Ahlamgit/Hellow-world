import Link from 'next/link';
import { KitchenVrShowroom } from '@/components/vr/KitchenVrShowroom';
import { api, VrScene } from '@/lib/api';

async function getKitchenScene(): Promise<VrScene | null> {
  try {
    const { data } = await api.get('/vr/category/kitchens');
    return data;
  } catch {
    return null;
  }
}

export default async function KitchenVrPage() {
  const scene = await getKitchenScene();

  if (!scene) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center px-6">
        <h1 className="text-2xl">VR scene unavailable</h1>
        <p className="mt-2 text-stone-600">Ensure the API is running and seeded.</p>
        <Link href="/showroom/kitchens" className="mt-4 text-amber-700">
          Back to kitchens
        </Link>
      </div>
    );
  }

  return <KitchenVrShowroom scene={scene} />;
}
