package com.khadamati.app.ui.screens

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.khadamati.app.ui.viewmodel.BookingViewModel

@Composable
fun MyBookingsScreen(
    viewModel: BookingViewModel,
    onBookingClick: (String) -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    LaunchedEffect(Unit) { viewModel.loadBookings() }

    if (state.isLoading) {
        CircularProgressIndicator(Modifier.padding(32.dp))
        return
    }

    LazyColumn(
        modifier = Modifier.fillMaxSize(),
        contentPadding = PaddingValues(16.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp),
    ) {
        items(state.bookings) { booking ->
            Card(
                modifier = Modifier.fillMaxWidth().clickable { onBookingClick(booking.id) },
            ) {
                Column(Modifier.padding(16.dp)) {
                    Text(booking.serviceName, style = MaterialTheme.typography.titleMedium)
                    Text(booking.bookingReference, style = MaterialTheme.typography.bodySmall)
                    Text(booking.status, color = MaterialTheme.colorScheme.primary)
                }
            }
        }
    }
}

@Composable
fun BookingDetailScreen(
    bookingId: String,
    viewModel: BookingViewModel,
    userRole: String,
    onPay: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    LaunchedEffect(bookingId) {
        viewModel.loadBookings()
    }
    val booking = state.bookings.find { it.id == bookingId }

    Column(Modifier.fillMaxSize().padding(16.dp), verticalArrangement = Arrangement.spacedBy(12.dp)) {
        if (booking == null) {
            CircularProgressIndicator()
            return@Column
        }
        Text(booking.serviceName, style = MaterialTheme.typography.headlineSmall)
        Text("${booking.bookingReference} · ${booking.status}")
        Text("${booking.craftsmanName} · ${booking.estimatedPrice} SAR")
        when {
            booking.status == "AwaitingPayment" && userRole == "Customer" ->
                Button(onClick = { viewModel.pay(bookingId); onPay() }, modifier = Modifier.fillMaxWidth()) { Text("Pay Now") }
            booking.status == "PendingCraftsmanConfirmation" && userRole == "Craftsman" -> {
                Button(onClick = { viewModel.accept(bookingId) }, modifier = Modifier.fillMaxWidth()) { Text("Accept") }
                Button(onClick = { viewModel.reject(bookingId, "Unavailable") }, modifier = Modifier.fillMaxWidth()) { Text("Reject") }
            }
        }
    }
}
