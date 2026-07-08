import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import {
  Alert, Box, Button, Card, CardContent, Chip, CircularProgress, Container,
  Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, Grid,
  Switch, TextField, Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';
import {
  subscriptionPlansApi, userSubscriptionApi,
  type SubscriptionPlan, type UserSubscription,
} from '../services/subscriptionsApi';
import { getApiErrorMessage } from '../utils/apiError';

const SUBSCRIBER_ROLES = new Set(['Craftsman', 'Store', 'StoreOwner']);

export function SubscriptionPlansPage() {
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';
  const { user } = useAuth();
  const navigate = useNavigate();
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [current, setCurrent] = useState<UserSubscription | null | undefined>(undefined);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      setError('');
      try {
        const [plansRes, currentRes] = await Promise.all([
          subscriptionPlansApi.list(SUBSCRIBER_ROLES.has(user?.role ?? '') || user?.primaryRole === 'StoreOwner'
            ? (user?.role === 'Store' || user?.primaryRole === 'StoreOwner' ? 'Store' : 'Craftsman')
            : undefined),
          userSubscriptionApi.getCurrent().catch(() => ({ data: { data: null } })),
        ]);
        setPlans(plansRes.data.data);
        setCurrent(currentRes.data.data);
      } catch (e) {
        setError(getApiErrorMessage(e, t('common.error')));
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [user, t]);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>
        {t('subscription.plansTitle')}
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {current && (
        <Alert severity="info" sx={{ mb: 3 }}>
          {t('subscription.activePlan', {
            plan: isAr ? current.planNameAr : current.planNameEn,
            status: current.status,
          })}{' '}
          <Button component={Link} to="/subscription" size="small">{t('subscription.manage')}</Button>
        </Alert>
      )}

      <Grid container spacing={3}>
        {plans.map((plan) => (
          <Grid key={plan.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
              <CardContent sx={{ flexGrow: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>
                    {isAr ? plan.nameAr : plan.nameEn}
                  </Typography>
                  {plan.isFeatured && <Chip label={t('subscription.featured')} size="small" color="primary" />}
                </Box>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                  {isAr ? plan.descriptionAr : plan.descriptionEn}
                </Typography>
                {plan.billingOptions.filter((b) => b.isActive).map((option) => (
                  <Typography key={option.id ?? option.cycle} variant="body2" sx={{ mb: 0.5 }}>
                    {t(`subscription.cycle.${option.cycle}`, option.cycle)}: {option.price} {plan.currency}
                  </Typography>
                ))}
                {plan.maxServices != null && (
                  <Typography variant="caption" sx={{ display: 'block' }} color="text.secondary">
                    {t('subscription.maxServices', { count: plan.maxServices })}
                  </Typography>
                )}
              </CardContent>
              <Box sx={{ p: 2, pt: 0 }}>
                <Button
                  fullWidth
                  variant="contained"
                  disabled={!!current}
                  onClick={() => navigate(`/subscriptions/${plan.id}`)}
                >
                  {current ? t('subscription.alreadySubscribed') : t('subscription.choosePlan')}
                </Button>
              </Box>
            </Card>
          </Grid>
        ))}
      </Grid>

      {plans.length === 0 && (
        <Typography color="text.secondary">{t('subscription.noPlans')}</Typography>
      )}
    </Container>
  );
}

export function SubscribePage() {
  const { planId } = useParams<{ planId: string }>();
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';
  const navigate = useNavigate();
  const [plan, setPlan] = useState<SubscriptionPlan | null>(null);
  const [billingOptionId, setBillingOptionId] = useState('');
  const [autoRenew, setAutoRenew] = useState(true);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!planId) return;
    subscriptionPlansApi.getById(planId)
      .then((res) => {
        setPlan(res.data.data);
        const first = res.data.data.billingOptions.find((b) => b.isActive);
        if (first?.id) setBillingOptionId(first.id);
      })
      .catch((e) => setError(getApiErrorMessage(e, t('common.error'))))
      .finally(() => setLoading(false));
  }, [planId, t]);

  const handleSubscribe = async () => {
    if (!plan || !billingOptionId) return;
    setSubmitting(true);
    setError('');
    try {
      await userSubscriptionApi.subscribe({ planId: plan.id, billingOptionId, autoRenew });
      navigate('/subscription');
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <CircularProgress sx={{ m: 4 }} />;
  if (!plan) return <Alert severity="error">{t('subscription.planNotFound')}</Alert>;

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>
        {t('subscription.subscribeTitle')}
      </Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Card>
        <CardContent>
          <Typography variant="h6">{isAr ? plan.nameAr : plan.nameEn}</Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {isAr ? plan.descriptionAr : plan.descriptionEn}
          </Typography>

          <Typography variant="subtitle2" gutterBottom>{t('subscription.billingCycle')}</Typography>
          {plan.billingOptions.filter((b) => b.isActive).map((option) => (
            <Button
              key={option.id}
              variant={billingOptionId === option.id ? 'contained' : 'outlined'}
              onClick={() => option.id && setBillingOptionId(option.id)}
              sx={{ mr: 1, mb: 1 }}
            >
              {t(`subscription.cycle.${option.cycle}`, option.cycle)} — {option.price} {plan.currency}
            </Button>
          ))}

          <FormControlLabel
            control={<Switch checked={autoRenew} onChange={(e) => setAutoRenew(e.target.checked)} />}
            label={t('subscription.autoRenew')}
            sx={{ display: 'block', mt: 2 }}
          />

          <Box sx={{ display: 'flex', gap: 1, mt: 3 }}>
            <Button onClick={() => navigate('/subscriptions')}>{t('common.back')}</Button>
            <Button variant="contained" onClick={handleSubscribe} disabled={submitting || !billingOptionId}>
              {submitting ? <CircularProgress size={24} /> : t('subscription.confirmSubscribe')}
            </Button>
          </Box>
        </CardContent>
      </Card>
    </Container>
  );
}

export function MySubscriptionPage() {
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';
  const [subscription, setSubscription] = useState<UserSubscription | null | undefined>(undefined);
  const [history, setHistory] = useState<UserSubscription[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [cancelOpen, setCancelOpen] = useState(false);
  const [cancelReason, setCancelReason] = useState('');
  const [actionLoading, setActionLoading] = useState(false);

  const load = async () => {
    setLoading(true);
    try {
      const [currentRes, historyRes] = await Promise.all([
        userSubscriptionApi.getCurrent(),
        userSubscriptionApi.getHistory(),
      ]);
      setSubscription(currentRes.data.data);
      setHistory(historyRes.data.data.items);
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleAutoRenew = async (autoRenew: boolean) => {
    setActionLoading(true);
    try {
      const res = await userSubscriptionApi.updateAutoRenew(autoRenew);
      setSubscription(res.data.data);
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setActionLoading(false);
    }
  };

  const handleCancel = async () => {
    if (!subscription) return;
    setActionLoading(true);
    try {
      await userSubscriptionApi.cancel(subscription.id, cancelReason || undefined);
      setCancelOpen(false);
      await load();
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setActionLoading(false);
    }
  };

  if (loading) return <CircularProgress sx={{ m: 4 }} />;

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>
        {t('subscription.mySubscription')}
      </Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {!subscription ? (
        <Card>
          <CardContent>
            <Typography gutterBottom>{t('subscription.noActive')}</Typography>
            <Button variant="contained" component={Link} to="/subscriptions">
              {t('subscription.browsePlans')}
            </Button>
          </CardContent>
        </Card>
      ) : (
        <Card sx={{ mb: 3 }}>
          <CardContent>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
              <div>
                <Typography variant="h6">{isAr ? subscription.planNameAr : subscription.planNameEn}</Typography>
                <Chip label={subscription.status} color="primary" size="small" sx={{ mt: 1 }} />
              </div>
              <Typography variant="h6" sx={{ fontWeight: 700 }}>
                {subscription.amountPaid} {subscription.currency}
              </Typography>
            </Box>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
              {t('subscription.startDate')}: {new Date(subscription.startDate).toLocaleDateString()}
            </Typography>
            {subscription.endDate && (
              <Typography variant="body2" color="text.secondary">
                {t('subscription.endDate')}: {new Date(subscription.endDate).toLocaleDateString()}
              </Typography>
            )}
            {subscription.billingCycle && (
              <Typography variant="body2" color="text.secondary">
                {t('subscription.billingCycle')}: {subscription.billingCycle}
              </Typography>
            )}
            <FormControlLabel
              control={
                <Switch
                  checked={subscription.autoRenew}
                  disabled={actionLoading}
                  onChange={(e) => handleAutoRenew(e.target.checked)}
                />
              }
              label={t('subscription.autoRenew')}
              sx={{ display: 'block', mt: 2 }}
            />
            <Button color="error" variant="outlined" sx={{ mt: 2 }} onClick={() => setCancelOpen(true)}>
              {t('subscription.cancel')}
            </Button>
          </CardContent>
        </Card>
      )}

      {history.length > 0 && (
        <>
          <Typography variant="h6" gutterBottom>{t('subscription.history')}</Typography>
          {history.map((item) => (
            <Card key={item.id} sx={{ mb: 1 }}>
              <CardContent sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <div>
                  <Typography sx={{ fontWeight: 600 }}>{isAr ? item.planNameAr : item.planNameEn}</Typography>
                  <Typography variant="caption" color="text.secondary">
                    {new Date(item.startDate).toLocaleDateString()} — {item.status}
                  </Typography>
                </div>
                <Typography>{item.amountPaid} {item.currency}</Typography>
              </CardContent>
            </Card>
          ))}
        </>
      )}

      <Dialog open={cancelOpen} onClose={() => setCancelOpen(false)}>
        <DialogTitle>{t('subscription.cancelTitle')}</DialogTitle>
        <DialogContent>
          <Typography variant="body2" sx={{ mb: 2 }}>{t('subscription.cancelHint')}</Typography>
          <TextField
            fullWidth
            label={t('subscription.cancelReason')}
            value={cancelReason}
            onChange={(e) => setCancelReason(e.target.value)}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCancelOpen(false)}>{t('common.cancel')}</Button>
          <Button color="error" onClick={handleCancel} disabled={actionLoading}>
            {t('subscription.confirmCancel')}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
}
