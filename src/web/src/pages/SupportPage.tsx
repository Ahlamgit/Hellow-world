import { useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Container, Tab, Tabs, TextField, Typography,
} from '@mui/material';
import { supportApi } from '../services/api';

export default function SupportPage() {
  const [tab, setTab] = useState(0);
  const [complaint, setComplaint] = useState({ subject: '', description: '' });
  const [ticket, setTicket] = useState({ subject: '', description: '', category: 'General' });
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const submitComplaint = async () => {
    setError('');
    try {
      await supportApi.createComplaint(complaint);
      setMessage('Complaint submitted successfully.');
      setComplaint({ subject: '', description: '' });
    } catch {
      setError('Failed to submit complaint');
    }
  };

  const submitTicket = async () => {
    setError('');
    try {
      await supportApi.createTicket(ticket);
      setMessage('Support ticket created.');
      setTicket({ subject: '', description: '', category: 'General' });
    } catch {
      setError('Failed to create ticket');
    }
  };

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>Help & Support</Typography>
      {message && <Alert severity="success" sx={{ mb: 2 }}>{message}</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 2 }}>
        <Tab label="Complaint" />
        <Tab label="Support Ticket" />
      </Tabs>
      <Card>
        <CardContent>
          {tab === 0 ? (
            <Box>
              <TextField fullWidth label="Subject" value={complaint.subject} onChange={(e) => setComplaint({ ...complaint, subject: e.target.value })} sx={{ mb: 2 }} />
              <TextField fullWidth multiline rows={4} label="Description" value={complaint.description} onChange={(e) => setComplaint({ ...complaint, description: e.target.value })} sx={{ mb: 2 }} />
              <Button variant="contained" onClick={submitComplaint}>Submit complaint</Button>
            </Box>
          ) : (
            <Box>
              <TextField fullWidth label="Subject" value={ticket.subject} onChange={(e) => setTicket({ ...ticket, subject: e.target.value })} sx={{ mb: 2 }} />
              <TextField fullWidth multiline rows={4} label="Description" value={ticket.description} onChange={(e) => setTicket({ ...ticket, description: e.target.value })} sx={{ mb: 2 }} />
              <Button variant="contained" onClick={submitTicket}>Create ticket</Button>
            </Box>
          )}
        </CardContent>
      </Card>
    </Container>
  );
}
