import { useEffect, useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Chip, Container, Tab, Tabs, TextField, Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { supportApi, type Complaint, type SupportTicket } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';

export default function SupportPage() {
  const { t } = useTranslation();
  const [tab, setTab] = useState(0);
  const [submitTab, setSubmitTab] = useState(0);
  const [complaint, setComplaint] = useState({ subject: '', description: '' });
  const [ticket, setTicket] = useState({ subject: '', description: '', category: 'General' });
  const [complaints, setComplaints] = useState<Complaint[]>([]);
  const [tickets, setTickets] = useState<SupportTicket[]>([]);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loadingHistory, setLoadingHistory] = useState(false);

  const loadHistory = async () => {
    setLoadingHistory(true);
    setError('');
    try {
      const [complaintsRes, ticketsRes] = await Promise.all([
        supportApi.myComplaints(),
        supportApi.myTickets(),
      ]);
      setComplaints(complaintsRes.data.data);
      setTickets(ticketsRes.data.data);
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setLoadingHistory(false);
    }
  };

  useEffect(() => {
    if (tab === 1) loadHistory();
  }, [tab]);

  const submitComplaint = async () => {
    setError('');
    try {
      await supportApi.createComplaint(complaint);
      setMessage(t('support.complaintSubmitted'));
      setComplaint({ subject: '', description: '' });
    } catch (e) {
      setError(getApiErrorMessage(e, t('support.complaintFailed')));
    }
  };

  const submitTicket = async () => {
    setError('');
    try {
      await supportApi.createTicket(ticket);
      setMessage(t('support.ticketSubmitted'));
      setTicket({ subject: '', description: '', category: 'General' });
    } catch (e) {
      setError(getApiErrorMessage(e, t('support.ticketFailed')));
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>{t('support.title')}</Typography>
      {message && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setMessage('')}>{message}</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 2 }}>
        <Tab label={t('support.submit')} />
        <Tab label={t('support.history')} />
      </Tabs>

      {tab === 0 ? (
        <Card>
          <CardContent>
            <Tabs value={submitTab} onChange={(_, v) => setSubmitTab(v)} sx={{ mb: 2 }}>
              <Tab label={t('support.complaint')} />
              <Tab label={t('support.ticket')} />
            </Tabs>
            {submitTab === 0 ? (
              <Box>
                <TextField fullWidth label={t('support.subject')} value={complaint.subject} onChange={(e) => setComplaint({ ...complaint, subject: e.target.value })} sx={{ mb: 2 }} />
                <TextField fullWidth multiline rows={4} label={t('support.description')} value={complaint.description} onChange={(e) => setComplaint({ ...complaint, description: e.target.value })} sx={{ mb: 2 }} />
                <Button variant="contained" onClick={submitComplaint} disabled={!complaint.subject.trim() || !complaint.description.trim()}>
                  {t('support.submitComplaint')}
                </Button>
              </Box>
            ) : (
              <Box>
                <TextField fullWidth label={t('support.subject')} value={ticket.subject} onChange={(e) => setTicket({ ...ticket, subject: e.target.value })} sx={{ mb: 2 }} />
                <TextField fullWidth label={t('support.category')} value={ticket.category} onChange={(e) => setTicket({ ...ticket, category: e.target.value })} sx={{ mb: 2 }} />
                <TextField fullWidth multiline rows={4} label={t('support.description')} value={ticket.description} onChange={(e) => setTicket({ ...ticket, description: e.target.value })} sx={{ mb: 2 }} />
                <Button variant="contained" onClick={submitTicket} disabled={!ticket.subject.trim() || !ticket.description.trim()}>
                  {t('support.submitTicket')}
                </Button>
              </Box>
            )}
          </CardContent>
        </Card>
      ) : (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
          {loadingHistory ? (
            <Typography>{t('common.loading')}</Typography>
          ) : (
            <>
              <Box>
                <Typography variant="h6" gutterBottom>{t('support.myComplaints')}</Typography>
                {complaints.length === 0 ? (
                  <Typography color="text.secondary">{t('support.noComplaints')}</Typography>
                ) : (
                  complaints.map((item) => (
                    <Card key={item.id} sx={{ mb: 1 }}>
                      <CardContent>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2, mb: 1 }}>
                          <Typography sx={{ fontWeight: 600 }}>{item.subject}</Typography>
                          <Chip label={item.status} size="small" />
                        </Box>
                        <Typography variant="body2" color="text.secondary">{item.description}</Typography>
                        <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
                          {new Date(item.createdAt).toLocaleString()}
                        </Typography>
                      </CardContent>
                    </Card>
                  ))
                )}
              </Box>
              <Box>
                <Typography variant="h6" gutterBottom>{t('support.myTickets')}</Typography>
                {tickets.length === 0 ? (
                  <Typography color="text.secondary">{t('support.noTickets')}</Typography>
                ) : (
                  tickets.map((item) => (
                    <Card key={item.id} sx={{ mb: 1 }}>
                      <CardContent>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2, mb: 1 }}>
                          <Typography sx={{ fontWeight: 600 }}>{item.ticketNumber} — {item.subject}</Typography>
                          <Chip label={item.status} size="small" />
                        </Box>
                        <Typography variant="body2" color="text.secondary">{item.description}</Typography>
                        <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
                          {item.category} · {new Date(item.createdAt).toLocaleString()}
                        </Typography>
                      </CardContent>
                    </Card>
                  ))
                )}
              </Box>
            </>
          )}
        </Box>
      )}
    </Container>
  );
}
