package com.khadamati.app.ui.screens

import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.ExperimentalLayoutApi
import androidx.compose.foundation.layout.FlowRow
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.AssistChip
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.DatePicker
import androidx.compose.material3.DatePickerDialog
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.material3.TopAppBar
import androidx.compose.material3.rememberDatePickerState
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.khadamati.app.R
import com.khadamati.app.data.remote.dto.CraftsmanOptionDto
import com.khadamati.app.data.remote.dto.TimeSlotDto
import com.khadamati.app.domain.model.Service
import com.khadamati.app.ui.viewmodel.BookingViewModel
import com.khadamati.app.ui.viewmodel.ServicesViewModel
import java.text.SimpleDateFormat
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter
import java.util.Locale

private val stepLabels = listOf(
    R.string.booking_step_service,
    R.string.booking_step_craftsman,
    R.string.booking_step_datetime,
    R.string.booking_step_confirm,
)

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun BookingWizardScreen(
    servicesViewModel: ServicesViewModel,
    bookingViewModel: BookingViewModel,
    preselectedServiceId: String?,
    onNavigateBack: () -> Unit,
    onBookingCreated: (String) -> Unit,
) {
    val servicesState by servicesViewModel.uiState.collectAsStateWithLifecycle()
    val bookingState by bookingViewModel.uiState.collectAsStateWithLifecycle()
    val isArabic = LocalConfiguration.current.locales[0].language == "ar"

    var activeStep by remember { mutableIntStateOf(if (preselectedServiceId != null) 1 else 0) }
    var selectedService by remember { mutableStateOf<Service?>(null) }
    var selectedCraftsman by remember { mutableStateOf<CraftsmanOptionDto?>(null) }
    var selectedDate by remember { mutableStateOf("") }
    var selectedSlot by remember { mutableStateOf<TimeSlotDto?>(null) }
    var showDatePicker by remember { mutableStateOf(false) }

    LaunchedEffect(preselectedServiceId, servicesState.services) {
        if (preselectedServiceId != null && selectedService == null) {
            servicesState.services.find { it.id == preselectedServiceId }?.let { service ->
                selectedService = service
                bookingViewModel.loadCraftsmen(service.id)
            }
        }
    }

    if (showDatePicker) {
        val datePickerState = rememberDatePickerState()
        DatePickerDialog(
            onDismissRequest = { showDatePicker = false },
            confirmButton = {
                TextButton(
                    onClick = {
                        datePickerState.selectedDateMillis?.let { millis ->
                            selectedDate = Instant.ofEpochMilli(millis)
                                .atZone(ZoneId.systemDefault())
                                .format(DateTimeFormatter.ISO_LOCAL_DATE)
                            selectedSlot = null
                        }
                        showDatePicker = false
                    },
                ) {
                    Text(stringResource(R.string.common_save))
                }
            },
            dismissButton = {
                TextButton(onClick = { showDatePicker = false }) {
                    Text(stringResource(R.string.common_cancel))
                }
            },
        ) {
            DatePicker(state = datePickerState)
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.booking_title)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(horizontal = 16.dp),
        ) {
            StepIndicator(activeStep = activeStep)

            bookingState.error?.let { error ->
                Spacer(modifier = Modifier.height(8.dp))
                Text(error, color = MaterialTheme.colorScheme.error)
            }

            Spacer(modifier = Modifier.height(16.dp))

            when (activeStep) {
                0 -> ServiceStep(
                    services = servicesState.services,
                    isArabic = isArabic,
                    isLoading = servicesState.isLoading && servicesState.services.isEmpty(),
                    onSelect = { service ->
                        selectedService = service
                        selectedCraftsman = null
                        selectedDate = ""
                        selectedSlot = null
                        activeStep = 1
                        bookingViewModel.loadCraftsmen(service.id)
                    },
                )
                1 -> CraftsmanStep(
                    craftsmen = bookingState.craftsmen,
                    isLoading = bookingState.isLoading,
                    isArabic = isArabic,
                    onSelect = { craftsman ->
                        selectedCraftsman = craftsman
                        selectedDate = ""
                        selectedSlot = null
                        activeStep = 2
                    },
                    onBack = { activeStep = 0 },
                )
                2 -> DateTimeStep(
                    selectedDate = selectedDate,
                    slots = bookingState.slots.filter { it.isAvailable },
                    selectedSlot = selectedSlot,
                    isLoading = bookingState.isLoading,
                    onPickDate = { showDatePicker = true },
                    onLoadSlots = {
                        val service = selectedService ?: return@DateTimeStep
                        val craftsman = selectedCraftsman ?: return@DateTimeStep
                        if (selectedDate.isBlank()) return@DateTimeStep
                        bookingViewModel.loadSlots(craftsman.id, service.id, selectedDate)
                    },
                    onSelectSlot = { selectedSlot = it },
                    onBack = { activeStep = 1 },
                    onNext = { if (selectedSlot != null) activeStep = 3 },
                )
                3 -> ConfirmStep(
                    service = selectedService,
                    craftsman = selectedCraftsman,
                    slot = selectedSlot,
                    isArabic = isArabic,
                    isSubmitting = bookingState.isLoading,
                    onBack = { activeStep = 2 },
                    onConfirm = {
                        val service = selectedService ?: return@ConfirmStep
                        val craftsman = selectedCraftsman ?: return@ConfirmStep
                        val slot = selectedSlot ?: return@ConfirmStep
                        bookingViewModel.createBooking(
                            serviceId = service.id,
                            craftsmanId = craftsman.id,
                            scheduledAt = slot.start,
                        ) { bookingId ->
                            onBookingCreated(bookingId)
                        }
                    },
                )
            }
        }
    }
}

@Composable
private fun StepIndicator(activeStep: Int) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.spacedBy(4.dp),
    ) {
        stepLabels.forEachIndexed { index, labelRes ->
            val isActive = index == activeStep
            val isComplete = index < activeStep
            AssistChip(
                onClick = {},
                enabled = false,
                label = {
                    Text(
                        text = stringResource(labelRes),
                        style = MaterialTheme.typography.labelSmall,
                        fontWeight = if (isActive) FontWeight.Bold else FontWeight.Normal,
                    )
                },
                modifier = Modifier
                    .weight(1f)
                    .then(
                        if (isActive || isComplete) {
                            Modifier.border(1.dp, MaterialTheme.colorScheme.primary, MaterialTheme.shapes.small)
                        } else {
                            Modifier
                        },
                    ),
            )
        }
    }
}

@Composable
private fun ServiceStep(
    services: List<Service>,
    isArabic: Boolean,
    isLoading: Boolean,
    onSelect: (Service) -> Unit,
) {
    if (isLoading) {
        Column(Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
            CircularProgressIndicator()
        }
        return
    }

    LazyColumn(
        verticalArrangement = Arrangement.spacedBy(8.dp),
        contentPadding = PaddingValues(bottom = 16.dp),
    ) {
        items(services, key = { it.id }) { service ->
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .clickable { onSelect(service) },
            ) {
                Column(Modifier.padding(16.dp)) {
                    Text(service.localizedName(isArabic), style = MaterialTheme.typography.titleMedium)
                    Text(
                        "${String.format(Locale.getDefault(), "%.2f", service.basePrice)} ${stringResource(R.string.common_currency)}",
                        color = MaterialTheme.colorScheme.primary,
                    )
                }
            }
        }
    }
}

@Composable
private fun CraftsmanStep(
    craftsmen: List<CraftsmanOptionDto>,
    isLoading: Boolean,
    isArabic: Boolean,
    onSelect: (CraftsmanOptionDto) -> Unit,
    onBack: () -> Unit,
) {
    if (isLoading) {
        CircularProgressIndicator()
        return
    }

    if (craftsmen.isEmpty()) {
        Text(stringResource(R.string.booking_no_craftsmen))
    } else {
        LazyColumn(
            verticalArrangement = Arrangement.spacedBy(8.dp),
            contentPadding = PaddingValues(bottom = 16.dp),
        ) {
            items(craftsmen, key = { it.id }) { craftsman ->
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .clickable { onSelect(craftsman) },
                ) {
                    Row(
                        modifier = Modifier.padding(16.dp),
                        horizontalArrangement = Arrangement.SpaceBetween,
                    ) {
                        Column {
                            Text(
                                "${craftsman.firstName} ${craftsman.lastName}",
                                style = MaterialTheme.typography.titleMedium,
                            )
                            craftsman.specialization?.let {
                                Text(it, style = MaterialTheme.typography.bodySmall)
                            }
                        }
                        Column(horizontalAlignment = Alignment.End) {
                            Text("⭐ ${String.format(Locale.getDefault(), "%.1f", craftsman.rating)}")
                            Text(
                                "${String.format(Locale.getDefault(), "%.2f", craftsman.price)} ${stringResource(R.string.common_currency)}",
                                color = MaterialTheme.colorScheme.primary,
                            )
                        }
                    }
                }
            }
        }
    }

    OutlinedButton(onClick = onBack, modifier = Modifier.padding(top = 8.dp)) {
        Text(stringResource(R.string.common_back))
    }
}

@OptIn(ExperimentalLayoutApi::class)
@Composable
private fun DateTimeStep(
    selectedDate: String,
    slots: List<TimeSlotDto>,
    selectedSlot: TimeSlotDto?,
    isLoading: Boolean,
    onPickDate: () -> Unit,
    onLoadSlots: () -> Unit,
    onSelectSlot: (TimeSlotDto) -> Unit,
    onBack: () -> Unit,
    onNext: () -> Unit,
) {
    Column(verticalArrangement = Arrangement.spacedBy(12.dp)) {
        OutlinedButton(onClick = onPickDate, modifier = Modifier.fillMaxWidth()) {
            Text(
                if (selectedDate.isBlank()) {
                    stringResource(R.string.booking_select_date)
                } else {
                    selectedDate
                },
            )
        }

        Button(
            onClick = onLoadSlots,
            enabled = selectedDate.isNotBlank() && !isLoading,
            modifier = Modifier.fillMaxWidth(),
        ) {
            Text(stringResource(R.string.booking_load_slots))
        }

        if (isLoading) {
            CircularProgressIndicator()
        } else if (slots.isEmpty() && selectedDate.isNotBlank()) {
            Text(stringResource(R.string.booking_no_slots))
        } else {
            FlowRow(
                horizontalArrangement = Arrangement.spacedBy(8.dp),
                verticalArrangement = Arrangement.spacedBy(8.dp),
            ) {
                slots.forEach { slot ->
                    val isSelected = selectedSlot?.start == slot.start
                    val label = formatSlotTime(slot.start)
                    if (isSelected) {
                        Button(onClick = { onSelectSlot(slot) }) { Text(label) }
                    } else {
                        OutlinedButton(onClick = { onSelectSlot(slot) }) { Text(label) }
                    }
                }
            }
        }

        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
            OutlinedButton(onClick = onBack) { Text(stringResource(R.string.common_back)) }
            Button(onClick = onNext, enabled = selectedSlot != null) {
                Text(stringResource(R.string.common_next))
            }
        }
    }
}

@Composable
private fun ConfirmStep(
    service: Service?,
    craftsman: CraftsmanOptionDto?,
    slot: TimeSlotDto?,
    isArabic: Boolean,
    isSubmitting: Boolean,
    onBack: () -> Unit,
    onConfirm: () -> Unit,
) {
    Card(
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant),
    ) {
        Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
            Text(stringResource(R.string.booking_summary), style = MaterialTheme.typography.titleMedium)
            service?.let { Text(it.localizedName(isArabic)) }
            craftsman?.let { Text("${it.firstName} ${it.lastName}") }
            slot?.let { Text(formatSlotDateTime(it.start)) }
            craftsman?.let {
                Text(
                    "${String.format(Locale.getDefault(), "%.2f", it.price)} ${stringResource(R.string.common_currency)}",
                    fontWeight = FontWeight.Bold,
                )
            }
        }
    }

    Spacer(modifier = Modifier.height(8.dp))

    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
        OutlinedButton(onClick = onBack, enabled = !isSubmitting) {
            Text(stringResource(R.string.common_back))
        }
        Button(onClick = onConfirm, enabled = !isSubmitting) {
            if (isSubmitting) {
                CircularProgressIndicator()
            } else {
                Text(stringResource(R.string.booking_confirm_and_pay))
            }
        }
    }
}

private fun formatSlotTime(iso: String): String {
    return try {
        val parser = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.US)
        val formatter = SimpleDateFormat("HH:mm", Locale.getDefault())
        parser.parse(iso)?.let { formatter.format(it) } ?: iso
    } catch (_: Exception) {
        iso
    }
}

private fun formatSlotDateTime(iso: String): String {
    return try {
        val parser = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.US)
        val formatter = SimpleDateFormat("yyyy-MM-dd HH:mm", Locale.getDefault())
        parser.parse(iso)?.let { formatter.format(it) } ?: iso
    } catch (_: Exception) {
        iso
    }
}
