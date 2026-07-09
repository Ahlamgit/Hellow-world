package com.khadamati.app.ui.screens

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
import androidx.compose.material3.Tab
import androidx.compose.material3.TabRow
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBar
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.khadamati.app.R
import com.khadamati.app.ui.viewmodel.SupportViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SupportScreen(
    viewModel: SupportViewModel,
    onNavigateBack: () -> Unit,
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    var tab by remember { mutableIntStateOf(0) }
    var submitTab by remember { mutableIntStateOf(0) }
    var subject by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var category by remember { mutableStateOf("General") }

    LaunchedEffect(Unit) { viewModel.load() }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.support_title)) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = null)
                    }
                },
            )
        },
    ) { padding ->
        Column(Modifier.fillMaxSize().padding(padding)) {
            TabRow(selectedTabIndex = tab) {
                Tab(selected = tab == 0, onClick = { tab = 0 }, text = { Text(stringResource(R.string.support_submit)) })
                Tab(selected = tab == 1, onClick = { tab = 1 }, text = { Text(stringResource(R.string.support_history)) })
            }

            when (tab) {
                0 -> Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(12.dp)) {
                    TabRow(selectedTabIndex = submitTab) {
                        Tab(selected = submitTab == 0, onClick = { submitTab = 0 }, text = { Text(stringResource(R.string.support_complaint)) })
                        Tab(selected = submitTab == 1, onClick = { submitTab = 1 }, text = { Text(stringResource(R.string.support_ticket)) })
                    }
                    OutlinedTextField(value = subject, onValueChange = { subject = it }, label = { Text(stringResource(R.string.support_subject)) }, modifier = Modifier.fillMaxWidth())
                    if (submitTab == 1) {
                        OutlinedTextField(value = category, onValueChange = { category = it }, label = { Text(stringResource(R.string.support_category)) }, modifier = Modifier.fillMaxWidth())
                    }
                    OutlinedTextField(value = description, onValueChange = { description = it }, label = { Text(stringResource(R.string.support_description)) }, modifier = Modifier.fillMaxWidth(), minLines = 4)
                    Button(
                        onClick = {
                            if (submitTab == 0) viewModel.submitComplaint(subject, description)
                            else viewModel.submitTicket(subject, description, category)
                            subject = ""
                            description = ""
                        },
                        enabled = subject.isNotBlank() && description.isNotBlank(),
                        modifier = Modifier.fillMaxWidth(),
                    ) {
                        Text(if (submitTab == 0) stringResource(R.string.support_submit_complaint) else stringResource(R.string.support_submit_ticket))
                    }
                    state.message?.let {
                        Text(stringResource(if (it == "complaint") R.string.support_complaint_submitted else R.string.support_ticket_submitted), color = MaterialTheme.colorScheme.primary)
                    }
                    state.error?.let { Text(it, color = MaterialTheme.colorScheme.error) }
                }
                else -> {
                    if (state.isLoading) {
                        Box(Modifier.fillMaxSize(), contentAlignment = Alignment.Center) { CircularProgressIndicator() }
                    } else {
                        LazyColumn(contentPadding = PaddingValues(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                            item { Text(stringResource(R.string.support_my_complaints), style = MaterialTheme.typography.titleMedium) }
                            if (state.complaints.isEmpty()) item { Text(stringResource(R.string.support_no_complaints)) }
                            items(state.complaints, key = { it.id }) { complaint ->
                                Card(Modifier.fillMaxWidth()) {
                                    Column(Modifier.padding(12.dp)) {
                                        Text(complaint.subject, style = MaterialTheme.typography.titleSmall)
                                        Text(complaint.status, color = MaterialTheme.colorScheme.primary)
                                        Text(complaint.description, style = MaterialTheme.typography.bodySmall)
                                    }
                                }
                            }
                            item { Text(stringResource(R.string.support_my_tickets), style = MaterialTheme.typography.titleMedium) }
                            if (state.tickets.isEmpty()) item { Text(stringResource(R.string.support_no_tickets)) }
                            items(state.tickets, key = { it.id }) { ticket ->
                                Card(Modifier.fillMaxWidth()) {
                                    Column(Modifier.padding(12.dp)) {
                                        Text("${ticket.ticketNumber} — ${ticket.subject}", style = MaterialTheme.typography.titleSmall)
                                        Text(ticket.status, color = MaterialTheme.colorScheme.primary)
                                        Text(ticket.description, style = MaterialTheme.typography.bodySmall)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
