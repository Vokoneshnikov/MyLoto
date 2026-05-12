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
                <input v-model="form.login" type="text" class="form-control" required>
              </div>
              <div class="col-md-6 mb-3">
                <label class="form-label">Email</label>
                <input v-model="form.email" type="email" class="form-control" required>
              </div>
            </div>

            <div class="row">
              <div class="col-md-6 mb-3">
                <label class="form-label">Имя</label>
                <input v-model="form.firstName" type="text" class="form-control" required>
              </div>
              <div class="col-md-6 mb-3">
                <label class="form-label">Фамилия</label>
                <input v-model="form.lastName" type="text" class="form-control" required>
              </div>
            </div>

            <div class="row">
              <div class="col-md-8 mb-3">
                <label class="form-label">Пароль</label>
                <input v-model="form.password" type="password" class="form-control" required>
              </div>
              <div class="col-md-4 mb-3">
                <label class="form-label">Возраст</label>
                <input v-model.number="form.age" type="number" class="form-control" required>
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

const handleRegister = async () => {
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