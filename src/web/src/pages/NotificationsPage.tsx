import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Alert, Box, Button, Card, CardActionArea, CardContent, Chip, CircularProgress,
  Container, Pagination, Tab, Tabs, Typography,
} from '@mui/material';
import { NotificationsNone } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { notificationsApi, type Notification } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';

type FilterTab = 'all' | 'unread';

function notificationLink(notification: Notification): string | null {
  if (!notification.referenceId) return null;
  if (notification.notificationType.startsWith('Booking')) {
    return `/bookings/${notification.referenceId}`;
  }
  return null;
}

export default function NotificationsPage() {
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';
  const navigate = useNavigate();

  const [filter, setFilter] = useState<FilterTab>('all');
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const pageSize = 15;

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const res = await notificationsApi.list({
        unreadOnly: filter === 'unread',
        page,
        pageSize,
      });
      setNotifications(res.data.data.items);
      setTotalCount(res.data.data.totalCount);
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setLoading(false);
    }
  }, [filter, page, pageSize, t]);

  useEffect(() => { load(); }, [load]);

  const handleOpen = async (notification: Notification) => {
    if (!notification.isRead) {
      try {
        await notificationsApi.markRead(notification.id);
        setNotifications((prev) =>
          prev.map((n) => (n.id === notification.id ? { ...n, isRead: true } : n)),
        );
      } catch {
        // still navigate even if mark-read fails
      }
    }
    const link = notificationLink(notification);
    if (link) navigate(link);
  };

  const handleMarkAllRead = async () => {
    const unread = notifications.filter((n) => !n.isRead);
    if (unread.length === 0) return;
    setLoading(true);
    try {
      await Promise.all(unread.map((n) => notificationsApi.markRead(n.id)));
      await load();
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
      setLoading(false);
    }
  };

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4" sx={{ fontWeight: 700 }}>{t('notifications.title')}</Typography>
        {notifications.some((n) => !n.isRead) && (
          <Button size="small" onClick={handleMarkAllRead} disabled={loading}>
            {t('notifications.markAllRead')}
          </Button>
        )}
      </Box>

      <Tabs
        value={filter}
        onChange={(_, value: FilterTab) => { setFilter(value); setPage(1); }}
        sx={{ mb: 2 }}
      >
        <Tab value="all" label={t('notifications.all')} />
        <Tab value="unread" label={t('notifications.unread')} />
      </Tabs>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
          <CircularProgress />
        </Box>
      ) : notifications.length === 0 ? (
        <Box sx={{ textAlign: 'center', py: 8 }}>
          <NotificationsNone sx={{ fontSize: 48, color: 'text.disabled', mb: 2 }} />
          <Typography color="text.secondary">
            {filter === 'unread' ? t('notifications.emptyUnread') : t('notifications.empty')}
          </Typography>
        </Box>
      ) : (
        <>
          {notifications.map((notification) => {
            const title = isAr ? notification.titleAr : notification.titleEn;
            const message = isAr ? notification.messageAr : notification.messageEn;
            const link = notificationLink(notification);

            return (
              <Card
                key={notification.id}
                sx={{
                  mb: 1.5,
                  borderLeft: notification.isRead ? undefined : 4,
                  borderColor: notification.isRead ? undefined : 'primary.main',
                  bgcolor: notification.isRead ? 'background.paper' : 'action.hover',
                }}
              >
                <CardActionArea onClick={() => handleOpen(notification)}>
                  <CardContent>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2, mb: 0.5 }}>
                      <Typography sx={{ fontWeight: notification.isRead ? 500 : 700 }}>
                        {title}
                      </Typography>
                      <Typography variant="caption" color="text.secondary" sx={{ flexShrink: 0 }}>
                        {new Date(notification.createdAt).toLocaleString()}
                      </Typography>
                    </Box>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                      {message}
                    </Typography>
                    <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                      <Chip label={notification.notificationType} size="small" variant="outlined" />
                      {!notification.isRead && (
                        <Chip label={t('notifications.unreadBadge')} size="small" color="primary" />
                      )}
                      {link && (
                        <Typography variant="caption" color="primary">
                          {t('notifications.viewBooking')}
                        </Typography>
                      )}
                    </Box>
                  </CardContent>
                </CardActionArea>
              </Card>
            );
          })}

          {totalPages > 1 && (
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
              <Pagination
                count={totalPages}
                page={page}
                onChange={(_, p) => setPage(p)}
                color="primary"
              />
            </Box>
          )}
        </>
      )}
    </Container>
  );
}
