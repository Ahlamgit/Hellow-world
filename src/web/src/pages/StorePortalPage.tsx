import { useEffect, useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Container, FormControlLabel,
  Grid, Switch, TextField, Typography,
} from '@mui/material';
import { storeApi, type StoreProfile, type StoreProduct } from '../services/api';

export function StorePortalPage() {
  const [profile, setProfile] = useState<StoreProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [form, setForm] = useState({ storeName: '', description: '', isOpen: true, openingTime: '08:00', closingTime: '22:00' });
  const [product, setProduct] = useState({ nameEn: '', nameAr: '', price: 0, stockQuantity: 0, isActive: true });

  const load = async () => {
    setLoading(true);
    try {
      const { data } = await storeApi.getProfile();
      const p = data.data;
      setProfile(p);
      setForm({
        storeName: p.storeName,
        description: p.description ?? '',
        isOpen: p.isOpen,
        openingTime: p.openingTime ?? '08:00',
        closingTime: p.closingTime ?? '22:00',
      });
    } catch {
      setError('Failed to load store profile');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const saveProfile = async () => {
    await storeApi.updateProfile(form);
    await load();
  };

  const addProduct = async () => {
    if (!product.nameEn) return;
    await storeApi.createProduct(product);
    setProduct({ nameEn: '', nameAr: '', price: 0, stockQuantity: 0, isActive: true });
    await load();
  };

  if (loading) return <Container sx={{ py: 6 }}><Typography>Loading...</Typography></Container>;
  if (error) return <Container sx={{ py: 6 }}><Alert severity="error">{error}</Alert></Container>;
  if (!profile) return null;

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>Store Portal</Typography>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 5 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Store Profile</Typography>
              <TextField fullWidth label="Store name" value={form.storeName} onChange={(e) => setForm({ ...form, storeName: e.target.value })} sx={{ mb: 2 }} />
              <TextField fullWidth multiline rows={3} label="Description" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} sx={{ mb: 2 }} />
              <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
                <TextField label="Opens" value={form.openingTime} onChange={(e) => setForm({ ...form, openingTime: e.target.value })} />
                <TextField label="Closes" value={form.closingTime} onChange={(e) => setForm({ ...form, closingTime: e.target.value })} />
              </Box>
              <FormControlLabel control={<Switch checked={form.isOpen} onChange={(e) => setForm({ ...form, isOpen: e.target.checked })} />} label="Store open" />
              <Button variant="contained" onClick={saveProfile} sx={{ mt: 2, display: 'block' }}>Save</Button>
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 7 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Products ({profile.products.length})</Typography>
              {profile.products.map((p: StoreProduct) => (
                <Box key={p.id} sx={{ display: 'flex', justifyContent: 'space-between', py: 1, borderBottom: 1, borderColor: 'divider' }}>
                  <Typography>{p.nameEn}</Typography>
                  <Typography>{p.price} SAR · Stock {p.stockQuantity}</Typography>
                </Box>
              ))}
              <Typography variant="subtitle2" sx={{ mt: 2, mb: 1 }}>Add product</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                <TextField size="small" label="Name (EN)" value={product.nameEn} onChange={(e) => setProduct({ ...product, nameEn: e.target.value })} />
                <TextField size="small" label="Name (AR)" value={product.nameAr} onChange={(e) => setProduct({ ...product, nameAr: e.target.value })} />
                <TextField size="small" type="number" label="Price" value={product.price} onChange={(e) => setProduct({ ...product, price: Number(e.target.value) })} sx={{ width: 90 }} />
                <TextField size="small" type="number" label="Stock" value={product.stockQuantity} onChange={(e) => setProduct({ ...product, stockQuantity: Number(e.target.value) })} sx={{ width: 90 }} />
                <Button variant="outlined" onClick={addProduct}>Add</Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
}
