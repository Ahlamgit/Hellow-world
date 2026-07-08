import { useEffect, useState } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import {
  Box, Button, Card, CardContent, Chip, CircularProgress, Container, Step, StepLabel, Stepper,
  TextField, Typography, Alert, Dialog, DialogTitle, DialogContent, DialogActions,
} from '@mui/material';
import Grid from '@mui/material/Grid';
import { useTranslation } from 'react-i18next';
import {
  bookingsApi, type Booking, type CraftsmanOption, type Service, type TimeSlot,
} from '../services/api';
import { servicesApi } from '../services/api';

const STEPS = ['service', 'craftsman', 'datetime', 'confirm'];

export function BookingWizardPage() {
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const preselectedServiceId = searchParams.get('serviceId');

  const [activeStep, setActiveStep] = useState(preselectedServiceId ? 1 : 0);
  const [services, setServices] = useState<Service[]>([]);
  const [craftsmen, setCraftsmen] = useState<CraftsmanOption[]>([]);
  const [slots, setSlots] = useState<TimeSlot[]>([]);
  const [selectedService, setSelectedService] = useState<Service | null>(null);
  const [selectedCraftsman, setSelectedCraftsman] = useState<CraftsmanOption | null>(null);
  const [selectedDate, setSelectedDate] = useState('');
  const [selectedSlot, setSelectedSlot] = useState<TimeSlot | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    servicesApi.getServices().then((res) => {
      setServices(res.data.data);
      if (preselectedServiceId) {
        const svc = res.data.data.find((s) => s.id === preselectedServiceId);
        if (svc) setSelectedService(svc);
      }
    });
  }, [preselectedServiceId]);

  const loadCraftsmen = async (serviceId: string) => {
    setLoading(true);
    try {
      const res = await bookingsApi.getCraftsmen(serviceId);
      setCraftsmen(res.data.data);
    } catch { setError(t('common.error')); }
    finally { setLoading(false); }
  };

  const loadSlots = async () => {
    if (!selectedCraftsman || !selectedService || !selectedDate) return;
    setLoading(true);
    try {
      const res = await bookingsApi.getAvailability(selectedCraftsman.id, selectedService.id, selectedDate);
      setSlots(res.data.data.filter((s) => s.isAvailable));
    } catch { setError(t('common.error')); }
    finally { setLoading(false); }
  };

  const handleCreateBooking = async () => {
    if (!selectedService || !selectedCraftsman || !selectedSlot) return;
    setLoading(true);
    setError('');
    try {
      const res = await bookingsApi.create({
        serviceId: selectedService.id,
        craftsmanId: selectedCraftsman.id,
        scheduledAt: selectedSlot.start,
      });
      await bookingsApi.confirm(res.data.data.id);
      navigate(`/bookings/${res.data.data.id}/payment`);
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string } } };
      setError(err.response?.data?.message ?? t('common.error'));
    } finally { setLoading(false); }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('booking.title')}</Typography>
      <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
        {STEPS.map((s) => <Step key={s}><StepLabel>{t(`booking.steps.${s}`)}</StepLabel></Step>)}
      </Stepper>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {activeStep === 0 && (
        <Grid container spacing={2}>
          {services.map((svc) => (
            <Grid key={svc.id} size={{ xs: 12, sm: 6 }}>
              <Card sx={{ cursor: 'pointer', border: selectedService?.id === svc.id ? 2 : 0, borderColor: 'primary.main' }}
                onClick={() => { setSelectedService(svc); setActiveStep(1); loadCraftsmen(svc.id); }}>
                <CardContent>
                  <Typography sx={{ fontWeight: 600 }}>{isAr ? svc.nameAr : svc.nameEn}</Typography>
                  <Typography variant="body2" color="text.secondary">{svc.basePrice} SAR</Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {activeStep === 1 && (
        <Box>
          {loading ? <CircularProgress /> : craftsmen.map((c) => (
            <Card key={c.id} sx={{ mb: 2, cursor: 'pointer' }}
              onClick={() => { setSelectedCraftsman(c); setActiveStep(2); }}>
              <CardContent sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Box>
                  <Typography sx={{ fontWeight: 600 }}>{c.firstName} {c.lastName}</Typography>
                  <Typography variant="body2">{c.specialization}</Typography>
                </Box>
                <Box sx={{ textAlign: 'right' }}>
                  <Chip label={`⭐ ${c.rating}`} size="small" />
                  <Typography variant="body2">{c.price} SAR</Typography>
                </Box>
              </CardContent>
            </Card>
          ))}
          <Button onClick={() => setActiveStep(0)}>{t('common.back')}</Button>
        </Box>
      )}

      {activeStep === 2 && (
        <Box>
          <TextField type="date" label={t('booking.selectDate')} fullWidth sx={{ mb: 2 }}
            value={selectedDate} onChange={(e) => setSelectedDate(e.target.value)}
            slotProps={{ inputLabel: { shrink: true } }} />
          <Button variant="outlined" onClick={loadSlots} disabled={!selectedDate}>{t('booking.loadSlots')}</Button>
          <Grid container spacing={1} sx={{ mt: 2 }}>
            {slots.map((slot) => (
              <Grid key={slot.start} size={{ xs: 6, sm: 4 }}>
                <Button fullWidth variant={selectedSlot?.start === slot.start ? 'contained' : 'outlined'}
                  onClick={() => setSelectedSlot(slot)}>
                  {new Date(slot.start).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </Button>
              </Grid>
            ))}
          </Grid>
          <Box sx={{ mt: 2, display: 'flex', gap: 1 }}>
            <Button onClick={() => setActiveStep(1)}>{t('common.back')}</Button>
            <Button variant="contained" disabled={!selectedSlot} onClick={() => setActiveStep(3)}>{t('common.next')}</Button>
          </Box>
        </Box>
      )}

      {activeStep === 3 && selectedService && selectedCraftsman && selectedSlot && (
        <Card>
          <CardContent>
            <Typography variant="h6">{t('booking.summary')}</Typography>
            <Typography>{isAr ? selectedService.nameAr : selectedService.nameEn}</Typography>
            <Typography>{selectedCraftsman.firstName} {selectedCraftsman.lastName}</Typography>
            <Typography>{new Date(selectedSlot.start).toLocaleString()}</Typography>
            <Typography sx={{ fontWeight: 700 }}>{selectedCraftsman.price} SAR</Typography>
            <Box sx={{ mt: 2, display: 'flex', gap: 1 }}>
              <Button onClick={() => setActiveStep(2)}>{t('common.back')}</Button>
              <Button variant="contained" onClick={handleCreateBooking} disabled={loading}>
                {loading ? <CircularProgress size={24} /> : t('booking.confirmAndPay')}
              </Button>
            </Box>
          </CardContent>
        </Card>
      )}
    </Container>
  );
}

export function MyBookingsPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    bookingsApi.list().then((res) => setBookings(res.data.data.items)).finally(() => setLoading(false));
  }, []);

  if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}><CircularProgress /></Box>;

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('booking.myBookings')}</Typography>
      {bookings.map((b) => (
        <Card key={b.id} sx={{ mb: 2, cursor: 'pointer' }} onClick={() => navigate(`/bookings/${b.id}`)}>
          <CardContent sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Box>
              <Typography sx={{ fontWeight: 600 }}>{b.serviceName}</Typography>
              <Typography variant="body2">{b.bookingReference} · {new Date(b.scheduledAt).toLocaleString()}</Typography>
            </Box>
            <Chip label={t(`booking.status.${b.status}`)} color={statusColor(b.status)} />
          </CardContent>
        </Card>
      ))}
    </Container>
  );
}

export function BookingDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [booking, setBooking] = useState<Booking | null>(null);
  const [rejectOpen, setRejectOpen] = useState(false);
  const [reason, setReason] = useState('');

  const reload = () => { if (id) bookingsApi.getById(id).then((res) => setBooking(res.data.data)); };
  useEffect(() => { reload(); }, [id]);

  if (!booking) return <CircularProgress sx={{ m: 4 }} />;

  const role = localStorage.getItem('userRole') ?? 'Customer';

  const actions = () => {
    if (booking.status === 'AwaitingPayment' && role === 'Customer')
      return <Button variant="contained" onClick={() => navigate(`/bookings/${id}/payment`)}>{t('booking.pay')}</Button>;
    if (booking.status === 'PendingCraftsmanConfirmation' && role === 'Craftsman') return (
      <Box sx={{ display: 'flex', gap: 1 }}>
        <Button variant="contained" onClick={() => { bookingsApi.accept(booking.id).then(() => reload()); }}>{t('booking.accept')}</Button>
        <Button color="error" onClick={() => setRejectOpen(true)}>{t('booking.reject')}</Button>
      </Box>
    );
    if (booking.status === 'Confirmed' && role === 'Craftsman')
      return <Button variant="contained" onClick={() => { bookingsApi.complete(booking.id).then(() => reload()); }}>{t('booking.complete')}</Button>;
    return null;
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }}>{booking.serviceName}</Typography>
      <Chip label={t(`booking.status.${booking.status}`)} color={statusColor(booking.status)} sx={{ my: 2 }} />
      <Card sx={{ mb: 2 }}><CardContent>
        <Typography><strong>{t('booking.reference')}:</strong> {booking.bookingReference}</Typography>
        <Typography><strong>{t('booking.craftsman')}:</strong> {booking.craftsmanName}</Typography>
        <Typography><strong>{t('booking.scheduled')}:</strong> {new Date(booking.scheduledAt).toLocaleString()}</Typography>
        <Typography><strong>{t('booking.price')}:</strong> {booking.estimatedPrice} SAR</Typography>
      </CardContent></Card>
      {actions()}
      <Dialog open={rejectOpen} onClose={() => setRejectOpen(false)}>
        <DialogTitle>{t('booking.reject')}</DialogTitle>
        <DialogContent>
          <TextField fullWidth multiline rows={3} value={reason} onChange={(e) => setReason(e.target.value)} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRejectOpen(false)}>{t('common.cancel')}</Button>
          <Button color="error" onClick={() => { bookingsApi.reject(booking.id, reason).then(() => { setRejectOpen(false); reload(); }); }}>{t('booking.reject')}</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
}

export function BookingPaymentPage() {
  const { id } = useParams<{ id: string }>();
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const pay = async () => {
    if (!id) return;
    setLoading(true);
    try {
      await bookingsApi.initiatePayment(id, 'Card');
      await bookingsApi.confirmPayment(id, `TXN-${Date.now()}`);
      navigate(`/bookings/${id}`);
    } finally { setLoading(false); }
  };

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('booking.payment')}</Typography>
      <Card><CardContent>
        <Typography gutterBottom>{t('booking.paymentPending')}</Typography>
        <Button variant="contained" fullWidth onClick={pay} disabled={loading}>
          {loading ? <CircularProgress size={24} /> : t('booking.confirmPayment')}
        </Button>
      </CardContent></Card>
    </Container>
  );
}

function statusColor(status: string): 'default' | 'primary' | 'success' | 'warning' | 'error' {
  if (['Confirmed', 'Completed', 'PaymentConfirmed'].includes(status)) return 'success';
  if (['AwaitingPayment', 'PendingCraftsmanConfirmation', 'Pending'].includes(status)) return 'warning';
  if (['Rejected', 'Cancelled', 'Expired', 'NoShow'].includes(status)) return 'error';
  return 'default';
}
