import { Box, Container } from '@mui/material';
import type { ReactNode } from 'react';
import { PublicHeader } from '../PublicHeader';

type AuthPageLayoutProps = {
  children: ReactNode;
  maxWidth?: 'sm' | 'md';
};

export function AuthPageLayout({ children, maxWidth = 'sm' }: AuthPageLayoutProps) {
  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <PublicHeader />
      <Container maxWidth={maxWidth} sx={{ py: { xs: 4, md: 6 } }}>
        {children}
      </Container>
    </Box>
  );
}
