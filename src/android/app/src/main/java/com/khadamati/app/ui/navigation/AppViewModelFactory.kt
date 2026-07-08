package com.khadamati.app.ui.navigation

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.khadamati.app.di.AppContainer

class AppViewModelFactory(
    private val container: AppContainer,
    private val creator: AppContainer.() -> ViewModel,
) : ViewModelProvider.Factory {

  @Suppress("UNCHECKED_CAST")
  override fun <T : ViewModel> create(modelClass: Class<T>): T {
    return container.creator() as T
  }
}
