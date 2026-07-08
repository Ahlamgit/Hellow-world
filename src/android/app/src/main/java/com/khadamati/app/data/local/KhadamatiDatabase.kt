package com.khadamati.app.data.local

import androidx.room.Database
import androidx.room.RoomDatabase
import com.khadamati.app.data.local.dao.ServiceCategoryDao
import com.khadamati.app.data.local.dao.ServiceDao
import com.khadamati.app.data.local.dao.UserDao
import com.khadamati.app.data.local.entity.ServiceCategoryEntity
import com.khadamati.app.data.local.entity.ServiceEntity
import com.khadamati.app.data.local.entity.UserEntity

@Database(
    entities = [
        UserEntity::class,
        ServiceCategoryEntity::class,
        ServiceEntity::class,
    ],
    version = 1,
    exportSchema = false,
)
abstract class KhadamatiDatabase : RoomDatabase() {
    abstract fun userDao(): UserDao
    abstract fun serviceCategoryDao(): ServiceCategoryDao
    abstract fun serviceDao(): ServiceDao
}
