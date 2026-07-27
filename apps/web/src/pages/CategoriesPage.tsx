import { Box, Card, CardActionArea, Container, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { categories } from '../data/sampleServices';

export function CategoriesPage() {
  const { t } = useTranslation();

  return (
    <Container maxWidth="lg" sx={{ py: 5 }}>
      <Typography variant="h4" sx={{ fontWeight: 800 }}>
        {t('categories.title')}
      </Typography>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr' },
          gap: 2,
          mt: 3,
        }}
      >
        {categories.map((cat) => (
          <Card key={cat} elevation={0} sx={{ border: 1, borderColor: 'divider', borderRadius: 3 }}>
            <CardActionArea component={Link} to={`/services?category=${cat}`} sx={{ p: 3 }}>
              <Typography variant="h6" sx={{ fontWeight: 700 }} color="primary.main">
                {t(`categories.${cat}`)}
              </Typography>
            </CardActionArea>
          </Card>
        ))}
      </Box>
    </Container>
  );
}
