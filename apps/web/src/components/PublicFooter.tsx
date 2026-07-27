import { Box, Container, Link as MuiLink, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export function PublicFooter() {
  const { t } = useTranslation();
  return (
    <Box component="footer" sx={{ bgcolor: 'grey.900', color: 'grey.100', py: 5, mt: 'auto' }}>
      <Container maxWidth="lg">
        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, justifyContent: 'space-between', gap: 3 }}>
          <Box>
            <Typography variant="h6" sx={{ color: 'primary.light', fontWeight: 700 }}>
              {t('app.nameAr')} · {t('app.name')}
            </Typography>
            <Typography variant="body2" sx={{ mt: 1, opacity: 0.8 }}>
              {t('footer.rights')}
            </Typography>
          </Box>
          <Box sx={{ display: 'flex', flexDirection: 'row', gap: 3, flexWrap: 'wrap' }}>
            <MuiLink component={Link} to="/services" color="inherit" underline="hover">
              {t('footer.explore')}
            </MuiLink>
            <MuiLink component={Link} to="/how-it-works" color="inherit" underline="hover">
              {t('nav.howItWorks')}
            </MuiLink>
            <MuiLink component={Link} to="/register?type=provider" color="inherit" underline="hover">
              {t('footer.providers')}
            </MuiLink>
          </Box>
        </Box>
      </Container>
    </Box>
  );
}
