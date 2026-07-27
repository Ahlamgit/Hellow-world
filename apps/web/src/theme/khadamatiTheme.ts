import { createTheme } from '@mui/material/styles';
import tokens from '../../../../packages/shared/tokens/design-tokens.json';

export const khadamatiTheme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: tokens.brand.primary,
      dark: tokens.brand.primaryDark,
      light: tokens.brand.primaryLight,
      contrastText: '#FFFFFF',
    },
    success: { main: tokens.semantic.success },
    warning: { main: tokens.semantic.warning },
    error: { main: tokens.semantic.error },
    info: { main: tokens.semantic.info },
    background: {
      default: tokens.neutral['50'],
      paper: tokens.neutral['0'],
    },
    text: {
      primary: tokens.neutral['900'],
      secondary: tokens.neutral['700'],
    },
  },
  typography: {
    fontFamily: tokens.typography.fontFamilyLatin,
  },
  shape: {
    borderRadius: tokens.radius.md,
  },
  spacing: tokens.spacing['1'],
});

export const khadamatiThemeRtl = createTheme({
  ...khadamatiTheme,
  direction: 'rtl',
  typography: {
    ...khadamatiTheme.typography,
    fontFamily: tokens.typography.fontFamilyArabic,
  },
});
