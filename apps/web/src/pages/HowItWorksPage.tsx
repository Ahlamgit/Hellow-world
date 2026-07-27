import { Box, Card, CardContent, Container, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';

const steps = ['step1', 'step2', 'step3', 'step4'] as const;

export function HowItWorksPage() {
  const { t } = useTranslation();

  return (
    <Container maxWidth="lg" sx={{ py: 5 }}>
      <Typography variant="h4" sx={{ fontWeight: 800 }}>
        {t('howItWorks.title')}
      </Typography>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' },
          gap: 2,
          mt: 3,
        }}
      >
        {steps.map((step, index) => (
          <Card key={step} elevation={0} sx={{ border: 1, borderColor: 'divider', borderRadius: 3 }}>
            <CardContent>
              <Box sx={{ display: 'flex', flexDirection: 'row', gap: 2, alignItems: 'flex-start' }}>
                <Box
                  sx={{
                    width: 36,
                    height: 36,
                    borderRadius: '50%',
                    bgcolor: 'primary.main',
                    color: 'primary.contrastText',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontWeight: 800,
                    flexShrink: 0,
                  }}
                >
                  {index + 1}
                </Box>
                <Box>
                  <Typography variant="h6" sx={{ fontWeight: 700 }}>
                    {t(`howItWorks.${step}Title`)}
                  </Typography>
                  <Typography color="text.secondary">{t(`howItWorks.${step}Desc`)}</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        ))}
      </Box>
    </Container>
  );
}
