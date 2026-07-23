export const DEFAULT_CURRENCY = 'USD';
export const DEFAULT_COUNTRY = 'LB';
export const DEFAULT_TIMEZONE = 'Asia/Beirut';

export function formatCurrency(amount: number | string, currency = DEFAULT_CURRENCY): string {
  return `${amount} ${currency}`;
}
