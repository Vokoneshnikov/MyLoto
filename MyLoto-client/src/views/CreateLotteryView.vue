<template>
  <div class="container py-5">
    <nav aria-label="breadcrumb" class="mb-4">
      <router-link to="/admin/lotteries" class="text-decoration-none text-muted small">← Назад в панель управления</router-link>
    </nav>

    <div class="row justify-content-center">
      <div class="col-lg-10">
        <div class="card border-0 shadow-sm rounded-4 p-4">
          <h2 class="fw-bold mb-4">Создание новой лотереи</h2>
          <hr class="mb-4">

          <form @submit.prevent="submitForm">
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <label class="form-label fw-semibold">Название</label>
                <input
                  v-model="form.name"
                  type="text"
                  :class="['form-control rounded-3', errors.name ? 'is-invalid' : '']"
                  placeholder="Например: Супер 6 из 45"
                  @input="errors.name = ''"
                >
                <div v-if="errors.name" class="invalid-feedback">{{ errors.name }}</div>
              </div>
              <div class="col-md-6">
                <label class="form-label fw-semibold">Тип лотереи</label>
                <select v-model.number="form.type" class="form-select rounded-3">
                  <option :value="1">Числовая (K из N)</option>
                  <option :value="2">Бинго</option>
                </select>
              </div>
              <div class="col-12">
                <label class="form-label fw-semibold">Описание</label>
                <textarea
                  v-model="form.description"
                  :class="['form-control rounded-3', errors.description ? 'is-invalid' : '']"
                  rows="2"
                  placeholder="Краткое описание правил..."
                  @input="errors.description = ''"
                ></textarea>
                <div v-if="errors.description" class="invalid-feedback">{{ errors.description }}</div>
              </div>
            </div>

            <div class="row g-3 mb-4 bg-light p-3 rounded-3">
              <div class="col-md-3">
                <label class="form-label fw-semibold">Цена билета (₽)</label>
                <input
                  v-model.number="form.ticketPrice"
                  type="number"
                  :class="['form-control rounded-3', errors.ticketPrice ? 'is-invalid' : '']"
                  @input="errors.ticketPrice = ''"
                >
                <div v-if="errors.ticketPrice" class="invalid-feedback">{{ errors.ticketPrice }}</div>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Начальный джекпот (₽)</label>
                <input
                  v-model.number="form.jackpotValue"
                  type="number"
                  :class="['form-control rounded-3', errors.jackpotValue ? 'is-invalid' : '']"
                  @input="errors.jackpotValue = ''"
                >
                <div v-if="errors.jackpotValue" class="invalid-feedback">{{ errors.jackpotValue }}</div>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Продажа (мин)</label>
                <input
                  v-model.number="durations.sales"
                  type="number"
                  :class="['form-control rounded-3', errors.sales ? 'is-invalid' : '']"
                  placeholder="Минуты"
                  @input="errors.sales = ''"
                >
                <div v-if="errors.sales" class="invalid-feedback">{{ errors.sales }}</div>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Розыгрыш (мин)</label>
                <input
                  v-model.number="durations.draw"
                  type="number"
                  :class="['form-control rounded-3', errors.draw ? 'is-invalid' : '']"
                  placeholder="Минуты"
                  @input="errors.draw = ''"
                >
                <div v-if="errors.draw" class="invalid-feedback">{{ errors.draw }}</div>
              </div>
            </div>

            <div v-if="form.type === 1" class="row g-3 mb-4 border-start border-primary border-4 ps-3 transition-all">
              <div class="col-md-6">
                <label class="form-label fw-semibold">Сколько чисел выбирает игрок (K)</label>
                <input
                  v-model.number="form.k"
                  type="number"
                  :class="['form-control rounded-3', errors.k ? 'is-invalid' : '']"
                  placeholder="6"
                  @input="errors.k = ''"
                >
                <div v-if="errors.k" class="invalid-feedback">{{ errors.k }}</div>
              </div>
              <div class="col-md-6">
                <label class="form-label fw-semibold">Из скольки чисел (N)</label>
                <input
                  v-model.number="form.n"
                  type="number"
                  :class="['form-control rounded-3', errors.n ? 'is-invalid' : '']"
                  placeholder="45"
                  @input="errors.n = ''"
                >
                <div v-if="errors.n" class="invalid-feedback">{{ errors.n }}</div>
              </div>
            </div>

            <div v-if="form.type === 2" class="row g-3 mb-4 border-start border-warning border-4 ps-3 transition-all">
              <div class="col-md-3">
                <label class="form-label fw-semibold">Строк</label>
                <input
                  v-model.number="form.rows"
                  type="number"
                  :class="['form-control rounded-3', errors.rows ? 'is-invalid' : '']"
                  placeholder="3"
                  @input="errors.rows = ''"
                >
                <div v-if="errors.rows" class="invalid-feedback">{{ errors.rows }}</div>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Колонок</label>
                <input
                  v-model.number="form.columns"
                  type="number"
                  :class="['form-control rounded-3', errors.columns ? 'is-invalid' : '']"
                  placeholder="10"
                  @input="errors.columns = ''"
                >
                <div v-if="errors.columns" class="invalid-feedback">{{ errors.columns }}</div>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Макс. шар</label>
                <input
                  v-model.number="form.maxBallValue"
                  type="number"
                  :class="['form-control rounded-3', errors.maxBallValue ? 'is-invalid' : '']"
                  placeholder="90"
                  @input="errors.maxBallValue = ''"
                >
                <div v-if="errors.maxBallValue" class="invalid-feedback">{{ errors.maxBallValue }}</div>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Порог Джекпота</label>
                <input
                  v-model.number="form.jackpotThreshold"
                  type="number"
                  :class="['form-control rounded-3', errors.jackpotThreshold ? 'is-invalid' : '']"
                  placeholder="5"
                  @input="errors.jackpotThreshold = ''"
                >
                <div v-if="errors.jackpotThreshold" class="invalid-feedback">{{ errors.jackpotThreshold }}</div>
              </div>
            </div>

            <div class="mb-4">
              <div class="d-flex justify-content-between align-items-center mb-3">
                <div>
                  <h5 class="fw-bold mb-0">Призовые уровни (Prize Tiers)</h5>
                  <small v-if="errors.prizeTiers" class="text-danger fw-semibold d-block mt-1">{{ errors.prizeTiers }}</small>
                </div>
                <button type="button" @click="addPrizeTier" class="btn btn-sm btn-outline-primary rounded-3 px-3">+ Добавить уровень</button>
              </div>

              <div v-for="(tier, index) in form.prizeTiers" :key="index" class="row g-2 mb-2 align-items-end shadow-sm p-3 rounded-3 bg-white border border-light">
                <div class="col-md-5">
                  <label class="small text-muted fw-semibold">Тип правила</label>
                  <select v-model.number="tier.ruleType" class="form-select form-select-sm rounded-2" @change="errors.prizeTiers = ''">
                    <option v-if="form.type === 1" :value="1">Угадано чисел (MatchedNumbers)</option>
                    <option v-if="form.type === 2" :value="2">Закрыто на шаре (ClosedAtBall)</option>
                    <option v-if="form.type === 2" :value="3">Джекпот (Jackpot)</option>
                  </select>
                </div>
                <div class="col-md-3">
                  <label class="small text-muted fw-semibold">Значение (K или № шара)</label>
                  <input v-model.number="tier.conditionValue" type="number" class="form-control form-control-sm rounded-2" @input="errors.prizeTiers = ''">
                </div>
                <div class="col-md-3">
                  <label class="small text-muted fw-semibold">Множитель (x)</label>
                  <input v-model.number="tier.rewardMultiplier" type="number" step="0.1" class="form-control form-control-sm rounded-2" @input="errors.prizeTiers = ''">
                </div>
                <div class="col-md-1">
                  <button type="button" @click="removePrizeTier(index)" class="btn btn-sm btn-outline-danger w-100 rounded-2">×</button>
                </div>
              </div>
            </div>

            <div class="d-grid gap-2">
              <button type="submit" class="btn btn-dark py-3 fw-bold rounded-3 shadow-sm" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                🚀 Создать и запустить лотерею
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, watch } from 'vue';
import { useRouter } from 'vue-router';
import { apiRequest } from '@/api/client';
import Swal from 'sweetalert2';

const router = useRouter();
const loading = ref(false);

const durations = reactive({ sales: 45, draw: 15 });

const form = reactive({
  name: '',
  description: '',
  ticketPrice: 100,
  type: 1,
  jackpotValue: 0,
  prizeTiers: [],
  k: null,
  n: null,
  rows: null,
  columns: null,
  maxBallValue: null,
  jackpotThreshold: null,
  isPaused: false
});

// Глобальный реактивный стейт ошибок
const errors = reactive({
  name: '',
  description: '',
  ticketPrice: '',
  jackpotValue: '',
  sales: '',
  draw: '',
  k: '',
  n: '',
  rows: '',
  columns: '',
  maxBallValue: '',
  jackpotThreshold: '',
  prizeTiers: ''
});

watch(() => form.type, () => {
  form.prizeTiers = [];
  // Очищаем ошибки при смене типа лотереи
  Object.keys(errors).forEach(key => errors[key] = '');

  if (form.type === 1) {
    form.k = 6; form.n = 45;
    form.rows = form.columns = form.maxBallValue = form.jackpotThreshold = null;
  } else {
    form.rows = 3; form.columns = 10; form.maxBallValue = 90; form.jackpotThreshold = 5;
    form.k = form.n = null;
  }
}, { immediate: true });

const addPrizeTier = () => {
  errors.prizeTiers = '';
  form.prizeTiers.push({
    ruleType: form.type === 1 ? 1 : 2,
    conditionValue: 0,
    rewardMultiplier: 1.0
  });
};

const removePrizeTier = (index) => {
  form.prizeTiers.splice(index, 1);
};

// Комплексный валидатор всей админской формы
const validateForm = () => {
  let isValid = true;
  Object.keys(errors).forEach(key => errors[key] = '');

  // 1. Простые базовые валидации
  if (!form.name.trim()) { errors.name = 'Введите название лотереи'; isValid = false; }
  if (!form.description.trim()) { errors.description = 'Заполните описание правил'; isValid = false; }
  if (form.ticketPrice <= 0) { errors.ticketPrice = 'Цена должна быть строго больше 0 ₽'; isValid = false; }
  if (form.jackpotValue < 0) { errors.jackpotValue = 'Джекпот не может быть отрицательным'; isValid = false; }
  if (durations.sales <= 0) { errors.sales = 'Укажите время продаж'; isValid = false; }
  if (durations.draw <= 0) { errors.draw = 'Укажите время розыгрыша'; isValid = false; }

  // 2. Специфичная валидация для Числовой лотереи (K из N)
  if (form.type === 1) {
    if (!form.k || form.k <= 0) { errors.k = 'Введите корректное число K'; isValid = false; }
    if (!form.n || form.n <= 0) { errors.n = 'Введите корректное число N'; isValid = false; }
    if (form.k && form.n && form.k >= form.n) {
      errors.k = 'Выбираемые числа (K) должны быть строго меньше общего пула (N)';
      isValid = false;
    }
  }

  // 3. Специфичная валидация для Бинго
  if (form.type === 2) {
    if (!form.rows || form.rows <= 0) { errors.rows = 'Неверное число строк'; isValid = false; }
    if (!form.columns || form.columns <= 0) { errors.columns = 'Неверное число колонок'; isValid = false; }
    if (!form.maxBallValue || form.maxBallValue <= 0) { errors.maxBallValue = 'Укажите макс. номер шара'; isValid = false; }
    if (!form.jackpotThreshold || form.jackpotThreshold <= 0) { errors.jackpotThreshold = 'Укажите порог джекпота'; isValid = false; }

    if (form.rows && form.columns && form.maxBallValue && (form.rows * form.columns > form.maxBallValue)) {
      errors.maxBallValue = 'Максимальный шар не может быть меньше, чем размер билета (Строки × Колонки)';
      isValid = false;
    }
  }

  // 4. Глубокая валидация массива призовых уровней
  if (form.prizeTiers.length === 0) {
    errors.prizeTiers = '🚨 Необходимо добавить хотя бы один призовой уровень!';
    isValid = false;
  } else {
    for (let i = 0; i < form.prizeTiers.length; i++) {
      const tier = form.prizeTiers[i];
      if (tier.conditionValue <= 0) {
        errors.prizeTiers = `Ошибка в строке ${i + 1}: Условие должно быть больше нуля.`;
        isValid = false;
        break;
      }
      if (tier.rewardMultiplier <= 0) {
        errors.prizeTiers = `Ошибка в строке ${i + 1}: Множитель награды должен быть больше нуля.`;
        isValid = false;
        break;
      }
      // Логическая проверка по типам лотереи
      if (form.type === 1 && tier.conditionValue > form.k) {
        errors.prizeTiers = `Ошибка в строке ${i + 1}: Нельзя угадать чисел больше, чем доступно для выбора (${form.k}).`;
        isValid = false;
        break;
      }
      if (form.type === 2 && tier.conditionValue > form.maxBallValue) {
        errors.prizeTiers = `Ошибка в строке ${i + 1}: Номер шара не может превышать максимальный шар системы (${form.maxBallValue}).`;
        isValid = false;
        break;
      }
    }
  }

  return isValid;
};

const submitForm = async () => {
  if (!validateForm()) {
    // Скроллим наверх к первой ошибке для удобства админа
    window.scrollTo({ top: 0, behavior: 'smooth' });
    return;
  }

  loading.value = true;
  try {
    const toTimeSpan = (mins) => {
      const h = Math.floor(mins / 60).toString().padStart(2, '0');
      const m = (mins % 60).toString().padStart(2, '0');
      return `${h}:${m}:00`;
    };

    const payload = {
      ...form,
      ticketSalesDuration: toTimeSpan(durations.sales),
      drawProcessingDuration: toTimeSpan(durations.draw)
    };

    await apiRequest('/lotteries/create', 'POST', payload);

    await Swal.fire({
      icon: 'success',
      title: 'Лотерея создана!',
      text: 'Новая игра успешно добавлена в систему.',
      confirmButtonColor: '#212529',
      timer: 2500
    });

    router.push('/admin/lotteries');
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Ошибка создания', text: error.message });
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.rounded-4 { border-radius: 1.25rem !important; }
.form-control:focus, .form-select:focus {
  border-color: #212529;
  box-shadow: 0 0 0 0.25rem rgba(33, 37, 41, 0.08);
}
.transition-all { transition: all 0.2s ease; }
</style>