<script setup>
import { RouterLink, RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

const handleLogout = () => {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
    <div class="container">
      <RouterLink class="navbar-brand" to="/home">🎰 MyLoto</RouterLink>

      <div class="collapse navbar-collapse">
        <ul class="navbar-nav me-auto">
          <li class="nav-item">
            <RouterLink class="nav-link" to="/home">Главная</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link" to="/profile">Профиль</RouterLink>
          </li>
        </ul>

        <div class="d-flex align-items-center" v-if="auth.isLoggedIn">
          <span class="text-light me-3">Баланс: <strong>{{ auth.balance }} ₽</strong></span>
          <span class="text-light me-3">{{ auth.user?.login }}</span>
          <button @click="handleLogout" class="btn btn-outline-danger btn-sm">Выйти</button>
        </div>

        <div class="d-flex" v-else>
          <RouterLink to="/login" class="btn btn-outline-light me-2">Войти</RouterLink>
          <RouterLink to="/register" class="btn btn-primary">Регистрация</RouterLink>
        </div>
      </div>
    </div>
  </nav>

  <div class="container">
    <RouterView />
  </div>
</template>

<style>
/* Можно добавить глобальные стили здесь */
body {
  background-color: #f8f9fa;
}
</style>