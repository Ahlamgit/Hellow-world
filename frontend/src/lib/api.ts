import axios from 'axios';
import Cookies from 'js-cookie';

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:4000/api';

export const api = axios.create({
  baseURL: API_URL,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = Cookies.get('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  _count?: { roomDesigns: number; furnitureItems: number };
}

export interface FurnitureItem {
  id: string;
  name: string;
  description?: string;
  material?: string;
  dimensions?: string;
  price: string | number;
  imageUrl?: string;
  model3dUrl?: string;
  vrInteractive: boolean;
  category?: Category;
}

export interface RoomDesign {
  id: string;
  title: string;
  description?: string;
  coverImage?: string;
  vrEnabled: boolean;
  vrSceneId?: string;
  category?: Category;
}

export interface VrScene {
  slug: string;
  name: string;
  categorySlug: string;
  hdrUrl?: string;
  furnitureItems: Array<
    FurnitureItem & { position: { x: number; y: number; z: number } }
  >;
}
