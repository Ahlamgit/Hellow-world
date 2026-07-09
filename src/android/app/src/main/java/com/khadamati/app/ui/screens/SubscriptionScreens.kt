package com.khadamati.app.ui.screens

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
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
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.FilterChip
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Switch
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.material3.TopAppBar
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.khadamati.app.R
import com.khadamati.app.data.remote.dto.PlanBillingOptionDto
import com.khadamati.app.data.remote.dto.SubscriptionPlanDto
import com.khadamati.app.data.remote.dto.UserSubscriptionDto
import com.khadamati.app.ui.viewmodel.SubscriptionViewModel
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter
import java.util.Locale

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SubscriptionPlansScreen(
    viewModel: SubscriptionViewModel,
    targetRole: String?,
    isArabic: Boolean,
    onNavigateBack: () -> Unit,
    onNavigateToSubscribe: (String) -> Unit,
    onNavigateToMySubscription: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()

    LaunchedEffect(targetRole) { viewModel.loadPlans(targetRole) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.subscription_plans_title)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        when {
            state.isLoading -> {
                Box(Modifier.fillMaxSize().padding(padding), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
            }
            else -> {
                LazyColumn(
                    modifier = Modifier.fillMaxSize().padding(padding),
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp),
                ) {
                    state.current?.let { current ->
                        item {
                            Card(Modifier.fillMaxWidth()) {
                                Row(
                                    Modifier.padding(16.dp).fillMaxWidth(),
                                    horizontalArrangement = Arrangement.SpaceBetween,
                                    verticalAlignment = Alignment.CenterVertically,
                                ) {
                                    Text(
                                        text = stringResource(
                                            R.string.subscription_active_plan,
                                            if (isArabic) current.planNameAr else current.planNameEn,
                                            current.status,
                                        ),
                                        style = MaterialTheme.typography.bodyMedium,
                                        modifier = Modifier.weight(1f),
                                    )
                                    TextButton(onClick = onNavigateToMySubscription) {
                                        Text(stringResource(R.string.subscription_manage))
                                    }
                                }
                            }
                        }
                    }

                    if (state.plans.isEmpty()) {
                        item {
                            Text(
                                stringResource(R.string.subscription_no_plans),
                                color = MaterialTheme.colorScheme.onSurfaceVariant,
                            )
                        }
                    }

                    items(state.plans, key = { it.id }) { plan ->
                        PlanCard(
                            plan = plan,
                            isArabic = isArabic,
                            hasActiveSubscription = state.current != null,
                            onChoose = { onNavigateToSubscribe(plan.id) },
                        )
                    }
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SubscribeScreen(
    planId: String,
    viewModel: SubscriptionViewModel,
    isArabic: Boolean,
    onNavigateBack: () -> Unit,
    onSubscribed: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()

    LaunchedEffect(planId) { viewModel.loadPlan(planId) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.subscription_subscribe_title)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        val plan = state.selectedPlan
        when {
            state.isLoading || plan == null -> {
                Box(Modifier.fillMaxSize().padding(padding), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
            }
            else -> {
                Column(
                    Modifier.fillMaxSize().padding(padding).padding(16.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp),
                ) {
                    Text(
                        text = if (isArabic) plan.nameAr else plan.nameEn,
                        style = MaterialTheme.typography.headlineSmall,
                        fontWeight = FontWeight.Bold,
                    )
                    val description = if (isArabic) plan.descriptionAr else plan.descriptionEn
                    if (!description.isNullOrBlank()) {
                        Text(description, color = MaterialTheme.colorScheme.onSurfaceVariant)
                    }

                    Text(
                        stringResource(R.string.subscription_billing_cycle),
                        style = MaterialTheme.typography.titleSmall,
                    )
                    val activeOptions = plan.billingOptions.filter { it.isActive && it.id != null }
                    activeOptions.forEach { option ->
                        FilterChip(
                            selected = state.selectedBillingOptionId == option.id,
                            onClick = { option.id?.let(viewModel::selectBillingOption) },
                            label = {
                                Text(
                                    "${billingCycleLabel(option.cycle)} — ${option.price} ${plan.currency}",
                                )
                            },
                        )
                    }

                    Row(
                        Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically,
                    ) {
                        Text(stringResource(R.string.subscription_auto_renew))
                        Switch(
                            checked = state.autoRenew,
                            onCheckedChange = viewModel::setAutoRenew,
                        )
                    }

                    Spacer(Modifier.height(8.dp))
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        OutlinedButton(onClick = onNavigateBack) {
                            Text(stringResource(R.string.common_back))
                        }
                        Button(
                            onClick = { viewModel.subscribe(onSubscribed) },
                            enabled = !state.isSubmitting && state.selectedBillingOptionId != null,
                        ) {
                            if (state.isSubmitting) {
                                CircularProgressIndicator()
                            } else {
                                Text(stringResource(R.string.subscription_confirm_subscribe))
                            }
                        }
                    }
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MySubscriptionScreen(
    viewModel: SubscriptionViewModel,
    isArabic: Boolean,
    onNavigateBack: () -> Unit,
    onBrowsePlans: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    var showCancelDialog by remember { mutableStateOf(false) }
    var cancelReason by remember { mutableStateOf("") }

    LaunchedEffect(Unit) { viewModel.loadMySubscription() }

    if (showCancelDialog) {
        AlertDialog(
            onDismissRequest = { showCancelDialog = false },
            title = { Text(stringResource(R.string.subscription_cancel_title)) },
            text = {
                Column {
                    Text(stringResource(R.string.subscription_cancel_hint))
                    Spacer(Modifier.height(8.dp))
                    OutlinedTextField(
                        value = cancelReason,
                        onValueChange = { cancelReason = it },
                        label = { Text(stringResource(R.string.subscription_cancel_reason)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                }
            },
            confirmButton = {
                TextButton(
                    onClick = {
                        viewModel.cancel(cancelReason.ifBlank { null }) {
                            showCancelDialog = false
                            cancelReason = ""
                        }
                    },
                    enabled = !state.isSubmitting,
                ) {
                    Text(stringResource(R.string.subscription_confirm_cancel))
                }
            },
            dismissButton = {
                TextButton(onClick = { showCancelDialog = false }) {
                    Text(stringResource(R.string.common_cancel))
                }
            },
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.subscription_my_subscription)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        when {
            state.isLoading -> {
                Box(Modifier.fillMaxSize().padding(padding), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
            }
            state.current == null -> {
                Column(
                    Modifier.fillMaxSize().padding(padding).padding(16.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp),
                ) {
                    Text(stringResource(R.string.subscription_no_active))
                    Button(onClick = onBrowsePlans) {
                        Text(stringResource(R.string.subscription_browse_plans))
                    }
                }
            }
            else -> {
                LazyColumn(
                    modifier = Modifier.fillMaxSize().padding(padding),
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp),
                ) {
                    item {
                        CurrentSubscriptionCard(
                            subscription = state.current!!,
                            isArabic = isArabic,
                            isSubmitting = state.isSubmitting,
                            onAutoRenewChange = viewModel::updateAutoRenew,
                            onCancel = { showCancelDialog = true },
                        )
                    }
                    if (state.history.isNotEmpty()) {
                        item {
                            Text(
                                stringResource(R.string.subscription_history),
                                style = MaterialTheme.typography.titleMedium,
                                fontWeight = FontWeight.SemiBold,
                            )
                        }
                        items(state.history, key = { it.id }) { item ->
                            HistoryCard(item, isArabic)
                        }
                    }
                }
            }
        }
    }
}

@Composable
private fun PlanCard(
    plan: SubscriptionPlanDto,
    isArabic: Boolean,
    hasActiveSubscription: Boolean,
    onChoose: () -> Unit,
) {
    Card(Modifier.fillMaxWidth()) {
        Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
            Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text(
                    if (isArabic) plan.nameAr else plan.nameEn,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.SemiBold,
                )
                if (plan.isFeatured) {
                    Text(
                        stringResource(R.string.subscription_featured),
                        style = MaterialTheme.typography.labelSmall,
                        color = MaterialTheme.colorScheme.primary,
                    )
                }
            }
            val description = if (isArabic) plan.descriptionAr else plan.descriptionEn
            if (!description.isNullOrBlank()) {
                Text(description, style = MaterialTheme.typography.bodySmall)
            }
            plan.billingOptions.filter { it.isActive }.forEach { option ->
                Text(
                    "${billingCycleLabel(option.cycle)}: ${option.price} ${plan.currency}",
                    style = MaterialTheme.typography.bodySmall,
                )
            }
            plan.maxServices?.let {
                Text(
                    stringResource(R.string.subscription_max_services, it),
                    style = MaterialTheme.typography.labelSmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                )
            }
            Button(
                onClick = onChoose,
                enabled = !hasActiveSubscription,
                modifier = Modifier.fillMaxWidth(),
            ) {
                Text(
                    if (hasActiveSubscription) {
                        stringResource(R.string.subscription_already_subscribed)
                    } else {
                        stringResource(R.string.subscription_choose_plan)
                    },
                )
            }
        }
    }
}

@Composable
private fun CurrentSubscriptionCard(
    subscription: UserSubscriptionDto,
    isArabic: Boolean,
    isSubmitting: Boolean,
    onAutoRenewChange: (Boolean) -> Unit,
    onCancel: () -> Unit,
) {
    Card(Modifier.fillMaxWidth()) {
        Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
            Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text(
                    if (isArabic) subscription.planNameAr else subscription.planNameEn,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                )
                subscription.amountPaid?.let { amount ->
                    Text(
                        "$amount ${subscription.currency.orEmpty()}",
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold,
                    )
                }
            }
            Text(subscription.status, color = MaterialTheme.colorScheme.primary)
            Text("${stringResource(R.string.subscription_start_date)}: ${formatDate(subscription.startDate)}")
            subscription.endDate?.let {
                Text("${stringResource(R.string.subscription_end_date)}: ${formatDate(it)}")
            }
            subscription.billingCycle?.let {
                Text("${stringResource(R.string.subscription_billing_cycle)}: $it")
            }
            Row(
                Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically,
            ) {
                Text(stringResource(R.string.subscription_auto_renew))
                Switch(
                    checked = subscription.autoRenew,
                    onCheckedChange = onAutoRenewChange,
                    enabled = !isSubmitting,
                )
            }
            OutlinedButton(onClick = onCancel, enabled = !isSubmitting) {
                Text(stringResource(R.string.subscription_cancel))
            }
        }
    }
}

@Composable
private fun HistoryCard(subscription: UserSubscriptionDto, isArabic: Boolean) {
    Card(Modifier.fillMaxWidth()) {
        Row(
            Modifier.padding(16.dp).fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
        ) {
            Column {
                Text(
                    if (isArabic) subscription.planNameAr else subscription.planNameEn,
                    fontWeight = FontWeight.SemiBold,
                )
                Text(
                    "${formatDate(subscription.startDate)} — ${subscription.status}",
                    style = MaterialTheme.typography.labelSmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                )
            }
            subscription.amountPaid?.let {
                Text("$it ${subscription.currency.orEmpty()}")
            }
        }
    }
}

@Composable
private fun billingCycleLabel(cycle: String): String = when (cycle) {
    "Monthly" -> stringResource(R.string.subscription_cycle_monthly)
    "Quarterly" -> stringResource(R.string.subscription_cycle_quarterly)
    "SemiAnnual" -> stringResource(R.string.subscription_cycle_semi_annual)
    "Annual" -> stringResource(R.string.subscription_cycle_annual)
    "Lifetime" -> stringResource(R.string.subscription_cycle_lifetime)
    else -> cycle
}

private fun formatDate(iso: String): String = runCatching {
    val instant = Instant.parse(iso)
    DateTimeFormatter.ofPattern("MMM d, yyyy", Locale.getDefault())
        .withZone(ZoneId.systemDefault())
        .format(instant)
}.getOrDefault(iso)
