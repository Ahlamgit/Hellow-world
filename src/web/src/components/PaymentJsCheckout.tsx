import { useEffect, useRef, useState } from 'react';
import { Alert, Box, Button, CircularProgress, TextField, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import type { BookingPayment } from '../services/api';

declare global {
  interface Window {
    PaymentJs?: new () => {
      init: (
        publicIntegrationKey: string,
        numberDivId: string,
        cvvDivId: string,
        callback: (payment: {
          tokenize: (
            data: Record<string, string>,
            onSuccess: (token: string) => void,
            onError: (errors: unknown[]) => void,
          ) => void;
        }) => void,
      ) => void;
    };
  }
}

interface PaymentJsCheckoutProps {
  payment: BookingPayment;
  loading: boolean;
  onAuthorize: (transactionToken: string) => Promise<void>;
}

export function PaymentJsCheckout({ payment, loading, onAuthorize }: PaymentJsCheckoutProps) {
  const { t } = useTranslation();
  const [scriptReady, setScriptReady] = useState(false);
  const [cardHolder, setCardHolder] = useState('');
  const [expMonth, setExpMonth] = useState('');
  const [expYear, setExpYear] = useState('');
  const [error, setError] = useState<string | null>(null);
  const paymentRef = useRef<ReturnType<NonNullable<typeof window.PaymentJs>> | null>(null);

  useEffect(() => {
    if (!payment.paymentJsScriptUrl || !payment.publicIntegrationKey) return;

    const existing = document.querySelector(`script[data-payment-js="true"]`);
    if (existing) {
      setScriptReady(true);
      return;
    }

    const script = document.createElement('script');
    script.src = payment.paymentJsScriptUrl;
    script.async = true;
    script.dataset.paymentJs = 'true';
    script.onload = () => setScriptReady(true);
    script.onerror = () => setError(t('booking.paymentJsLoadFailed'));
    document.body.appendChild(script);

    return () => {
      script.remove();
    };
  }, [payment.paymentJsScriptUrl, payment.publicIntegrationKey, t]);

  useEffect(() => {
    if (!scriptReady || !window.PaymentJs || !payment.publicIntegrationKey || paymentRef.current) return;

    const paymentJs = new window.PaymentJs();
    paymentJs.init(payment.publicIntegrationKey, 'areeba-number-div', 'areeba-cvv-div', (instance) => {
      paymentRef.current = instance as unknown as ReturnType<NonNullable<typeof window.PaymentJs>>;
    });
  }, [scriptReady, payment.publicIntegrationKey]);

  const submit = () => {
    setError(null);
    const instance = paymentRef.current as unknown as {
      tokenize?: (
        data: Record<string, string>,
        onSuccess: (token: string) => void,
        onError: (errors: Array<{ message?: string }>) => void,
      ) => void;
    };

    if (!instance?.tokenize) {
      setError(t('booking.paymentJsNotReady'));
      return;
    }

    instance.tokenize(
      {
        card_holder: cardHolder,
        month: expMonth,
        year: expYear,
      },
      async (token) => {
        try {
          await onAuthorize(token);
        } catch (err) {
          setError(err instanceof Error ? err.message : t('booking.paymentAuthorizeFailed'));
        }
      },
      (errors) => {
        setError(errors[0]?.message ?? t('booking.paymentTokenizeFailed'));
      },
    );
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      <Alert severity="info">{t('booking.paymentJsNotice')}</Alert>
      <TextField label={t('booking.cardHolder')} value={cardHolder} onChange={(e) => setCardHolder(e.target.value)} fullWidth />
      <Box>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>{t('booking.cardNumber')}</Typography>
        <Box id="areeba-number-div" sx={{ height: 40, border: '1px solid', borderColor: 'divider', borderRadius: 1 }} />
      </Box>
      <Box>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>{t('booking.cardCvv')}</Typography>
        <Box id="areeba-cvv-div" sx={{ height: 40, border: '1px solid', borderColor: 'divider', borderRadius: 1, maxWidth: 120 }} />
      </Box>
      <Box sx={{ display: 'flex', gap: 1 }}>
        <TextField label={t('booking.expMonth')} value={expMonth} onChange={(e) => setExpMonth(e.target.value)} fullWidth />
        <TextField label={t('booking.expYear')} value={expYear} onChange={(e) => setExpYear(e.target.value)} fullWidth />
      </Box>
      {error && <Alert severity="error">{error}</Alert>}
      <Button variant="contained" fullWidth onClick={submit} disabled={loading || !scriptReady}>
        {loading ? <CircularProgress size={24} /> : t('booking.submitPayment')}
      </Button>
    </Box>
  );
}
