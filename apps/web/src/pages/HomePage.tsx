import { useState } from 'react';
import {
  Box,
  Button,
  Chip,
  Container,
  InputAdornment,
  TextField,
  Typography,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import VerifiedUserIcon from '@mui/icons-material/VerifiedUser';
import EventAvailableIcon from '@mui/icons-material/EventAvailable';
import SupportAgentIcon from '@mui/icons-material/SupportAgent';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { ServiceCard } from '../components/ServiceCard';
import { categories, sampleServices } from '../data/sampleServices';

export function HomePage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [query, setQuery] = useState('');

  const onSearch = () => {
    navigate(`/services?q=${encodeURIComponent(query)}`);
  };

  return (
    <>
      <Box
        sx={{
          background: 'linear-gradient(135deg, #FFF3E0 0%, #FFFFFF 55%, #FFE0B2 100%)',
          py: { xs: 6, md: 10 },
        }}
      >
        <Container maxWidth="lg">
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, alignItems: { xs: 'center', md: 'flex-start' }, textAlign: { xs: 'center', md: 'start' } }}>
            <Typography variant="h3" component="h1" sx={{ fontWeight: 800, maxWidth: 720, color: 'grey.900' }}>
              {t('home.heroTitle')}
            </Typography>
            <Typography variant="h6" color="text.secondary" sx={{ maxWidth: 600, fontWeight: 400 }}>
              {t('home.heroSubtitle')}
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 1, width: '100%', maxWidth: 640 }}>
              <TextField
                fullWidth
                placeholder={t('home.searchPlaceholder')}
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') onSearch();
                }}
                sx={{ '& .MuiOutlinedInput-root': { bgcolor: 'background.paper', borderRadius: 2 } }}
                slotProps={{
                  input: {
                    startAdornment: (
                      <InputAdornment position="start">
                        <SearchIcon color="primary" />
                      </InputAdornment>
                    ),
                  },
                }}
              />
              <Button variant="contained" size="large" onClick={onSearch} sx={{ px: 4, whiteSpace: 'nowrap' }}>
                {t('home.searchButton')}
              </Button>
            </Box>
            <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 2, pt: 1 }}>
              {[
                { icon: <VerifiedUserIcon color="primary" />, label: t('home.trustVerified') },
                { icon: <EventAvailableIcon color="primary" />, label: t('home.trustBooking') },
                { icon: <SupportAgentIcon color="primary" />, label: t('home.trustSupport') },
              ].map((item) => (
                <Box key={item.label} sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  {item.icon}
                  <Typography variant="body2" sx={{ fontWeight: 600 }}>
                    {item.label}
                  </Typography>
                </Box>
              ))}
            </Box>
          </Box>
        </Container>
      </Box>

      <Container maxWidth="lg" sx={{ py: 6 }}>
        <Typography variant="h5" sx={{ fontWeight: 800, mb: 2 }}>
          {t('home.categoriesTitle')}
        </Typography>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mb: 5 }}>
          {categories.map((cat) => (
            <Chip
              key={cat}
              component={Link}
              to={`/services?category=${cat}`}
              clickable
              label={t(`categories.${cat}`)}
              color="primary"
              variant="outlined"
            />
          ))}
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h5" sx={{ fontWeight: 800 }}>
            {t('home.featuredTitle')}
          </Typography>
          <Button component={Link} to="/services">
            {t('nav.browse')}
          </Button>
        </Box>
        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', lg: '1fr 1fr 1fr' },
            gap: 2,
          }}
        >
          {sampleServices.slice(0, 6).map((service) => (
            <ServiceCard key={service.id} service={service} />
          ))}
        </Box>

        <Box sx={{ mt: 6, p: 4, borderRadius: 3, bgcolor: 'primary.main', color: 'primary.contrastText', textAlign: 'center' }}>
          <Typography variant="h6" sx={{ fontWeight: 700, mb: 1 }}>
            {t('home.ctaProvider')}
          </Typography>
          <Button component={Link} to="/login" variant="contained" sx={{ bgcolor: 'background.paper', color: 'primary.main', mt: 1 }}>
            {t('home.ctaProviderLink')}
          </Button>
        </Box>
      </Container>
    </>
  );
}
