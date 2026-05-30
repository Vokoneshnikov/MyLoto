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
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4 shadow-sm">
    <div class="container">
      <RouterLink class="navbar-brand fw-bold" to="/home">🎰 MyLoto</RouterLink>

      <div class="collapse navbar-collapse">
        <ul class="navbar-nav me-auto">
          <li class="nav-item">
            <RouterLink class="nav-link" to="/home">Главная</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link" to="/profile">Профиль</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link d-flex align-items-center gap-1 text-danger-hover" to="/live">
              <span class="live-dot me-1"></span> Прямые трансляции
            </RouterLink>
          </li>

          <li class="nav-item" v-if="auth.isAdmin">
            <RouterLink class="nav-link" to="/admin/lotteries">
              Управление лотереями
            </RouterLink>
          </li>
        </ul>

        <div class="d-flex align-items-center" v-if="auth.isLoggedIn">
          <div class="d-flex flex-column align-items-end me-3">
            <span class="text-light small opacity-75">{{ auth.isAdmin ? 'Администратор' : 'Игрок' }}</span>
            <span class="text-light fw-bold">{{ auth.user?.login }}</span>
          </div>
          <button @click="handleLogout" class="btn btn-outline-danger btn-sm rounded-3">Выйти</button>
        </div>

        <div class="d-flex" v-else>
          <RouterLink to="/login" class="btn btn-outline-light me-2 rounded-3">Войти</RouterLink>
          <RouterLink to="/register" class="btn btn-primary rounded-3 px-4">Регистрация</RouterLink>
        </div>
      </div>
    </div>
  </nav>

  <div class="container">
    <RouterView />
  </div>
</template>

<style>
body {
  background-color: #f8f9fa;
}

.nav-link.router-link-active {
  color: #fff !important;
  font-weight: 600;
}

/* Пульсирующая красная точка для Live в шапке */
.live-dot {
  width: 8px;
  height: 8px;
  background-color: #dc3545;
  border-radius: 50%;
  display: inline-block;
  animation: blinker 1.5s linear infinite;
}

@keyframes blinker {
  50% {
    opacity: 0;
  }
}
</style>