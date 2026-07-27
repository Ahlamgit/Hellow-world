import { useMemo, type ReactNode } from 'react';
import { CssBaseline } from '@mui/material';
import { ThemeProvider } from '@mui/material/styles';
import { CacheProvider } from '@emotion/react';
import createCache from '@emotion/cache';
import { prefixer } from 'stylis';
import rtlPlugin from 'stylis-plugin-rtl';
import { useTranslation } from 'react-i18next';
import { khadamatiTheme, khadamatiThemeRtl } from '../theme/khadamatiTheme';

export function AppProviders({ children }: { children: ReactNode }) {
  const { i18n } = useTranslation();
  const isRtl = i18n.language === 'ar';
  const theme = isRtl ? khadamatiThemeRtl : khadamatiTheme;
  const cache = useMemo(
    () =>
      createCache({
        key: isRtl ? 'muirtl' : 'muiltr',
        stylisPlugins: isRtl ? [prefixer, rtlPlugin] : [prefixer],
      }),
    [isRtl],
  );

  return (
    <CacheProvider key={i18n.language} value={cache}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </CacheProvider>
  );
}
