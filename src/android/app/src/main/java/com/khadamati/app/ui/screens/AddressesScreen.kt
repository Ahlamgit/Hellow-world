package com.khadamati.app.ui.screens

import android.Manifest
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
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
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBar
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.khadamati.app.R
import com.khadamati.app.location.GeoLocation
import com.khadamati.app.location.LocationProvider
import com.khadamati.app.ui.viewmodel.AddressViewModel
import kotlinx.coroutines.launch

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun AddressesScreen(
    viewModel: AddressViewModel,
    locationProvider: LocationProvider,
    onNavigateBack: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    val scope = rememberCoroutineScope()
    var label by remember { mutableStateOf("Home") }
    var street by remember { mutableStateOf("") }
    var city by remember { mutableStateOf("") }
    var country by remember { mutableStateOf("LB") }
    var location by remember { mutableStateOf<GeoLocation?>(null) }

    val permissionLauncher = rememberLauncherForActivityResult(
        ActivityResultContracts.RequestMultiplePermissions(),
    ) { grants ->
        if (grants.values.any { it }) {
            scope.launch { location = locationProvider.getCurrentLocation() }
        }
    }

    LaunchedEffect(Unit) { viewModel.load() }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.addresses_title)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        LazyColumn(
            modifier = Modifier.fillMaxSize().padding(padding),
            contentPadding = PaddingValues(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp),
        ) {
            item {
                Text(stringResource(R.string.addresses_add), style = MaterialTheme.typography.titleMedium)
                OutlinedTextField(value = label, onValueChange = { label = it }, label = { Text(stringResource(R.string.addresses_label)) }, modifier = Modifier.fillMaxWidth())
                OutlinedTextField(value = street, onValueChange = { street = it }, label = { Text(stringResource(R.string.addresses_street)) }, modifier = Modifier.fillMaxWidth())
                OutlinedTextField(value = city, onValueChange = { city = it }, label = { Text(stringResource(R.string.addresses_city)) }, modifier = Modifier.fillMaxWidth())
                OutlinedTextField(value = country, onValueChange = { country = it }, label = { Text(stringResource(R.string.addresses_country)) }, modifier = Modifier.fillMaxWidth())
                Button(
                    onClick = {
                        permissionLauncher.launch(
                            arrayOf(
                                Manifest.permission.ACCESS_FINE_LOCATION,
                                Manifest.permission.ACCESS_COARSE_LOCATION,
                            ),
                        )
                    },
                    modifier = Modifier.fillMaxWidth(),
                ) { Text(stringResource(R.string.addresses_use_location)) }
                location?.let {
                    Text(
                        stringResource(R.string.addresses_coordinates, it.latitude, it.longitude),
                        style = MaterialTheme.typography.bodySmall,
                    )
                }
                Button(
                    onClick = {
                        viewModel.addAddress(label, street, city, country, location?.latitude, location?.longitude) {
                            street = ""
                            city = ""
                            location = null
                        }
                    },
                    enabled = !state.isSaving && street.isNotBlank() && city.isNotBlank(),
                    modifier = Modifier.fillMaxWidth(),
                ) { Text(stringResource(R.string.common_save)) }
            }
            if (state.isLoading && state.addresses.isEmpty()) {
                item { Box(Modifier.fillMaxWidth(), contentAlignment = Alignment.Center) { CircularProgressIndicator() } }
            }
            items(state.addresses, key = { it.id }) { address ->
                Card(Modifier.fillMaxWidth()) {
                    Column(Modifier.padding(16.dp)) {
                        Text(address.label, style = MaterialTheme.typography.titleMedium)
                        Text("${address.street}, ${address.city}")
                        if (address.latitude != null && address.longitude != null) {
                            Text(
                                stringResource(R.string.addresses_coordinates, address.latitude!!, address.longitude!!),
                                style = MaterialTheme.typography.bodySmall,
                            )
                        }
                    }
                }
            }
        }
    }
}
