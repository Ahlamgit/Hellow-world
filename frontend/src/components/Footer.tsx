import Link from 'next/link';

export function Footer() {
  return (
    <footer className="mt-auto border-t border-stone-200 bg-stone-50">
      <div className="mx-auto flex max-w-7xl flex-col gap-4 px-6 py-10 text-sm text-stone-500 md:flex-row md:items-center md:justify-between">
        <p>© {new Date().getFullYear()} Trendy Interiors. Complete interior solutions.</p>
        <div className="flex gap-6">
          <Link href="/showroom">Showroom</Link>
          <Link href="/furniture">Furniture</Link>
          <Link href="/projects">Projects</Link>
        </div>
      </div>
    </footer>
  );
}
