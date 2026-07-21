import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { locationsApi, type City, type Region } from '../services/locationsApi';

export function useLocationCatalog() {
  const { i18n } = useTranslation();
  const [regions, setRegions] = useState<Region[]>([]);
  const [cities, setCities] = useState<City[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const regionLabel = (region: Region) => (i18n.language === 'ar' ? region.nameAr : region.nameEn);
  const cityLabel = (city: City) => (i18n.language === 'ar' ? city.nameAr : city.nameEn);

  useEffect(() => {
    (async () => {
      setLoading(true);
      setError('');
      try {
        const { data } = await locationsApi.listRegions();
        setRegions(data.data.items);
      } catch {
        setError('Failed to load regions');
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const loadCities = async (regionId?: string) => {
    if (!regionId) {
      setCities([]);
      return [];
    }
    try {
      const { data } = await locationsApi.listCities(regionId);
      setCities(data.data.items);
      return data.data.items;
    } catch {
      setCities([]);
      return [];
    }
  };

  const findRegionByName = (name?: string) =>
    regions.find((r) => r.nameEn === name || r.nameAr === name || r.code === name);

  const findRegionById = (id?: string) => regions.find((r) => r.id === id);

  return {
    regions,
    cities,
    loading,
    error,
    regionLabel,
    cityLabel,
    loadCities,
    findRegionByName,
    findRegionById,
  };
}
