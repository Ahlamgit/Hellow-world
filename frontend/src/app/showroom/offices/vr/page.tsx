import Link from 'next/link';

export default function OfficeVrPage() {
  return (
    <div className="mx-auto max-w-2xl px-6 py-20 text-center">
      <h1 className="font-serif text-3xl">Office VR Showroom</h1>
      <p className="mt-4 text-stone-600">
        Future VR showroom slot — isolated from auth, payments, and quotations.
      </p>
      <Link href="/showroom/offices" className="mt-6 inline-block text-amber-700">
        Browse office designs
      </Link>
    </div>
  );
}
