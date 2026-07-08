package com.khadamati.app.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.khadamati.app.data.local.entity.ServiceCategoryEntity
import com.khadamati.app.data.local.entity.ServiceEntity
import com.khadamati.app.data.local.entity.UserEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface UserDao {

    @Query("SELECT * FROM users LIMIT 1")
    fun observeCurrentUser(): Flow<UserEntity?>

    @Query("SELECT * FROM users LIMIT 1")
    suspend fun getCurrentUser(): UserEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun upsert(user: UserEntity)

    @Query("DELETE FROM users")
    suspend fun clear()
}

@Dao
interface ServiceCategoryDao {

    @Query("SELECT * FROM service_categories ORDER BY displayOrder ASC")
    fun observeAll(): Flow<List<ServiceCategoryEntity>>

    @Query("SELECT * FROM service_categories ORDER BY displayOrder ASC")
    suspend fun getAll(): List<ServiceCategoryEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun upsertAll(categories: List<ServiceCategoryEntity>)

    @Query("DELETE FROM service_categories")
    suspend fun clear()
}

@Dao
interface ServiceDao {

    @Query("SELECT * FROM services ORDER BY nameEn ASC")
    fun observeAll(): Flow<List<ServiceEntity>>

    @Query("SELECT * FROM services WHERE categoryId = :categoryId ORDER BY nameEn ASC")
    fun observeByCategory(categoryId: String): Flow<List<ServiceEntity>>

    @Query("SELECT * FROM services ORDER BY nameEn ASC")
    suspend fun getAll(): List<ServiceEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun upsertAll(services: List<ServiceEntity>)

    @Query("DELETE FROM services")
    suspend fun clear()
}
