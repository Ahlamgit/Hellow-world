package com.khadamati.app.ui.screens

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material3.FloatingActionButton
import androidx.compose.material3.Icon
import androidx.compose.material3.Scaffold
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import android.content.Intent
import android.net.Uri
import com.khadamati.app.R
import com.khadamati.app.ui.viewmodel.BookingViewModel

@Composable
fun MyBookingsScreen(
    viewModel: BookingViewModel,
    onBookingClick: (String) -> Unit,
    onNewBooking: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    LaunchedEffect(Unit) { viewModel.loadBookings() }

    Scaffold(
        floatingActionButton = {
            FloatingActionButton(onClick = onNewBooking) {
                Icon(Icons.Default.Add, contentDescription = stringResource(R.string.booking_new))
            }
        },
    ) { padding ->
        if (state.isLoading && state.bookings.isEmpty()) {
            Box(Modifier.fillMaxSize().padding(padding), contentAlignment = Alignment.Center) {
                CircularProgressIndicator()
            }
            return@Scaffold
        }

        LazyColumn(
            modifier = Modifier.fillMaxSize().padding(padding),
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
}

@Composable
fun BookingDetailScreen(
    bookingId: String,
    viewModel: BookingViewModel,
    userRole: String,
    onPay: () -> Unit,
    onOpenChat: (String) -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    var showCancelDialog by remember { mutableStateOf(false) }
    var showRejectDialog by remember { mutableStateOf(false) }
    var showRescheduleDialog by remember { mutableStateOf(false) }
    var showNoShowDialog by remember { mutableStateOf(false) }
    var showReviewDialog by remember { mutableStateOf(false) }
    var reviewRating by remember { mutableStateOf(5) }
    var reviewComment by remember { mutableStateOf("") }
    var reasonText by remember { mutableStateOf("") }
    var rescheduleDate by remember { mutableStateOf("") }
    var selectedSlot by remember { mutableStateOf<String?>(null) }

    LaunchedEffect(bookingId) {
        viewModel.loadBooking(bookingId)
    }
    val booking = state.selectedBooking
    val context = LocalContext.current
    LaunchedEffect(state.checkoutUrl) {
        val url = state.checkoutUrl ?: return@LaunchedEffect
        runCatching {
            context.startActivity(Intent(Intent.ACTION_VIEW, Uri.parse(url)))
        }
        viewModel.consumeCheckoutUrl()
    }

    Column(
        Modifier
            .fillMaxSize()
            .padding(16.dp)
            .verticalScroll(rememberScrollState()),
        verticalArrangement = Arrangement.spacedBy(12.dp),
    ) {
        if (booking == null) {
            CircularProgressIndicator()
            return@Column
        }

        Text(booking.serviceName, style = MaterialTheme.typography.headlineSmall)
        Text("${booking.bookingReference} · ${booking.status}")
        Text("${booking.craftsmanName} · ${booking.estimatedPrice} SAR")

        OutlinedButton(onClick = { onOpenChat(bookingId) }, modifier = Modifier.fillMaxWidth()) {
            Text(stringResource(R.string.chat_open))
        }

        val isCustomer = userRole == "Customer"
        val isCraftsman = userRole == "Craftsman"
        val cancellable = booking.status in setOf("Pending", "AwaitingPayment", "Confirmed", "Rescheduled")

        when {
            booking.status == "AwaitingPayment" && isCustomer -> {
                Button(onClick = { viewModel.pay(bookingId) }, modifier = Modifier.fillMaxWidth()) {
                    Text(stringResource(R.string.booking_pay))
                }
                state.paymentPendingMessage?.let { msg ->
                    Text(msg, style = MaterialTheme.typography.bodySmall)
                }
                OutlinedButton(onClick = { viewModel.loadBooking(bookingId) }, modifier = Modifier.fillMaxWidth()) {
                    Text("Refresh payment status")
                }
            }
            booking.status == "PendingCraftsmanConfirmation" && isCraftsman -> {
                Button(onClick = { viewModel.accept(bookingId) }, modifier = Modifier.fillMaxWidth()) {
                    Text(stringResource(R.string.booking_accept))
                }
                OutlinedButton(onClick = { showRejectDialog = true }, modifier = Modifier.fillMaxWidth()) {
                    Text(stringResource(R.string.booking_reject))
                }
            }
        }

        if (booking.status == "Confirmed" && isCraftsman) {
            Button(onClick = { viewModel.complete(bookingId) }, modifier = Modifier.fillMaxWidth()) {
                Text(stringResource(R.string.booking_complete))
            }
            OutlinedButton(onClick = { showNoShowDialog = true }, modifier = Modifier.fillMaxWidth()) {
                Text(stringResource(R.string.booking_no_show))
            }
        }

        if (cancellable && (isCustomer || isCraftsman)) {
            OutlinedButton(onClick = { showCancelDialog = true }, modifier = Modifier.fillMaxWidth()) {
                Text(stringResource(R.string.booking_cancel))
            }
        }

        if (booking.status == "Confirmed" && (isCustomer || isCraftsman)) {
            OutlinedButton(onClick = { showRescheduleDialog = true }, modifier = Modifier.fillMaxWidth()) {
                Text(stringResource(R.string.booking_reschedule))
            }
        }

        if (booking.status == "Completed" && isCustomer && booking.customerRating == null) {
            Button(onClick = { showReviewDialog = true }, modifier = Modifier.fillMaxWidth()) {
                Text(stringResource(R.string.review_leave))
            }
        }

        booking.customerRating?.let { rating ->
            Text(stringResource(R.string.review_your_review))
            Text("★ $rating")
            booking.customerReview?.let { Text(it) }
        }
    }

    if (showCancelDialog) {
        AlertDialog(
            onDismissRequest = { showCancelDialog = false },
            title = { Text(stringResource(R.string.booking_cancel)) },
            text = {
                OutlinedTextField(
                    value = reasonText,
                    onValueChange = { reasonText = it },
                    label = { Text(stringResource(R.string.booking_cancel_reason)) },
                    modifier = Modifier.fillMaxWidth(),
                )
            },
            confirmButton = {
                Button(
                    onClick = {
                        viewModel.cancel(bookingId, reasonText)
                        reasonText = ""
                        showCancelDialog = false
                    },
                    enabled = reasonText.isNotBlank(),
                ) { Text(stringResource(R.string.booking_confirm_cancel)) }
            },
            dismissButton = {
                OutlinedButton(onClick = { showCancelDialog = false }) { Text(stringResource(R.string.common_cancel)) }
            },
        )
    }

    if (showRejectDialog) {
        AlertDialog(
            onDismissRequest = { showRejectDialog = false },
            title = { Text(stringResource(R.string.booking_reject)) },
            text = {
                OutlinedTextField(
                    value = reasonText,
                    onValueChange = { reasonText = it },
                    label = { Text(stringResource(R.string.booking_cancel_reason)) },
                    modifier = Modifier.fillMaxWidth(),
                )
            },
            confirmButton = {
                Button(
                    onClick = {
                        viewModel.reject(bookingId, reasonText)
                        reasonText = ""
                        showRejectDialog = false
                    },
                    enabled = reasonText.isNotBlank(),
                ) { Text(stringResource(R.string.booking_reject)) }
            },
            dismissButton = {
                OutlinedButton(onClick = { showRejectDialog = false }) { Text(stringResource(R.string.common_cancel)) }
            },
        )
    }

    if (showNoShowDialog) {
        AlertDialog(
            onDismissRequest = { showNoShowDialog = false },
            title = { Text(stringResource(R.string.booking_no_show)) },
            text = { Text(stringResource(R.string.booking_no_show_hint)) },
            confirmButton = {
                Button(onClick = { viewModel.noShow(bookingId); showNoShowDialog = false }) {
                    Text(stringResource(R.string.booking_confirm_no_show))
                }
            },
            dismissButton = {
                OutlinedButton(onClick = { showNoShowDialog = false }) { Text(stringResource(R.string.common_cancel)) }
            },
        )
    }

    if (showRescheduleDialog && booking != null) {
        AlertDialog(
            onDismissRequest = { showRescheduleDialog = false },
            title = { Text(stringResource(R.string.booking_reschedule)) },
            text = {
                Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    OutlinedTextField(
                        value = rescheduleDate,
                        onValueChange = { rescheduleDate = it },
                        label = { Text(stringResource(R.string.booking_select_date)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    Button(onClick = {
                        viewModel.loadSlots(booking.craftsmanId, booking.serviceId, rescheduleDate)
                    }) { Text(stringResource(R.string.booking_load_slots)) }
                    state.slots.filter { it.isAvailable }.forEach { slot ->
                        OutlinedButton(
                            onClick = { selectedSlot = slot.start },
                            modifier = Modifier.fillMaxWidth(),
                        ) { Text(slot.start) }
                    }
                    OutlinedTextField(
                        value = reasonText,
                        onValueChange = { reasonText = it },
                        label = { Text(stringResource(R.string.booking_reschedule_reason)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                }
            },
            confirmButton = {
                Button(
                    onClick = {
                        selectedSlot?.let {
                            viewModel.reschedule(bookingId, it, reasonText.ifBlank { null })
                        }
                        reasonText = ""
                        selectedSlot = null
                        rescheduleDate = ""
                        showRescheduleDialog = false
                    },
                    enabled = selectedSlot != null,
                ) { Text(stringResource(R.string.booking_confirm_reschedule)) }
            },
            dismissButton = {
                OutlinedButton(onClick = { showRescheduleDialog = false }) { Text(stringResource(R.string.common_cancel)) }
            },
        )
    }

    if (showReviewDialog) {
        AlertDialog(
            onDismissRequest = { showReviewDialog = false },
            title = { Text(stringResource(R.string.review_title)) },
            text = {
                Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    OutlinedTextField(
                        value = reviewRating.toString(),
                        onValueChange = { reviewRating = it.toIntOrNull()?.coerceIn(1, 5) ?: reviewRating },
                        label = { Text(stringResource(R.string.review_rating)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    OutlinedTextField(
                        value = reviewComment,
                        onValueChange = { reviewComment = it },
                        label = { Text(stringResource(R.string.review_comment)) },
                        modifier = Modifier.fillMaxWidth(),
                        minLines = 3,
                    )
                }
            },
            confirmButton = {
                Button(onClick = {
                    viewModel.submitReview(bookingId, reviewRating, reviewComment.ifBlank { null })
                    reviewComment = ""
                    showReviewDialog = false
                }) { Text(stringResource(R.string.review_submit)) }
            },
            dismissButton = {
                OutlinedButton(onClick = { showReviewDialog = false }) { Text(stringResource(R.string.common_cancel)) }
            },
        )
    }
}
