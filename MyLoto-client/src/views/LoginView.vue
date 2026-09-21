<template>
  <div class="row justify-content-center">
    <div class="col-md-4">
      <div class="card shadow-sm mt-5">
        <div class="card-body p-4">
          <h2 class="card-title text-center mb-4">Вход</h2>

          <form @submit.prevent="handleLogin">
            <div class="mb-3">
              <label class="form-label">Логин</label>
              <input
                v-model="form.login"
                type="text"
                :class="['form-control', errors.login ? 'is-invalid' : '']"
                @input="errors.login = ''"
                placeholder="Введите ваш логин"
              >
              <div v-if="errors.login" class="invalid-feedback">{{ errors.login }}</div>
            </div>

            <div class="mb-4">
              <label class="form-label">Пароль</label>
              <input
                v-model="form.password"
                type="password"
                :class="['form-control', errors.password ? 'is-invalid' : '']"
                @input="errors.password = ''"
                placeholder="Введите пароль"
              >
              <div v-if="errors.password" class="invalid-feedback">{{ errors.password }}</div>
            </div>

            <div v-if="error" class="alert alert-danger">{{ error }}</div>

            <button type="submit" class="btn btn-primary w-100" :disabled="loading">
              {{ loading ? 'Проверка...' : 'Войти' }}
            </button>
          </form>

          <div class="text-center mt-3">
            <router-link to="/register">Нет аккаунта? Зарегистрироваться</router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { apiRequest } from '@/api/client';
import { useRouter } from 'vue-router';

const auth = useAuthStore();
const router = useRouter();

const form = reactive({ login: '', password: '' });
const errors = reactive({ login: '', password: '' }); // Стейт для локальных ошибок
const loading = ref(false);
const error = ref('');

// Валидатор, полностью синхронизированный с бэкендом
const validateForm = () => {
  let isValid = true;
  errors.login = '';
  errors.password = '';

  if (!form.login.trim()) {
    errors.login = 'Логин обязателен для заполнения';
    isValid = false;
  } else if (form.login.trim().length < 3) {
    errors.login = 'Логин должен быть не менее 3 символов';
    isValid = false;
  }

  if (!form.password) {
    errors.password = 'Пароль обязателен для заполнения';
    isValid = false;
  } else if (form.password.length < 6) {
    errors.password = 'Пароль должен быть не менее 6 символов';
    isValid = false;
  }

  return isValid;
};

const handleLogin = async () => {
  if (!validateForm()) return; // Стопаем, если локальная проверка провалилась

  loading.value = true;
  error.value = '';
  try {
    const data = await apiRequest('/auth/login', 'POST', form);
    auth.setAuth(data); // Сохраняем сессию в Pinia и LocalStorage
    router.push('/profile');
  } catch (e) {
    error.value = e.message;
  } finally {
    loading.value = false;
  }
};
</script>