export type ServiceCategory = 'plumbing' | 'electrical' | 'cleaning' | 'ac' | 'painting' | 'moving';

export type SampleService = {
  id: string;
  category: ServiceCategory;
  titleEn: string;
  titleAr: string;
  providerEn: string;
  providerAr: string;
  rating: number;
  reviewCount: number;
  verified: boolean;
  descriptionEn: string;
  descriptionAr: string;
};

export const categories: ServiceCategory[] = ['plumbing', 'electrical', 'cleaning', 'ac', 'painting', 'moving'];

export const sampleServices: SampleService[] = [
  {
    id: 'plumb-001',
    category: 'plumbing',
    titleEn: 'Emergency pipe repair',
    titleAr: 'إصلاح أنابيب طارئ',
    providerEn: 'Beirut Plumbing Co.',
    providerAr: 'شركة بيروت للسباكة',
    rating: 4.8,
    reviewCount: 124,
    verified: true,
    descriptionEn: 'Fast response for leaks, clogs, and pipe repairs across Greater Beirut.',
    descriptionAr: 'استجابة سريعة للتسريبات والانسدادات وإصلاح الأنابيب في بيروت الكبرى.',
  },
  {
    id: 'elec-001',
    category: 'electrical',
    titleEn: 'Home electrical inspection',
    titleAr: 'فحص كهرباء المنزل',
    providerEn: 'Ahmed Electric Services',
    providerAr: 'خدمات أحمد للكهرباء',
    rating: 4.9,
    reviewCount: 89,
    verified: true,
    descriptionEn: 'Safety checks, wiring fixes, and panel upgrades by certified electricians.',
    descriptionAr: 'فحوصات السلامة وإصلاح التمديدات وترقية اللوحات من كهربائيين معتمدين.',
  },
  {
    id: 'clean-001',
    category: 'cleaning',
    titleEn: 'Deep home cleaning',
    titleAr: 'تنظيف عميق للمنزل',
    providerEn: 'Sparkle Home Care',
    providerAr: 'سباركل لرعاية المنزل',
    rating: 4.7,
    reviewCount: 210,
    verified: true,
    descriptionEn: 'Thorough cleaning for apartments and houses — eco-friendly products available.',
    descriptionAr: 'تنظيف شامل للشقق والمنازل — منتجات صديقة للبيئة متوفرة.',
  },
  {
    id: 'ac-001',
    category: 'ac',
    titleEn: 'AC maintenance & gas refill',
    titleAr: 'صيانة مكيف وتعبئة غاز',
    providerEn: 'CoolAir Lebanon',
    providerAr: 'كول إير لبنان',
    rating: 4.6,
    reviewCount: 156,
    verified: true,
    descriptionEn: 'Seasonal AC service, filter replacement, and cooling performance checks.',
    descriptionAr: 'صيانة موسمية للمكيفات واستبدال الفلاتر وفحص الأداء.',
  },
  {
    id: 'paint-001',
    category: 'painting',
    titleEn: 'Interior wall painting',
    titleAr: 'دهان جدران داخلية',
    providerEn: 'ColorPro Studios',
    providerAr: 'استوديوهات كولور برو',
    rating: 4.5,
    reviewCount: 67,
    verified: false,
    descriptionEn: 'Professional prep and finish for living rooms, bedrooms, and offices.',
    descriptionAr: 'تحضير واحترافية في التشطيب لغرف المعيشة والنوم والمكاتب.',
  },
  {
    id: 'move-001',
    category: 'moving',
    titleEn: 'Apartment moving service',
    titleAr: 'خدمة نقل شقة',
    providerEn: 'EasyMove Beirut',
    providerAr: 'إيزي موف بيروت',
    rating: 4.4,
    reviewCount: 98,
    verified: true,
    descriptionEn: 'Packing, transport, and setup — team handles furniture with care.',
    descriptionAr: 'تعبئة ونقل وتركيب — فريق يتعامل مع الأثاث بعناية.',
  },
];

export function getServiceById(id: string): SampleService | undefined {
  return sampleServices.find((s) => s.id === id);
}

export function filterServices(query: string, category?: ServiceCategory | 'all'): SampleService[] {
  const q = query.trim().toLowerCase();
  return sampleServices.filter((s) => {
    const matchCategory = !category || category === 'all' || s.category === category;
    const matchQuery =
      !q ||
      s.titleEn.toLowerCase().includes(q) ||
      s.titleAr.includes(q) ||
      s.providerEn.toLowerCase().includes(q);
    return matchCategory && matchQuery;
  });
}
