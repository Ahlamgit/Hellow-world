import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { arSA, enUS } from '@mui/material/locale';
import { useTranslation } from 'react-i18next';

type ThemeMode = 'light' | 'dark';

interface ThemeContextType {
  mode: ThemeMode;
  toggleMode: () => void;
}

const ThemeContext = createContext<ThemeContextType>({ mode: 'light', toggleMode: () => {} });

export const useThemeMode = () => useContext(ThemeContext);

export function ThemeContextProvider({ children }: { children: ReactNode }) {
  const [mode, setMode] = useState<ThemeMode>(
    () => (localStorage.getItem('khadamati-theme') as ThemeMode) || 'light'
  );
  const { i18n } = useTranslation();
  const isRtl = i18n.language === 'ar';

  const toggleMode = () => {
    setMode((prev) => {
      const next = prev === 'light' ? 'dark' : 'light';
      localStorage.setItem('khadamati-theme', next);
      return next;
    });
  };

  const theme = useMemo(
    () =>
      createTheme(
        {
          direction: isRtl ? 'rtl' : 'ltr',
          palette: {
            mode,
            primary: { main: '#1565C0', light: '#5E92F3', dark: '#003C8F' },
            secondary: { main: '#FF6F00', light: '#FFA040', dark: '#C43E00' },
            background: {
              default: mode === 'light' ? '#F5F7FA' : '#121212',
              paper: mode === 'light' ? '#FFFFFF' : '#1E1E1E',
            },
          },
          typography: {
            fontFamily: isRtl
              ? '"Tajawal", "Roboto", "Helvetica", "Arial", sans-serif'
              : '"Roboto", "Helvetica", "Arial", sans-serif',
          },
          shape: { borderRadius: 12 },
          components: {
            MuiButton: { styleOverrides: { root: { textTransform: 'none', fontWeight: 600 } } },
            MuiCard: { styleOverrides: { root: { boxShadow: mode === 'light' ? '0 2px 12px rgba(0,0,0,0.08)' : undefined } } },
          },
        },
        isRtl ? arSA : enUS
      ),
    [mode, isRtl]
  );

  return (
    <ThemeContext.Provider value={{ mode, toggleMode }}>
      <ThemeProvider theme={theme}>{children}</ThemeProvider>
    </ThemeContext.Provider>
  );
}
