import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  FormHelperText,
  Radio,
  RadioGroup,
  TextField,
  Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { fetchLegalDocument } from '../auth/legalApi';
import { savePendingRegistration } from '../auth/pendingRegistration';
import {
  registerTypeToApiRole,
  validateConfirmPassword,
  validatePassword,
  type RegisterAccountType,
} from '../auth/passwordValidation';
import { sanitizeReturnUrl, withReturnUrl } from '../auth/redirects';
import { AuthPageLayout } from '../components/auth/AuthPageLayout';
import { PasswordField } from '../components/auth/PasswordField';

type Step = 'type' | 'form' | 'legal';

export function RegisterPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const { register } = useAuth();

  const returnUrl = sanitizeReturnUrl(params.get('returnUrl'));
  const initialType = params.get('type') as RegisterAccountType | null;
  const lang = i18n.language.startsWith('ar') ? 'ar' : 'en';

  const [step, setStep] = useState<Step>(initialType ? 'form' : 'type');
  const [accountType, setAccountType] = useState<RegisterAccountType | ''>(initialType ?? '');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [acceptPrivacy, setAcceptPrivacy] = useState(false);
  const [termsVersion, setTermsVersion] = useState('');
  const [privacyVersion, setPrivacyVersion] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const passwordCheck = validatePassword(password);
  const passwordsMatch = validateConfirmPassword(password, confirmPassword);
  const legalComplete = acceptTerms && acceptPrivacy && termsVersion && privacyVersion;

  useEffect(() => {
    if (step !== 'legal') return;
    void fetchLegalDocument('terms', lang).then((d) => setTermsVersion(d.version));
    void fetchLegalDocument('privacy', lang).then((d) => setPrivacyVersion(d.version));
  }, [step, lang]);

  const onSelectType = () => {
    if (!accountType) {
      setError(t('auth.selectAccountType'));
      return;
    }
    setError(null);
    setStep('form');
  };

  const onFormContinue = (event: React.FormEvent) => {
    event.preventDefault();
    if (!passwordCheck.valid) {
      setError(t('auth.passwordWeak'));
      return;
    }
    if (!passwordsMatch) {
      setError(t('auth.passwordMismatch'));
      return;
    }
    setError(null);
    setStep('legal');
  };

  const onLegalSubmit = async () => {
    if (!legalComplete || !accountType) {
      setError(t('auth.acceptPoliciesRequired'));
      return;
    }
    setError(null);
    setSubmitting(true);
    try {
      const phoneE164 = phone.trim().startsWith('+') ? phone.trim() : `+${phone.trim().replace(/\D/g, '')}`;
      const pending = await register({
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        phoneE164,
        email: email.trim(),
        password,
        confirmPassword,
        role: registerTypeToApiRole(accountType),
        acceptTerms,
        acceptPrivacy,
        termsVersion,
        privacyVersion,
        language: lang,
      });
      savePendingRegistration({
        email: pending.email,
        role: pending.role,
        phoneE164: pending.phoneE164,
        returnUrl: returnUrl ?? undefined,
      });
      navigate('/register/verify', { replace: true });
    } catch (err) {
      setError(err instanceof Error ? err.message : t('common.error'));
    } finally {
      setSubmitting(false);
    }
  };

  const legalReturn = withReturnUrl('/register', returnUrl);

  const subtitle =
    step === 'type'
      ? t('auth.chooseAccountType')
      : step === 'form'
        ? t('auth.registerSubtitle')
        : t('legal.reviewBeforeContinue');

  return (
    <AuthPageLayout maxWidth="md">
      <Button component={Link} to="/" startIcon={<ArrowBackIcon />} sx={{ mb: 3 }}>
        {t('login.browseWithout')}
      </Button>

      <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }} align="center">
        {t('auth.createAccount')}
      </Typography>
      <Typography color="text.secondary" align="center" sx={{ mb: 4 }}>
        {subtitle}
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {step === 'type' && (
        <Box sx={{ p: 3, borderRadius: 3, border: 1, borderColor: 'divider', bgcolor: 'background.paper' }}>
          <RadioGroup
            value={accountType}
            onChange={(e) => setAccountType(e.target.value as RegisterAccountType)}
            sx={{ gap: 1 }}
          >
            {(['customer', 'provider', 'store'] as RegisterAccountType[]).map((type) => (
              <FormControlLabel
                key={type}
                value={type}
                control={<Radio />}
                label={
                  <Box>
                    <Typography sx={{ fontWeight: 700 }}>{t(`roles.${type}`)}</Typography>
                    <Typography variant="body2" color="text.secondary">{t(`auth.${type}Desc`)}</Typography>
                  </Box>
                }
                sx={{ mx: 0, px: 2, py: 1.5, borderRadius: 2, border: 1, borderColor: 'divider', width: '100%' }}
              />
            ))}
          </RadioGroup>
          <Button variant="contained" size="large" fullWidth sx={{ mt: 3 }} onClick={onSelectType}>
            {t('auth.continue')}
          </Button>
        </Box>
      )}

      {step === 'form' && (
        <Box
          component="form"
          onSubmit={onFormContinue}
          sx={{ p: 3, borderRadius: 3, border: 1, borderColor: 'divider', bgcolor: 'background.paper', display: 'grid', gap: 2 }}
        >
          <Button size="small" onClick={() => setStep('type')} sx={{ justifySelf: 'start' }}>
            {t('auth.changeAccountType')}
          </Button>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
            <TextField label={t('auth.firstName')} required value={firstName} onChange={(e) => setFirstName(e.target.value)} fullWidth />
            <TextField label={t('auth.lastName')} required value={lastName} onChange={(e) => setLastName(e.target.value)} fullWidth />
          </Box>
          <TextField label={t('auth.phone')} required value={phone} onChange={(e) => setPhone(e.target.value)} fullWidth placeholder="+961..." helperText={t('auth.phoneHint')} />
          <TextField label={t('login.email')} type="email" required value={email} onChange={(e) => setEmail(e.target.value)} fullWidth />
          <PasswordField label={t('login.password')} required value={password} onChange={(e) => setPassword(e.target.value)} fullWidth helperText={t('auth.passwordRules')} />
          {password && !passwordCheck.valid && (
            <FormHelperText error>
              {passwordCheck.errors.map((key) => t(`auth.passwordError.${key}`)).join(' · ')}
            </FormHelperText>
          )}
          <PasswordField
            label={t('auth.confirmPassword')}
            required
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            fullWidth
            error={confirmPassword.length > 0 && !passwordsMatch}
          />
          <Button type="submit" variant="contained" size="large">
            {t('auth.continue')}
          </Button>
        </Box>
      )}

      {step === 'legal' && (
        <Box sx={{ p: 3, borderRadius: 3, border: 1, borderColor: 'divider', bgcolor: 'background.paper', display: 'grid', gap: 2 }}>
          <Typography color="text.secondary">{t('legal.reviewPrompt')}</Typography>
          <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 1 }}>
            <Button component={Link} to={`/legal/terms?returnUrl=${encodeURIComponent(legalReturn)}`} variant="outlined" fullWidth>
              {t('legal.viewTerms')}
            </Button>
            <Button component={Link} to={`/legal/privacy?returnUrl=${encodeURIComponent(legalReturn)}`} variant="outlined" fullWidth>
              {t('legal.viewPrivacy')}
            </Button>
          </Box>
          <FormControlLabel
            control={<Checkbox checked={acceptTerms} onChange={(e) => setAcceptTerms(e.target.checked)} />}
            label={t('legal.acceptTermsLabel')}
          />
          <FormControlLabel
            control={<Checkbox checked={acceptPrivacy} onChange={(e) => setAcceptPrivacy(e.target.checked)} />}
            label={t('legal.acceptPrivacyLabel')}
          />
          <Button variant="contained" size="large" disabled={!legalComplete || submitting} onClick={onLegalSubmit}>
            {submitting ? t('common.loading') : t('auth.createAccount')}
          </Button>
          <Button size="small" onClick={() => setStep('form')}>{t('common.back')}</Button>
        </Box>
      )}

      <Typography align="center" sx={{ mt: 3 }}>
        {t('auth.alreadyHaveAccount')}{' '}
        <Link to={withReturnUrl('/login', returnUrl)}>{t('nav.signIn')}</Link>
      </Typography>
    </AuthPageLayout>
  );
}
