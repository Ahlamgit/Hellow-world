import { useEffect, useState } from 'react';
import {
  Box, Card, CardContent, TextField, Button, Typography, Alert, Link as MuiLink,
} from '@mui/material';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { authApi, identityApi, type SessionDto } from '../services/api';
import { DevActionLinkAlert } from '../components/DevActionLinkAlert';
import { getApiErrorMessage } from '../utils/apiError';
import { useAuth } from '../context/AuthContext';

function AuthCard({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '70vh', px: 2, py: 4 }}>
      <Card sx={{ width: '100%', maxWidth: 440 }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>{title}</Typography>
          {children}
        </CardContent>
      </Card>
    </Box>
  );
}

export function ForgotPasswordPage() {
  const { t } = useTranslation();
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [actionLink, setActionLink] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);
    try {
      const { data } = await authApi.forgotPassword(email);
      setSuccess(data.data.message || t('identity.forgotPasswordSuccess'));
      setActionLink(data.data.actionLink ?? '');
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthCard title={t('identity.forgotPasswordTitle')}>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        {t('identity.forgotPasswordHint')}
      </Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
      <DevActionLinkAlert link={actionLink} label={t('identity.devResetLinkHint')} />
      <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField label={t('auth.email')} type="email" value={email} onChange={(e) => setEmail(e.target.value)} required fullWidth />
        <Button type="submit" variant="contained" size="large" disabled={loading}>
          {loading ? t('common.loading') : t('identity.sendResetLink')}
        </Button>
      </Box>
      <Typography variant="body2" sx={{ mt: 2, textAlign: 'center' }}>
        <MuiLink component={Link} to="/login">{t('identity.backToLogin')}</MuiLink>
      </Typography>
    </AuthCard>
  );
}

export function ResetPasswordPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const params = new URLSearchParams(window.location.search);
  const tokenFromUrl = params.get('token') ?? '';
  const [token, setToken] = useState(tokenFromUrl);
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (newPassword !== confirmPassword) {
      setError(t('identity.passwordMismatch'));
      return;
    }
    setLoading(true);
    try {
      const { data } = await authApi.resetPassword(token, newPassword, confirmPassword);
      setSuccess(data.data.message || t('identity.resetPasswordSuccess'));
      setTimeout(() => navigate('/login'), 2000);
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthCard title={t('identity.resetPasswordTitle')}>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
      <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        {!tokenFromUrl && (
          <TextField label={t('identity.resetToken')} value={token} onChange={(e) => setToken(e.target.value)} required fullWidth />
        )}
        <TextField label={t('identity.newPassword')} type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} required fullWidth />
        <TextField label={t('identity.confirmPassword')} type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required fullWidth />
        <Button type="submit" variant="contained" size="large" disabled={loading || !token}>
          {loading ? t('common.loading') : t('identity.resetPassword')}
        </Button>
      </Box>
      <Typography variant="body2" sx={{ mt: 2, textAlign: 'center' }}>
        <MuiLink component={Link} to="/login">{t('identity.backToLogin')}</MuiLink>
      </Typography>
    </AuthCard>
  );
}

export function VerifyEmailPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();
  const { user, isAuthenticated } = useAuth();
  const params = new URLSearchParams(window.location.search);
  const tokenFromUrl = params.get('token') ?? '';
  const initialActionLink = (location.state as { actionLink?: string } | null)?.actionLink ?? '';
  const [token, setToken] = useState(tokenFromUrl);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [resendLoading, setResendLoading] = useState(false);
  const [resendMessage, setResendMessage] = useState('');
  const [actionLink, setActionLink] = useState(initialActionLink);

  const verify = async (verifyToken: string) => {
    setError('');
    setLoading(true);
    try {
      const { data } = await authApi.verifyEmail(verifyToken);
      setSuccess(data.data.message || t('identity.verifyEmailSuccess'));
      setTimeout(() => navigate(isAuthenticated ? '/dashboard' : '/login'), 2000);
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (tokenFromUrl) verify(tokenFromUrl);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [tokenFromUrl]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await verify(token);
  };

  const handleResend = async () => {
    const email = user?.email;
    if (!email) {
      setResendMessage(t('identity.resendRequiresLogin'));
      return;
    }
    setResendLoading(true);
    setResendMessage('');
    try {
      const { data } = await authApi.resendEmailVerification(email);
      setResendMessage(data.data.message || t('identity.resendSuccess'));
      setActionLink(data.data.actionLink ?? '');
    } catch (err) {
      setResendMessage(getApiErrorMessage(err, t('common.error')));
    } finally {
      setResendLoading(false);
    }
  };

  return (
    <AuthCard title={t('identity.verifyEmailTitle')}>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        {tokenFromUrl ? t('identity.verifyingEmail') : t('identity.verifyEmailHint')}
      </Typography>
      <DevActionLinkAlert link={actionLink} label={t('identity.devVerificationLinkHint')} />
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
      {resendMessage && <Alert severity="info" sx={{ mb: 2 }}>{resendMessage}</Alert>}
      {!tokenFromUrl && !success && (
        <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <TextField label={t('identity.verificationToken')} value={token} onChange={(e) => setToken(e.target.value)} required fullWidth />
          <Button type="submit" variant="contained" size="large" disabled={loading || !token}>
            {loading ? t('common.loading') : t('identity.verifyEmail')}
          </Button>
        </Box>
      )}
      {loading && tokenFromUrl && <Typography align="center">{t('common.loading')}</Typography>}
      {!success && (
        <Box sx={{ mt: 2, textAlign: 'center' }}>
          <Button onClick={handleResend} disabled={resendLoading} size="small">
            {resendLoading ? t('common.loading') : t('identity.resendVerification')}
          </Button>
        </Box>
      )}
    </AuthCard>
  );
}

export function ChangePasswordPage() {
  const { t } = useTranslation();
  const { logout } = useAuth();
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (newPassword !== confirmPassword) {
      setError(t('identity.passwordMismatch'));
      return;
    }
    setLoading(true);
    try {
      const { data } = await authApi.changePassword(currentPassword, newPassword, confirmPassword);
      setSuccess(data.data.message || t('identity.changePasswordSuccess'));
      setTimeout(() => logout(), 2000);
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthCard title={t('identity.changePasswordTitle')}>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        {t('identity.changePasswordHint')}
      </Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
      <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField label={t('identity.currentPassword')} type="password" value={currentPassword} onChange={(e) => setCurrentPassword(e.target.value)} required fullWidth />
        <TextField label={t('identity.newPassword')} type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} required fullWidth />
        <TextField label={t('identity.confirmPassword')} type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required fullWidth />
        <Button type="submit" variant="contained" size="large" disabled={loading}>
          {loading ? t('common.loading') : t('identity.changePassword')}
        </Button>
      </Box>
    </AuthCard>
  );
}

export function SessionsPage() {
  const { t } = useTranslation();
  const { logout } = useAuth();
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const [actionId, setActionId] = useState<string | null>(null);

  const loadSessions = async () => {
    setLoading(true);
    setError('');
    try {
      const { data } = await identityApi.getSessions();
      setSessions(data.data);
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadSessions(); }, []);

  const revokeSession = async (sessionId: string) => {
    setActionId(sessionId);
    setMessage('');
    try {
      await identityApi.revokeSession(sessionId);
      setMessage(t('identity.sessionRevoked'));
      const current = sessions.find((s) => s.isCurrent);
      if (current?.id === sessionId) {
        logout();
        return;
      }
      await loadSessions();
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setActionId(null);
    }
  };

  const revokeOthers = async () => {
    setActionId('others');
    setMessage('');
    try {
      await identityApi.revokeOtherSessions();
      setMessage(t('identity.otherSessionsRevoked'));
      await loadSessions();
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setActionId(null);
    }
  };

  const revokeAll = async () => {
    setActionId('all');
    setMessage('');
    try {
      await identityApi.revokeAllSessions();
      logout();
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
      setActionId(null);
    }
  };

  const formatDate = (value?: string) => value ? new Date(value).toLocaleString() : '—';

  return (
    <Box sx={{ maxWidth: 800, mx: 'auto', py: 6, px: 2 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('identity.sessionsTitle')}</Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>{t('identity.sessionsHint')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {message && <Alert severity="success" sx={{ mb: 2 }}>{message}</Alert>}
      <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 3 }}>
        <Button variant="outlined" onClick={revokeOthers} disabled={!!actionId || sessions.length <= 1}>
          {actionId === 'others' ? t('common.loading') : t('identity.logoutOtherDevices')}
        </Button>
        <Button variant="outlined" color="error" onClick={revokeAll} disabled={!!actionId}>
          {actionId === 'all' ? t('common.loading') : t('identity.logoutAllDevices')}
        </Button>
      </Box>
      {loading ? (
        <Typography>{t('common.loading')}</Typography>
      ) : (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          {sessions.map((session) => (
            <Card key={session.id} variant="outlined">
              <CardContent sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: 2, flexWrap: 'wrap' }}>
                <Box>
                  <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
                    {session.deviceName || session.browser || t('identity.unknownDevice')}
                    {session.isCurrent && ` (${t('identity.currentSession')})`}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {[session.platform, session.browser, session.ipAddress].filter(Boolean).join(' · ')}
                  </Typography>
                  <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
                    {t('identity.lastActive')}: {formatDate(session.lastActivityAt ?? session.createdAt)}
                  </Typography>
                </Box>
                <Button
                  size="small"
                  color="error"
                  variant="outlined"
                  disabled={!!actionId}
                  onClick={() => revokeSession(session.id)}
                >
                  {actionId === session.id ? t('common.loading') : t('identity.revokeSession')}
                </Button>
              </CardContent>
            </Card>
          ))}
          {sessions.length === 0 && <Typography color="text.secondary">{t('identity.noSessions')}</Typography>}
        </Box>
      )}
    </Box>
  );
}
