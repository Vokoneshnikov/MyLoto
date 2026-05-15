<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-lg-10">
        <div class="card border-0 shadow-sm rounded-4 p-4">
          <h2 class="fw-bold mb-4">Создание новой лотереи</h2>
          <hr class="mb-4">

          <form @submit.prevent="submitForm">
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <label class="form-label fw-semibold">Название</label>
                <input v-model="form.name" type="text" class="form-control rounded-3" placeholder="Например: Супер 6 из 45" required>
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
                <textarea v-model="form.description" class="form-control rounded-3" rows="2" placeholder="Краткое описание правил..." required></textarea>
              </div>
            </div>

            <div class="row g-3 mb-4 bg-light p-3 rounded-3">
              <div class="col-md-3">
                <label class="form-label fw-semibold">Цена билета (₽)</label>
                <input v-model.number="form.ticketPrice" type="number" class="form-control" required>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Начальный джекпот</label>
                <input v-model.number="form.jackpotValue" type="number" class="form-control">
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Продажа (мин)</label>
                <input v-model.number="durations.sales" type="number" class="form-control" placeholder="Минуты">
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Розыгрыш (мин)</label>
                <input v-model.number="durations.draw" type="number" class="form-control" placeholder="Минуты">
              </div>
            </div>

            <div v-if="form.type === 1" class="row g-3 mb-4 border-start border-primary border-4 ps-3">
              <div class="col-md-6">
                <label class="form-label fw-semibold">Сколько чисел выбирает игрок (K)</label>
                <input v-model.number="form.k" type="number" class="form-control" placeholder="6">
              </div>
              <div class="col-md-6">
                <label class="form-label fw-semibold">Из скольки чисел (N)</label>
                <input v-model.number="form.n" type="number" class="form-control" placeholder="45">
              </div>
            </div>

            <div v-if="form.type === 2" class="row g-3 mb-4 border-start border-warning border-4 ps-3">
              <div class="col-md-3">
                <label class="form-label fw-semibold">Строк</label>
                <input v-model.number="form.rows" type="number" class="form-control" placeholder="3">
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Колонок</label>
                <input v-model.number="form.columns" type="number" class="form-control" placeholder="10">
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Макс. шар</label>
                <input v-model.number="form.maxBallValue" type="number" class="form-control" placeholder="90">
              </div>
              <div class="col-md-3">
                <label class="form-label fw-semibold">Порог Джекпота</label>
                <input v-model.number="form.jackpotThreshold" type="number" class="form-control" placeholder="5">
              </div>
            </div>

            <div class="mb-4">
              <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="fw-bold mb-0">Призовые уровни (Prize Tiers)</h5>
                <button type="button" @click="addPrizeTier" class="btn btn-sm btn-outline-primary">+ Добавить уровень</button>
              </div>

              <div v-for="(tier, index) in form.prizeTiers" :key="index" class="row g-2 mb-2 align-items-end shadow-sm p-2 rounded-2 bg-white">
                <div class="col-md-5">
                  <label class="small text-muted">Тип правила</label>
                  <select v-model.number="tier.ruleType" class="form-select form-select-sm">
                    <option v-if="form.type === 1" :value="1">Угадано чисел (MatchedNumbers)</option>
                    <option v-if="form.type === 2" :value="2">Закрыто на шаре (ClosedAtBall)</option>
                    <option v-if="form.type === 2" :value="3">Джекпот (Jackpot)</option>
                  </select>
                </div>
                <div class="col-md-3">
                  <label class="small text-muted">Значение (K или № шара)</label>
                  <input v-model.number="tier.conditionValue" type="number" class="form-control form-select-sm">
                </div>
                <div class="col-md-3">
                  <label class="small text-muted">Множитель (x)</label>
                  <input v-model.number="tier.rewardMultiplier" type="number" step="0.1" class="form-control form-select-sm">
                </div>
                <div class="col-md-1">
                  <button type="button" @click="removePrizeTier(index)" class="btn btn-sm btn-outline-danger w-100">×</button>
                </div>
              </div>
            </div>

            <div class="d-grid gap-2">
              <button type="submit" class="btn btn-primary py-3 fw-bold rounded-3" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                Создать лотерею
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

const router = useRouter();
const loading = ref(false);

// Вспомогательные данные для времени (минуты -> TimeSpan)
const durations = reactive({
  sales: 45,
  draw: 15
});

const form = reactive({
  name: '',
  description: '',
  ticketPrice: 100,
  type: 1, // 1 = K_Out_Of_N, 2 = Bingo
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

// Очистка призовых уровней при смене типа лотереи
watch(() => form.type, () => {
  form.prizeTiers = [];
  if (form.type === 1) {
    form.k = 6; form.n = 45;
    form.rows = form.columns = form.maxBallValue = form.jackpotThreshold = null;
  } else {
    form.rows = 3; form.columns = 10; form.maxBallValue = 90; form.jackpotThreshold = 5;
    form.k = form.n = null;
  }
}, { immediate: true });

const addPrizeTier = () => {
  form.prizeTiers.push({
    ruleType: form.type === 1 ? 1 : 2,
    conditionValue: 0,
    rewardMultiplier: 1.0
  });
};

const removePrizeTier = (index) => {
  form.prizeTiers.splice(index, 1);
};

const submitForm = async () => {
  loading.value = true;
  try {
    // Конвертируем минуты в формат HH:mm:ss для TimeSpan
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
    alert('Лотерея успешно создана!');
    router.push('/home');
  } catch (error) {
    alert('Ошибка: ' + error.message);
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.rounded-4 { border-radius: 1.25rem !important; }
.form-control:focus, .form-select:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.1);
}
</style>