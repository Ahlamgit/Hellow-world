import Link from 'next/link';

export default function BedroomVrPage() {
  return (
    <div className="mx-auto max-w-2xl px-6 py-20 text-center">
      <h1 className="font-serif text-3xl">Bedroom VR Showroom</h1>
      <p className="mt-4 text-stone-600">
        Architecture supports additional VR showrooms. Bedroom VR content can be
        added via the admin VR module without changing core business logic.
      </p>
      <Link href="/showroom/bedrooms" className="mt-6 inline-block text-amber-700">
        Browse bedroom designs
      </Link>
    </div>
  );
}
