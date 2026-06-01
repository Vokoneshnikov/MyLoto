<template>
  <div class="row justify-content-center">
    <div class="col-md-6">
      <div class="card shadow-sm mt-5">
        <div class="card-body p-4">
          <h2 class="card-title text-center mb-4">Регистрация</h2>

          <form @submit.prevent="handleRegister">
            <div class="row">
              <div class="col-md-6 mb-3">
                <label class="form-label">Логин</label>
                <input
                  v-model="form.login"
                  type="text"
                  :class="['form-control', errors.login ? 'is-invalid' : '']"
                  @input="errors.login = ''"
                >
                <div v-if="errors.login" class="invalid-feedback">{{ errors.login }}</div>
              </div>
              <div class="col-md-6 mb-3">
                <label class="form-label">Email</label>
                <input
                  v-model="form.email"
                  type="text"
                  :class="['form-control', errors.email ? 'is-invalid' : '']"
                  @input="errors.email = ''"
                >
                <div v-if="errors.email" class="invalid-feedback">{{ errors.email }}</div>
              </div>
            </div>

            <div class="row">
              <div class="col-md-6 mb-3">
                <label class="form-label">Имя</label>
                <input
                  v-model="form.firstName"
                  type="text"
                  :class="['form-control', errors.firstName ? 'is-invalid' : '']"
                  @input="errors.firstName = ''"
                >
                <div v-if="errors.firstName" class="invalid-feedback">{{ errors.firstName }}</div>
              </div>
              <div class="col-md-6 mb-3">
                <label class="form-label">Фамилия</label>
                <input
                  v-model="form.lastName"
                  type="text"
                  :class="['form-control', errors.lastName ? 'is-invalid' : '']"
                  @input="errors.lastName = ''"
                >
                <div v-if="errors.lastName" class="invalid-feedback">{{ errors.lastName }}</div>
              </div>
            </div>

            <div class="row">
              <div class="col-md-8 mb-3">
                <label class="form-label">Пароль</label>
                <input
                  v-model="form.password"
                  type="password"
                  :class="['form-control', errors.password ? 'is-invalid' : '']"
                  @input="errors.password = ''"
                >
                <div v-if="errors.password" class="invalid-feedback">{{ errors.password }}</div>
              </div>
              <div class="col-md-4 mb-3">
                <label class="form-label">Возраст</label>
                <input
                  v-model.number="form.age"
                  type="number"
                  :class="['form-control', errors.age ? 'is-invalid' : '']"
                  @input="errors.age = ''"
                >
                <div v-if="errors.age" class="invalid-feedback">{{ errors.age }}</div>
              </div>
            </div>

            <div v-if="error" class="alert alert-danger">{{ error }}</div>
            <div v-if="success" class="alert alert-success">Регистрация успешна! Переходим ко входу...</div>

            <button type="submit" class="btn btn-primary w-100" :disabled="loading">
              {{ loading ? 'Создаем аккаунт...' : 'Зарегистрироваться' }}
            </button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { apiRequest } from '@/api/client';
import { useRouter } from 'vue-router';

const router = useRouter();
const loading = ref(false);
const error = ref('');
const success = ref(false);

const form = reactive({
  login: '',
  email: '',
  password: '',
  firstName: '',
  lastName: '',
  age: 18
});

// Локальный стейт для хранения ошибок полей
const errors = reactive({
  login: '',
  email: '',
  firstName: '',
  lastName: '',
  password: '',
  age: ''
});

// Синхронный валидатор, дублирующий правила FluentValidation с бэка
const validateForm = () => {
  let isValid = true;

  // Сбрасываем старые ошибки
  Object.keys(errors).forEach(key => errors[key] = '');

  // 1. Проверка логина
  if (!form.login.trim()) {
    errors.login = 'Логин обязателен для заполнения';
    isValid = false;
  } else if (form.login.trim().length < 3) {
    errors.login = 'Логин должен содержать минимум 3 символа';
    isValid = false;
  }

  // 2. Проверка Email регулярным выражением
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!form.email.trim()) {
    errors.email = 'Email обязателен';
    isValid = false;
  } else if (!emailRegex.test(form.email.trim())) {
    errors.email = 'Введите корректный Email адрес';
    isValid = false;
  }

  // 3. Проверка Имени и Фамилии
  if (!form.firstName.trim()) {
    errors.firstName = 'Имя обязательно';
    isValid = false;
  }
  if (!form.lastName.trim()) {
    errors.lastName = 'Фамилия обязательна';
    isValid = false;
  }

  // 4. Проверка пароля
  if (!form.password) {
    errors.password = 'Пароль обязателен';
    isValid = false;
  } else if (form.password.length < 6) {
    errors.password = 'Пароль должен быть не менее 6 символов';
    isValid = false;
  }

  // 5. Законодательный рубеж (18+)
  if (form.age === null || form.age === undefined || form.age === '') {
    errors.age = 'Укажите возраст';
    isValid = false;
  } else if (form.age < 18) {
    errors.age = 'Доступно строго с 18 лет';
    isValid = false;
  }

  return isValid;
};

const handleRegister = async () => {
  if (!validateForm()) return; // Если форма не валидна — никуда не летим

  loading.value = true;
  error.value = '';
  try {
    await apiRequest('/auth/register', 'POST', form);
    success.value = true;
    setTimeout(() => router.push('/login'), 2000);
  } catch (e) {
    error.value = e.message;
  } finally {
    loading.value = false;
  }
};
</script>