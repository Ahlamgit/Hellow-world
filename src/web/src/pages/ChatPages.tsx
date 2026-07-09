import { useCallback, useEffect, useRef, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import {
  Alert, Box, Card, CardContent, CircularProgress, Container,
  IconButton, TextField, Typography,
} from '@mui/material';
import { ArrowBack, Send } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { chatApi, type ChatConversation, type ChatMessage } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';

export function ChatListPage() {
  const { t } = useTranslation();
  const [conversations, setConversations] = useState<ChatConversation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    (async () => {
      try {
        const { data } = await chatApi.listConversations();
        setConversations(data.data);
      } catch (e) {
        setError(getApiErrorMessage(e, t('common.error')));
      } finally {
        setLoading(false);
      }
    })();
  }, [t]);

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('chat.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}><CircularProgress /></Box>
      ) : conversations.length === 0 ? (
        <Typography color="text.secondary">{t('chat.empty')}</Typography>
      ) : (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
          {conversations.map((conversation) => (
            <Card
              key={conversation.id}
              component={Link}
              to={`/chat/${conversation.bookingId}`}
              sx={{ textDecoration: 'none', color: 'inherit' }}
            >
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                  <Box>
                    <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>{conversation.serviceName}</Typography>
                    <Typography variant="body2" color="text.secondary">{conversation.bookingReference}</Typography>
                    {conversation.lastMessagePreview && (
                      <Typography variant="body2" sx={{ mt: 1 }} noWrap>{conversation.lastMessagePreview}</Typography>
                    )}
                  </Box>
                  {conversation.unreadCount > 0 && (
                    <Typography variant="caption" color="primary.main" sx={{ fontWeight: 700 }}>
                      {conversation.unreadCount}
                    </Typography>
                  )}
                </Box>
              </CardContent>
            </Card>
          ))}
        </Box>
      )}
    </Container>
  );
}

export function BookingChatPage() {
  const { bookingId } = useParams<{ bookingId: string }>();
  const { t } = useTranslation();
  const navigate = useNavigate();
  const bottomRef = useRef<HTMLDivElement>(null);
  const [conversation, setConversation] = useState<ChatConversation | null>(null);
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [draft, setDraft] = useState('');
  const [loading, setLoading] = useState(true);
  const [sending, setSending] = useState(false);
  const [error, setError] = useState('');

  const loadMessages = useCallback(async (conversationId: string) => {
    const { data } = await chatApi.getMessages(conversationId);
    setMessages(data.data.items);
  }, []);

  useEffect(() => {
    if (!bookingId) return;
    (async () => {
      try {
        const { data } = await chatApi.getBookingChat(bookingId);
        setConversation(data.data);
        await loadMessages(data.data.id);
      } catch (e) {
        setError(getApiErrorMessage(e, t('common.error')));
      } finally {
        setLoading(false);
      }
    })();
  }, [bookingId, loadMessages, t]);

  useEffect(() => {
    if (!conversation) return;
    const interval = window.setInterval(() => {
      loadMessages(conversation.id).catch(() => undefined);
    }, 5000);
    return () => window.clearInterval(interval);
  }, [conversation, loadMessages]);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSend = async () => {
    if (!conversation || !draft.trim()) return;
    setSending(true);
    setError('');
    try {
      const { data } = await chatApi.sendMessage(conversation.id, draft.trim());
      setMessages((prev) => [...prev, data.data]);
      setDraft('');
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setSending(false);
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4, display: 'flex', flexDirection: 'column', minHeight: '70vh' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <IconButton onClick={() => navigate('/chat')} aria-label={t('common.back')}>
          <ArrowBack />
        </IconButton>
        <Box>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            {conversation?.serviceName ?? t('chat.title')}
          </Typography>
          {conversation?.bookingReference && (
            <Typography variant="body2" color="text.secondary">{conversation.bookingReference}</Typography>
          )}
        </Box>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Box sx={{ flexGrow: 1, overflowY: 'auto', mb: 2 }}>
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}><CircularProgress /></Box>
        ) : (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
            {messages.map((message) => (
              <Box
                key={message.id}
                sx={{ display: 'flex', justifyContent: message.isMine ? 'flex-end' : 'flex-start' }}
              >
                <Box
                  sx={{
                    maxWidth: '75%',
                    px: 2,
                    py: 1.5,
                    borderRadius: 2,
                    bgcolor: message.isMine ? 'primary.light' : 'action.hover',
                  }}
                >
                  {!message.isMine && (
                    <Typography variant="caption" color="text.secondary">{message.senderName}</Typography>
                  )}
                  <Typography>{message.body}</Typography>
                </Box>
              </Box>
            ))}
            <div ref={bottomRef} />
          </Box>
        )}
      </Box>

      <Box sx={{ display: 'flex', gap: 1 }}>
        <TextField
          fullWidth
          multiline
          maxRows={3}
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          placeholder={t('chat.messageHint')}
          onKeyDown={(e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
              e.preventDefault();
              handleSend();
            }
          }}
        />
        <IconButton color="primary" onClick={handleSend} disabled={sending || !draft.trim()}>
          <Send />
        </IconButton>
      </Box>
    </Container>
  );
}
