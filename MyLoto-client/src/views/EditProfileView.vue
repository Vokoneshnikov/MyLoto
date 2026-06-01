<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <div class="card border-0 shadow-sm rounded-4">
          <div class="card-body p-4 p-md-5">
            <div class="d-flex align-items-center mb-4">
              <router-link to="/profile" class="btn btn-link text-decoration-none p-0 me-3">
                <i class="bi bi-arrow-left fs-4"></i>
              </router-link>
              <h2 class="fw-bold mb-0">Редактирование профиля</h2>
            </div>

            <form @submit.prevent="handleUpdate">
              <div class="row g-3">
                <!-- Имя -->
                <div class="col-md-6">
                  <label class="form-label small fw-bold text-muted">Имя</label>
                  <input
                    v-model="form.Name"
                    type="text"
                    :class="['form-control form-control-lg rounded-3', errors.Name ? 'is-invalid' : '']"
                    placeholder="Введите имя"
                    @input="errors.Name = ''"
                  >
                  <div v-if="errors.Name" class="invalid-feedback">{{ errors.Name }}</div>
                </div>

                <!-- 	Фамилия -->
                <div class="col-md-6">
                  <label class="form-label small fw-bold text-muted">Фамилия</label>
                  <input
                    v-model="form.Surname"
                    type="text"
                    :class="['form-control form-control-lg rounded-3', errors.Surname ? 'is-invalid' : '']"
                    placeholder="Введите фамилию"
                    @input="errors.Surname = ''"
                  >
                  <div v-if="errors.Surname" class="invalid-feedback">{{ errors.Surname }}</div>
                </div>

                <!-- Адрес (Address) -->
                <div class="col-12">
                  <label class="form-label small fw-bold text-muted">Адрес</label>
                  <input
                    v-model="form.Address"
                    type="text"
                    class="form-control rounded-3"
                    placeholder="Ваш город или адрес"
                  >
                </div>

                <!-- Кнопки -->
                <div class="col-12 mt-4">
                  <button
                    type="submit"
                    class="btn btn-primary btn-lg w-100 rounded-pill fw-bold"
                    :disabled="saving"
                  >
                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                    Сохранить изменения
                  </button>
                  <router-link to="/profile" class="btn btn-light btn-lg w-100 rounded-pill mt-2">
                    Отмена
                  </router-link>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { apiRequest } from '@/api/client';

const router = useRouter();
const saving = ref(false);

const form = ref({
  Name: '',
  Surname: '',
  Address: ''
});

// Локальный стейт для отслеживания ошибок валидации
const errors = reactive({
  Name: '',
  Surname: ''
});

// Загружаем текущие данные, чтобы пользователь видел, что редактирует
onMounted(async () => {
  try {
    const currentProfile = await apiRequest('/profile');
    form.value = {
      Name: currentProfile.firstName || '',
      Surname: currentProfile.lastName || '',
      Address: currentProfile.Address || '',
    };
    console.log(currentProfile);
  } catch (e) {
    console.error("Не удалось загрузить данные профиля:", e.message);
  }
});

// Функция валидации, синхронизированная с требованиями доменной модели бэка
const validateForm = () => {
  let isValid = true;
  errors.Name = '';
  errors.Surname = '';

  if (!form.value.Name.trim()) {
    errors.Name = 'Имя обязательно для заполнения';
    isValid = false;
  }

  if (!form.value.Surname.trim()) {
    errors.Surname = 'Фамилия обязательна для заполнения';
    isValid = false;
  }

  return isValid;
};

const handleUpdate = async () => {
  if (!validateForm()) return; // Отсекаем пустые отправки на взлете

  saving.value = true;
  try {
    // Отправляем PUT запрос на обновление
    await apiRequest('/profile', 'PUT', form.value);

    // Если всё ок, возвращаемся в профиль
    alert("Профиль успешно обновлен!");
    router.push('/profile');
  } catch (e) {
    alert("Ошибка при обновлении: " + e.message);
  } finally {
    saving.value = false;
  }
};
</script>

<style scoped>
.rounded-4 { border-radius: 1.25rem !important; }
.form-control:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.1);
}
</style>