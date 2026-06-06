import Link from 'next/link';

const features = [
  {
    title: 'Digital Showroom',
    desc: 'Browse complete room designs across kitchens, bedrooms, offices, and more.',
    href: '/showroom',
  },
  {
    title: 'VR Experience',
    desc: 'Walk through immersive 3D showrooms and explore furniture in context.',
    href: '/showroom/kitchens/vr',
  },
  {
    title: 'Project Builder',
    desc: 'Create projects, add designs and furniture, and request quotations.',
    href: '/projects',
  },
  {
    title: 'Customization',
    desc: 'Request material and color changes with reference uploads.',
    href: '/furniture',
  },
];

export default function HomePage() {
  return (
    <div>
      <section className="relative overflow-hidden bg-gradient-to-br from-stone-900 via-stone-800 to-amber-900 text-white">
        <div className="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1618221195710-dd6b41faaea6?w=1600&q=80')] bg-cover bg-center opacity-20" />
        <div className="relative mx-auto max-w-7xl px-6 py-28 md:py-36">
          <p className="mb-4 text-sm uppercase tracking-[0.3em] text-amber-200">
            Digital Showroom Platform
          </p>
          <h1 className="font-serif text-5xl leading-tight md:text-7xl">
            Trendy Interiors
          </h1>
          <p className="mt-6 max-w-2xl text-lg text-stone-200">
            Complete interior solutions for homes, offices, hotels, restaurants,
            and commercial establishments. Browse, customize, quote, and pay —
            all in one platform.
          </p>
          <div className="mt-10 flex flex-wrap gap-4">
            <Link
              href="/showroom"
              className="rounded-full bg-amber-600 px-8 py-3 font-medium text-white hover:bg-amber-500"
            >
              Explore Showroom
            </Link>
            <Link
              href="/showroom/kitchens/vr"
              className="rounded-full border border-white/40 px-8 py-3 font-medium hover:bg-white/10"
            >
              Enter VR Kitchen
            </Link>
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-6 py-20">
        <h2 className="font-serif text-3xl text-stone-900">Platform Features</h2>
        <div className="mt-10 grid gap-6 md:grid-cols-2 lg:grid-cols-4">
          {features.map((f) => (
            <Link
              key={f.title}
              href={f.href}
              className="group rounded-2xl border border-stone-200 bg-white p-6 shadow-sm transition hover:border-amber-300 hover:shadow-md"
            >
              <h3 className="text-lg font-semibold text-stone-900 group-hover:text-amber-700">
                {f.title}
              </h3>
              <p className="mt-2 text-sm text-stone-600">{f.desc}</p>
            </Link>
          ))}
        </div>
      </section>

      <section className="bg-stone-100 py-20">
        <div className="mx-auto max-w-7xl px-6 text-center">
          <h2 className="font-serif text-3xl">Ready to design your space?</h2>
          <p className="mx-auto mt-4 max-w-xl text-stone-600">
            Create an account to save favorites, build projects, receive
            quotations, and track progress.
          </p>
          <Link
            href="/auth/register"
            className="mt-8 inline-block rounded-full bg-stone-900 px-8 py-3 text-white hover:bg-stone-800"
          >
            Get Started
          </Link>
        </div>
      </section>
    </div>
  );
}
