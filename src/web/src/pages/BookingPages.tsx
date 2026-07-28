import { useEffect, useState, type ReactNode } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import {
  Box, Button, Card, CardContent, Chip, CircularProgress, Container, Step, StepLabel, Stepper,
  TextField, Typography, Alert, Dialog, DialogTitle, DialogContent, DialogActions, MenuItem, Rating,
} from '@mui/material';
import Grid from '@mui/material/Grid';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';
import {
  bookingsApi, type Booking, type CraftsmanOption, type Service, type TimeSlot, type AddressDto,
} from '../services/api';
import { servicesApi, usersApi } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';
import { DEFAULT_CURRENCY, formatCurrency } from '../config/platform';
import { PaymentJsCheckout } from '../components/PaymentJsCheckout';
import type { BookingPayment } from '../services/api';

const CANCELLABLE_STATUSES = new Set(['Pending', 'AwaitingPayment', 'Confirmed', 'Rescheduled']);

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
  const [addresses, setAddresses] = useState<AddressDto[]>([]);
  const [selectedAddressId, setSelectedAddressId] = useState('');
  const [description, setDescription] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [nearbySearched, setNearbySearched] = useState(false);

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
    setNearbySearched(false);
    try {
      const res = await bookingsApi.getCraftsmen(serviceId);
      setCraftsmen(res.data.data);
    } catch { setError(t('common.error')); }
    finally { setLoading(false); }
  };

  const loadNearbyCraftsmen = async (serviceId: string) => {
    if (!navigator.geolocation) {
      setError(t('booking.locationUnsupported'));
      return;
    }
    setLoading(true);
    setError('');
    setNearbySearched(true);
    navigator.geolocation.getCurrentPosition(
      async (position) => {
        try {
          const res = await bookingsApi.getNearbyCraftsmen(
            serviceId,
            position.coords.latitude,
            position.coords.longitude,
          );
          setCraftsmen(res.data.data);
        } catch {
          setError(t('common.error'));
        } finally {
          setLoading(false);
        }
      },
      () => {
        setError(t('booking.locationDenied'));
        setLoading(false);
      },
      { enableHighAccuracy: true, timeout: 15000 },
    );
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

  useEffect(() => {
    if (activeStep !== 3) return;
    usersApi.listAddresses()
      .then((res) => {
        setAddresses(res.data.data);
        const defaultAddress = res.data.data.find((address) => address.isDefault);
        if (defaultAddress) setSelectedAddressId(defaultAddress.id);
      })
      .catch(() => setAddresses([]));
  }, [activeStep]);

  const handleCreateBooking = async () => {
    if (!selectedService || !selectedCraftsman || !selectedSlot) return;
    setLoading(true);
    setError('');
    try {
      const res = await bookingsApi.create({
        serviceId: selectedService.id,
        craftsmanId: selectedCraftsman.id,
        scheduledAt: selectedSlot.start,
        addressId: selectedAddressId || undefined,
        description: description.trim() || undefined,
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
                  <Typography variant="body2" color="text.secondary">{formatCurrency(svc.basePrice)}</Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {activeStep === 1 && (
        <Box>
          {selectedService && (
            <Button
              variant="outlined"
              sx={{ mb: 2 }}
              disabled={loading}
              onClick={() => loadNearbyCraftsmen(selectedService.id)}
            >
              {t('booking.findNearby')}
            </Button>
          )}
          {loading ? <CircularProgress /> : craftsmen.length === 0 ? (
            <Box sx={{ mb: 2 }}>
              <Typography color="text.secondary" sx={{ mb: 1 }}>
                {nearbySearched ? t('booking.noNearbyCraftsmen') : t('booking.noCraftsmen')}
              </Typography>
              {nearbySearched && selectedService && (
                <Button variant="outlined" onClick={() => loadCraftsmen(selectedService.id)}>
                  {t('booking.showAllCraftsmen')}
                </Button>
              )}
            </Box>
          ) : craftsmen.map((c) => (
            <Card key={c.id} sx={{ mb: 2, cursor: 'pointer' }}
              onClick={() => { setSelectedCraftsman(c); setActiveStep(2); }}>
              <CardContent sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Box>
                  <Typography sx={{ fontWeight: 600 }}>{c.firstName} {c.lastName}</Typography>
                  <Typography variant="body2">{c.specialization}</Typography>
                  {c.distanceKm != null && (
                    <Typography variant="caption" color="primary">{t('booking.distanceKm', { km: c.distanceKm })}</Typography>
                  )}
                </Box>
                <Box sx={{ textAlign: 'right' }}>
                  <Chip label={`⭐ ${c.rating}`} size="small" />
                  <Typography variant="body2">{formatCurrency(c.price)}</Typography>
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
            <Typography sx={{ fontWeight: 700 }}>{formatCurrency(selectedCraftsman.price)}</Typography>
            <TextField
              select
              fullWidth
              sx={{ mt: 2 }}
              label={t('addresses.selectAddress')}
              value={selectedAddressId}
              onChange={(e) => setSelectedAddressId(e.target.value)}
            >
              <MenuItem value="">{t('addresses.noAddress')}</MenuItem>
              {addresses.map((address) => (
                <MenuItem key={address.id} value={address.id}>
                  {address.label} — {address.street}, {address.city}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              fullWidth
              sx={{ mt: 2 }}
              label={t('booking.description')}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              multiline
              minRows={2}
            />
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
  const { user } = useAuth();
  const [booking, setBooking] = useState<Booking | null>(null);
  const [error, setError] = useState('');
  const [actionLoading, setActionLoading] = useState(false);

  const [rejectOpen, setRejectOpen] = useState(false);
  const [reason, setReason] = useState('');

  const [cancelOpen, setCancelOpen] = useState(false);
  const [cancelReason, setCancelReason] = useState('');

  const [rescheduleOpen, setRescheduleOpen] = useState(false);
  const [rescheduleDate, setRescheduleDate] = useState('');
  const [rescheduleSlots, setRescheduleSlots] = useState<TimeSlot[]>([]);
  const [selectedRescheduleSlot, setSelectedRescheduleSlot] = useState<TimeSlot | null>(null);
  const [rescheduleReason, setRescheduleReason] = useState('');

  const [noShowOpen, setNoShowOpen] = useState(false);
  const [reviewOpen, setReviewOpen] = useState(false);
  const [reviewRating, setReviewRating] = useState<number | null>(5);
  const [reviewComment, setReviewComment] = useState('');
  const [reviewSuccess, setReviewSuccess] = useState('');

  const reload = () => {
    if (!id) return;
    bookingsApi.getById(id).then((res) => setBooking(res.data.data)).catch((e) => {
      setError(getApiErrorMessage(e, t('common.error')));
    });
  };

  useEffect(() => { reload(); }, [id]);

  if (!booking) return <CircularProgress sx={{ m: 4 }} />;

  const role = user?.role || user?.primaryRole || 'Customer';
  const isCustomer = role === 'Customer';
  const isCraftsman = role === 'Craftsman';

  const loadRescheduleSlots = async () => {
    if (!rescheduleDate || !booking) return;
    setActionLoading(true);
    setError('');
    try {
      const res = await bookingsApi.getAvailability(booking.craftsmanId, booking.serviceId, rescheduleDate);
      setRescheduleSlots(res.data.data.filter((s) => s.isAvailable));
      setSelectedRescheduleSlot(null);
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setActionLoading(false);
    }
  };

  const handleCancel = async () => {
    if (!cancelReason.trim()) return;
    setActionLoading(true);
    setError('');
    try {
      await bookingsApi.cancel(booking.id, cancelReason);
      setCancelOpen(false);
      setCancelReason('');
      reload();
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setActionLoading(false);
    }
  };

  const handleReschedule = async () => {
    if (!selectedRescheduleSlot) return;
    setActionLoading(true);
    setError('');
    try {
      const res = await bookingsApi.reschedule(
        booking.id,
        selectedRescheduleSlot.start,
        rescheduleReason || undefined,
      );
      setRescheduleOpen(false);
      setRescheduleDate('');
      setRescheduleSlots([]);
      setSelectedRescheduleSlot(null);
      setRescheduleReason('');
      if (res.data.data.status === 'AwaitingPayment') {
        navigate(`/bookings/${booking.id}/payment`);
      } else {
        reload();
      }
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setActionLoading(false);
    }
  };

  const handleNoShow = async () => {
    setActionLoading(true);
    setError('');
    try {
      await bookingsApi.noShow(booking.id);
      setNoShowOpen(false);
      reload();
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setActionLoading(false);
    }
  };

  const handleSubmitReview = async () => {
    if (!reviewRating) return;
    setActionLoading(true);
    setError('');
    try {
      await bookingsApi.submitReview(booking.id, reviewRating, reviewComment.trim() || undefined);
      setReviewOpen(false);
      setReviewSuccess(t('review.submitted'));
      reload();
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setActionLoading(false);
    }
  };

  const chatEligible = !['Cancelled', 'Rejected', 'Expired'].includes(booking.status);

  const actions = () => {
    const buttons: ReactNode[] = [];

    if (booking.status === 'AwaitingPayment' && isCustomer) {
      buttons.push(
        <Button key="pay" variant="contained" onClick={() => navigate(`/bookings/${id}/payment`)}>
          {t('booking.pay')}
        </Button>,
      );
    }

    if (booking.status === 'PendingCraftsmanConfirmation' && isCraftsman) {
      buttons.push(
        <Button key="accept" variant="contained" onClick={async () => {
          setActionLoading(true);
          try { await bookingsApi.accept(booking.id); reload(); }
          catch (e) { setError(getApiErrorMessage(e, t('common.error'))); }
          finally { setActionLoading(false); }
        }}>{t('booking.accept')}</Button>,
        <Button key="reject" color="error" onClick={() => setRejectOpen(true)}>{t('booking.reject')}</Button>,
      );
    }

    if (booking.status === 'Confirmed' && isCraftsman) {
      buttons.push(
        <Button key="complete" variant="contained" onClick={async () => {
          setActionLoading(true);
          try { await bookingsApi.complete(booking.id); reload(); }
          catch (e) { setError(getApiErrorMessage(e, t('common.error'))); }
          finally { setActionLoading(false); }
        }}>{t('booking.complete')}</Button>,
        <Button key="no-show" color="warning" onClick={() => setNoShowOpen(true)}>{t('booking.noShow')}</Button>,
      );
    }

    if (CANCELLABLE_STATUSES.has(booking.status) && (isCustomer || isCraftsman)) {
      buttons.push(
        <Button key="cancel" color="error" variant="outlined" onClick={() => setCancelOpen(true)}>
          {t('booking.cancel')}
        </Button>,
      );
    }

    if (booking.status === 'Confirmed' && (isCustomer || isCraftsman)) {
      buttons.push(
        <Button key="reschedule" variant="outlined" onClick={() => setRescheduleOpen(true)}>
          {t('booking.reschedule')}
        </Button>,
      );
    }

    if (chatEligible && (isCustomer || isCraftsman)) {
      buttons.push(
        <Button key="chat" variant="outlined" onClick={() => navigate(`/chat/${booking.id}`)}>
          {t('chat.openChat')}
        </Button>,
      );
    }

    if (booking.status === 'Completed' && isCustomer && !booking.customerRating) {
      buttons.push(
        <Button key="review" variant="contained" onClick={() => setReviewOpen(true)}>
          {t('review.leaveReview')}
        </Button>,
      );
    }

    if (buttons.length === 0) return null;
    return <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>{buttons}</Box>;
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }}>{booking.serviceName}</Typography>
      <Chip label={t(`booking.status.${booking.status}`)} color={statusColor(booking.status)} sx={{ my: 2 }} />
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {reviewSuccess && <Alert severity="success" sx={{ mb: 2 }}>{reviewSuccess}</Alert>}
      <Card sx={{ mb: 2 }}><CardContent>
        <Typography><strong>{t('booking.reference')}:</strong> {booking.bookingReference}</Typography>
        <Typography><strong>{t('booking.craftsman')}:</strong> {booking.craftsmanName}</Typography>
        <Typography><strong>{t('booking.scheduled')}:</strong> {new Date(booking.scheduledAt).toLocaleString()}</Typography>
        <Typography><strong>{t('booking.price')}:</strong> {formatCurrency(booking.estimatedPrice)}</Typography>
        {booking.cancellationReason && (
          <Typography color="error" sx={{ mt: 1 }}>
            <strong>{t('booking.cancelReason')}:</strong> {booking.cancellationReason}
          </Typography>
        )}
        {booking.customerRating != null && (
          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle2">{t('review.yourReview')}</Typography>
            <Rating value={booking.customerRating} readOnly size="small" />
            {booking.customerReview && (
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>{booking.customerReview}</Typography>
            )}
          </Box>
        )}
      </CardContent></Card>

      {booking.statusHistory.length > 0 && (
        <Card sx={{ mb: 2 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>{t('booking.statusHistory')}</Typography>
            {booking.statusHistory.map((entry, index) => (
              <Box key={`${entry.createdAt}-${index}`} sx={{ mb: 1 }}>
                <Typography variant="body2">
                  {entry.oldStatus ? `${entry.oldStatus} → ` : ''}{entry.newStatus}
                  {' · '}{new Date(entry.createdAt).toLocaleString()}
                </Typography>
                {entry.notes && (
                  <Typography variant="caption" color="text.secondary">{entry.notes}</Typography>
                )}
              </Box>
            ))}
          </CardContent>
        </Card>
      )}

      {actions()}

      <Dialog open={rejectOpen} onClose={() => setRejectOpen(false)}>
        <DialogTitle>{t('booking.reject')}</DialogTitle>
        <DialogContent>
          <TextField fullWidth multiline rows={3} value={reason} onChange={(e) => setReason(e.target.value)} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRejectOpen(false)}>{t('common.cancel')}</Button>
          <Button color="error" disabled={actionLoading || !reason.trim()} onClick={async () => {
            setActionLoading(true);
            try {
              await bookingsApi.reject(booking.id, reason);
              setRejectOpen(false);
              setReason('');
              reload();
            } catch (e) { setError(getApiErrorMessage(e, t('common.error'))); }
            finally { setActionLoading(false); }
          }}>{t('booking.reject')}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={cancelOpen} onClose={() => setCancelOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('booking.cancelTitle')}</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>{t('booking.cancelHint')}</Typography>
          <TextField
            fullWidth
            multiline
            rows={3}
            label={t('booking.cancelReason')}
            value={cancelReason}
            onChange={(e) => setCancelReason(e.target.value)}
            required
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCancelOpen(false)}>{t('common.cancel')}</Button>
          <Button color="error" disabled={actionLoading || !cancelReason.trim()} onClick={handleCancel}>
            {t('booking.confirmCancel')}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={rescheduleOpen} onClose={() => setRescheduleOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('booking.rescheduleTitle')}</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>{t('booking.rescheduleHint')}</Typography>
          <TextField
            type="date"
            label={t('booking.selectDate')}
            fullWidth
            sx={{ mb: 2 }}
            value={rescheduleDate}
            onChange={(e) => setRescheduleDate(e.target.value)}
            slotProps={{ inputLabel: { shrink: true } }}
          />
          <Button variant="outlined" onClick={loadRescheduleSlots} disabled={!rescheduleDate || actionLoading} sx={{ mb: 2 }}>
            {t('booking.loadSlots')}
          </Button>
          <Grid container spacing={1}>
            {rescheduleSlots.map((slot) => (
              <Grid key={slot.start} size={{ xs: 6 }}>
                <Button
                  fullWidth
                  variant={selectedRescheduleSlot?.start === slot.start ? 'contained' : 'outlined'}
                  onClick={() => setSelectedRescheduleSlot(slot)}
                >
                  {new Date(slot.start).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </Button>
              </Grid>
            ))}
          </Grid>
          <TextField
            fullWidth
            multiline
            rows={2}
            label={t('booking.rescheduleReason')}
            value={rescheduleReason}
            onChange={(e) => setRescheduleReason(e.target.value)}
            sx={{ mt: 2 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRescheduleOpen(false)}>{t('common.cancel')}</Button>
          <Button variant="contained" disabled={actionLoading || !selectedRescheduleSlot} onClick={handleReschedule}>
            {t('booking.confirmReschedule')}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={noShowOpen} onClose={() => setNoShowOpen(false)}>
        <DialogTitle>{t('booking.noShowTitle')}</DialogTitle>
        <DialogContent>
          <Typography>{t('booking.noShowHint')}</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setNoShowOpen(false)}>{t('common.cancel')}</Button>
          <Button color="warning" disabled={actionLoading} onClick={handleNoShow}>
            {t('booking.confirmNoShow')}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={reviewOpen} onClose={() => setReviewOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('review.title')}</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>{t('review.hint')}</Typography>
          <Typography component="legend">{t('review.rating')}</Typography>
          <Rating
            value={reviewRating}
            onChange={(_, value) => setReviewRating(value)}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            multiline
            minRows={3}
            label={t('review.comment')}
            value={reviewComment}
            onChange={(e) => setReviewComment(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setReviewOpen(false)}>{t('common.cancel')}</Button>
          <Button variant="contained" disabled={actionLoading || !reviewRating} onClick={handleSubmitReview}>
            {t('review.submit')}
          </Button>
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
  const [payment, setPayment] = useState<BookingPayment | null>(null);
  const [statusMessage, setStatusMessage] = useState<string | null>(null);

  const initiate = async () => {
    if (!id) return;
    setLoading(true);
    setStatusMessage(null);
    try {
      const { data } = await bookingsApi.initiatePayment(id, 'Card');
      setPayment(data.data);
    } finally { setLoading(false); }
  };

  const authorize = async (transactionToken: string) => {
    if (!id || !payment?.attemptId) return;
    setLoading(true);
    setStatusMessage(null);
    try {
      const { data } = await bookingsApi.authorizePayment(id, payment.attemptId, transactionToken);
      const result = data.data;
      if (result.redirectUrl) {
        window.location.href = result.redirectUrl;
        return;
      }
      setStatusMessage(result.message ?? t('booking.paymentAwaitingConfirmation'));
      const refreshed = await bookingsApi.get(id);
      if (refreshed.data.data.status !== 'AwaitingPayment') {
        navigate(`/bookings/${id}`);
      }
    } finally { setLoading(false); }
  };

  const confirm = async () => {
    if (!id || !payment?.sessionId) return;
    setLoading(true);
    try {
      await bookingsApi.confirmPayment(id, payment.sessionId);
      navigate(`/bookings/${id}`);
    } finally { setLoading(false); }
  };

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('booking.payment')}</Typography>
      <Card><CardContent>
        <Typography gutterBottom>{t('booking.paymentPending')}</Typography>
        {payment?.amount != null && <Typography sx={{ mb: 2, fontWeight: 600 }}>{formatCurrency(payment.amount)}</Typography>}
        {!payment ? (
          <Button variant="contained" fullWidth onClick={initiate} disabled={loading}>
            {loading ? <CircularProgress size={24} /> : t('booking.startPayment')}
          </Button>
        ) : payment.requiresClientAuthorizationHandoff && payment.publicIntegrationKey ? (
          <PaymentJsCheckout payment={payment} loading={loading} onAuthorize={authorize} />
        ) : (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
            {payment.checkoutUrl && (
              <Button variant="outlined" href={payment.checkoutUrl} target="_blank" rel="noreferrer">
                {t('booking.openCheckout')}
              </Button>
            )}
            {payment.supportsClientSideConfirmation && (
              <Button variant="contained" fullWidth onClick={confirm} disabled={loading || !payment.sessionId}>
                {loading ? <CircularProgress size={24} /> : t('booking.confirmPayment')}
              </Button>
            )}
          </Box>
        )}
        {statusMessage && <Alert sx={{ mt: 2 }} severity="info">{statusMessage}</Alert>}
      </CardContent></Card>
    </Container>
  );
}

export function PaymentCheckoutPage() {
  const { t } = useTranslation();
  const [searchParams] = useSearchParams();
  const session = searchParams.get('session');
  const amount = searchParams.get('amount');
  const currency = searchParams.get('currency') ?? DEFAULT_CURRENCY;

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>
        {t('booking.checkoutTitle')}
      </Typography>
      {!session ? (
        <Alert severity="error">{t('booking.checkoutMissingSession')}</Alert>
      ) : (
        <Card>
          <CardContent sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <Alert severity="info">{t('booking.checkoutDevNotice')}</Alert>
            <Box>
              <Typography variant="body2" color="text.secondary">{t('booking.checkoutSession')}</Typography>
              <Typography sx={{ fontFamily: 'monospace' }}>{session}</Typography>
            </Box>
            {amount && (
              <Box>
                <Typography variant="body2" color="text.secondary">{t('booking.checkoutAmount')}</Typography>
                <Typography variant="h5" sx={{ fontWeight: 700 }}>{amount} {currency}</Typography>
              </Box>
            )}
            <Typography color="text.secondary">{t('booking.checkoutReturnHint')}</Typography>
          </CardContent>
        </Card>
      )}
    </Container>
  );
}

function statusColor(status: string): 'default' | 'primary' | 'success' | 'warning' | 'error' {
  if (['Confirmed', 'Completed', 'PaymentConfirmed'].includes(status)) return 'success';
  if (['AwaitingPayment', 'PendingCraftsmanConfirmation', 'Pending'].includes(status)) return 'warning';
  if (['Rejected', 'Cancelled', 'Expired', 'NoShow'].includes(status)) return 'error';
  return 'default';
}
