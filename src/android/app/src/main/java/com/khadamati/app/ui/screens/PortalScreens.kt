package com.khadamati.app.ui.screens

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Switch
import androidx.compose.material3.Text
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
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.khadamati.app.R
import com.khadamati.app.ui.viewmodel.CraftsmanPortalViewModel
import com.khadamati.app.ui.viewmodel.StorePortalViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CraftsmanPortalScreen(
    viewModel: CraftsmanPortalViewModel,
    isArabic: Boolean,
    onNavigateBack: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    var specialization by remember { mutableStateOf("") }
    var years by remember { mutableStateOf("0") }
    var radius by remember { mutableStateOf("25") }
    var isAvailable by remember { mutableStateOf(true) }
    var selectedServiceId by remember { mutableStateOf("") }
    var customPrice by remember { mutableStateOf("0") }

    LaunchedEffect(Unit) { viewModel.load() }

    LaunchedEffect(state.profile) {
        state.profile?.let { profile ->
            specialization = profile.specialization.orEmpty()
            years = profile.yearsOfExperience.toString()
            radius = (profile.serviceRadiusKm ?: 25.0).toString()
            isAvailable = profile.isAvailable
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.craftsman_portal_title)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        if (state.isLoading && state.profile == null) {
            Column(
                modifier = Modifier.fillMaxSize().padding(padding),
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
            ) {
                CircularProgressIndicator()
            }
            return@Scaffold
        }

        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .verticalScroll(rememberScrollState())
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp),
        ) {
            state.errorMessage?.let { Text(it, color = MaterialTheme.colorScheme.error) }
            if (state.message == "saved") {
                Text(stringResource(R.string.portal_saved), color = MaterialTheme.colorScheme.primary)
            }
            if (state.message == "service_added") {
                Text(stringResource(R.string.portal_service_added), color = MaterialTheme.colorScheme.primary)
            }

            Card(Modifier.fillMaxWidth()) {
                Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text(stringResource(R.string.portal_profile), style = MaterialTheme.typography.titleMedium)
                    state.profile?.let { profile ->
                        Text("⭐ ${profile.rating} · ${profile.completedJobs} ${stringResource(R.string.portal_jobs)}")
                    }
                    OutlinedTextField(
                        value = specialization,
                        onValueChange = { specialization = it },
                        label = { Text(stringResource(R.string.craftsman_specialization)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    OutlinedTextField(
                        value = years,
                        onValueChange = { years = it },
                        label = { Text(stringResource(R.string.craftsman_years)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    OutlinedTextField(
                        value = radius,
                        onValueChange = { radius = it },
                        label = { Text(stringResource(R.string.craftsman_radius)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Switch(checked = isAvailable, onCheckedChange = { isAvailable = it })
                        Text(stringResource(R.string.craftsman_available))
                    }
                    Button(
                        onClick = {
                            viewModel.saveProfile(
                                specialization = specialization,
                                yearsOfExperience = years.toIntOrNull() ?: 0,
                                isAvailable = isAvailable,
                                serviceRadiusKm = radius.toDoubleOrNull() ?: 25.0,
                            )
                        },
                        enabled = !state.isLoading,
                    ) {
                        Text(stringResource(R.string.common_save))
                    }
                }
            }

            Card(Modifier.fillMaxWidth()) {
                Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text(stringResource(R.string.craftsman_offered_services), style = MaterialTheme.typography.titleMedium)
                    state.profile?.services?.forEach { service ->
                        Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                            Text(if (isArabic) service.serviceNameAr else service.serviceNameEn)
                            Text("${service.customPrice} ${stringResource(R.string.common_currency)}")
                        }
                    }
                    Text(stringResource(R.string.portal_pick_service), style = MaterialTheme.typography.labelMedium)
                    state.services.forEach { service ->
                        OutlinedButton(
                            onClick = { selectedServiceId = service.id },
                            modifier = Modifier.fillMaxWidth(),
                        ) {
                            Text(if (isArabic) service.nameAr else service.nameEn)
                        }
                    }
                    OutlinedTextField(
                        value = customPrice,
                        onValueChange = { customPrice = it },
                        label = { Text(stringResource(R.string.portal_price)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    Button(
                        onClick = {
                            viewModel.addService(
                                serviceId = selectedServiceId,
                                customPrice = customPrice.toDoubleOrNull() ?: 0.0,
                            )
                        },
                        enabled = !state.isLoading && selectedServiceId.isNotBlank(),
                    ) {
                        Text(stringResource(R.string.portal_add_service))
                    }
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun StorePortalScreen(
    viewModel: StorePortalViewModel,
    isArabic: Boolean,
    onNavigateBack: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    var storeName by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var isOpen by remember { mutableStateOf(true) }
    var openingTime by remember { mutableStateOf("08:00") }
    var closingTime by remember { mutableStateOf("22:00") }
    var productNameEn by remember { mutableStateOf("") }
    var productNameAr by remember { mutableStateOf("") }
    var productPrice by remember { mutableStateOf("0") }
    var productStock by remember { mutableStateOf("0") }

    LaunchedEffect(Unit) { viewModel.load() }

    LaunchedEffect(state.profile) {
        state.profile?.let { profile ->
            storeName = profile.storeName
            description = profile.description.orEmpty()
            isOpen = profile.isOpen
            openingTime = profile.openingTime ?: "08:00"
            closingTime = profile.closingTime ?: "22:00"
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.store_portal_title)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        if (state.isLoading && state.profile == null) {
            Column(
                modifier = Modifier.fillMaxSize().padding(padding),
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
            ) {
                CircularProgressIndicator()
            }
            return@Scaffold
        }

        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .verticalScroll(rememberScrollState())
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp),
        ) {
            state.errorMessage?.let { Text(it, color = MaterialTheme.colorScheme.error) }
            if (state.message == "saved") {
                Text(stringResource(R.string.portal_saved), color = MaterialTheme.colorScheme.primary)
            }
            if (state.message == "product_added") {
                Text(stringResource(R.string.portal_product_added), color = MaterialTheme.colorScheme.primary)
            }

            Card(Modifier.fillMaxWidth()) {
                Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text(stringResource(R.string.store_profile), style = MaterialTheme.typography.titleMedium)
                    OutlinedTextField(
                        value = storeName,
                        onValueChange = { storeName = it },
                        label = { Text(stringResource(R.string.store_name)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    OutlinedTextField(
                        value = description,
                        onValueChange = { description = it },
                        label = { Text(stringResource(R.string.store_description)) },
                        modifier = Modifier.fillMaxWidth(),
                        minLines = 3,
                    )
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        OutlinedTextField(
                            value = openingTime,
                            onValueChange = { openingTime = it },
                            label = { Text(stringResource(R.string.store_opens)) },
                            modifier = Modifier.weight(1f),
                        )
                        OutlinedTextField(
                            value = closingTime,
                            onValueChange = { closingTime = it },
                            label = { Text(stringResource(R.string.store_closes)) },
                            modifier = Modifier.weight(1f),
                        )
                    }
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Switch(checked = isOpen, onCheckedChange = { isOpen = it })
                        Text(stringResource(R.string.store_open))
                    }
                    Button(
                        onClick = {
                            viewModel.saveProfile(
                                storeName = storeName,
                                description = description,
                                isOpen = isOpen,
                                openingTime = openingTime,
                                closingTime = closingTime,
                            )
                        },
                        enabled = !state.isLoading && storeName.isNotBlank(),
                    ) {
                        Text(stringResource(R.string.common_save))
                    }
                }
            }

            Card(Modifier.fillMaxWidth()) {
                Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text(stringResource(R.string.store_products), style = MaterialTheme.typography.titleMedium)
                    state.profile?.products?.forEach { product ->
                        Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                            Text(if (isArabic) product.nameAr else product.nameEn)
                            Text("${product.price} ${stringResource(R.string.common_currency)} · ${product.stockQuantity}")
                        }
                    }
                    OutlinedTextField(
                        value = productNameEn,
                        onValueChange = { productNameEn = it },
                        label = { Text(stringResource(R.string.store_product_name_en)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    OutlinedTextField(
                        value = productNameAr,
                        onValueChange = { productNameAr = it },
                        label = { Text(stringResource(R.string.store_product_name_ar)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    OutlinedTextField(
                        value = productPrice,
                        onValueChange = { productPrice = it },
                        label = { Text(stringResource(R.string.portal_price)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    OutlinedTextField(
                        value = productStock,
                        onValueChange = { productStock = it },
                        label = { Text(stringResource(R.string.store_stock)) },
                        modifier = Modifier.fillMaxWidth(),
                    )
                    Button(
                        onClick = {
                            viewModel.addProduct(
                                nameEn = productNameEn,
                                nameAr = productNameAr,
                                price = productPrice.toDoubleOrNull() ?: 0.0,
                                stockQuantity = productStock.toIntOrNull() ?: 0,
                            )
                            productNameEn = ""
                            productNameAr = ""
                            productPrice = "0"
                            productStock = "0"
                        },
                        enabled = !state.isLoading && productNameEn.isNotBlank(),
                    ) {
                        Text(stringResource(R.string.portal_add_product))
                    }
                }
            }
            Spacer(modifier = Modifier.height(16.dp))
        }
    }
}
