import Link from 'next/link';

export default function BathroomVrPage() {
  return (
    <div className="mx-auto max-w-2xl px-6 py-20 text-center">
      <h1 className="font-serif text-3xl">Bathroom VR Showroom</h1>
      <p className="mt-4 text-stone-600">
        Extendable VR module — add scenes through the VrScene database table.
      </p>
      <Link href="/showroom/bathrooms" className="mt-6 inline-block text-amber-700">
        Browse bathroom designs
      </Link>
    </div>
  );
}
