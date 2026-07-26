export type ActorType = 'customer' | 'craftsman' | 'store' | 'admin';

export const ACTOR_TYPES: ActorType[] = ['customer', 'craftsman', 'store', 'admin'];

export const ACTOR_STORAGE_KEY = 'khadamati_actor';

export function saveActor(actor: ActorType): void {
  sessionStorage.setItem(ACTOR_STORAGE_KEY, actor);
}

export function loadActor(): ActorType | null {
  const value = sessionStorage.getItem(ACTOR_STORAGE_KEY);
  return ACTOR_TYPES.includes(value as ActorType) ? (value as ActorType) : null;
}

export function clearActor(): void {
  sessionStorage.removeItem(ACTOR_STORAGE_KEY);
}
