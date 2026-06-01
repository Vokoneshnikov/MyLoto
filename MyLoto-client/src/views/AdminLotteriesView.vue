<template>
  <div class="container py-5">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-1">Управление лотереями</h2>
        <p class="text-muted mb-0">Приостановка генерации тиражей и запуск новых игр</p>
      </div>
      <router-link to="/admin/lotteries/create" class="btn btn-primary px-4 py-2 rounded-3 fw-bold shadow-sm">
        ➕ Создать лотерею
      </router-link>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="lotteries.length === 0" class="card border-0 shadow-sm rounded-4 p-5 text-center">
      <h4 class="text-muted mb-0">Лотерей пока нет. Создайте первую!</h4>
    </div>

    <div v-else class="card border-0 shadow-sm rounded-4 overflow-hidden">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-dark">
          <tr>
            <th class="ps-4">ID</th>
            <th>Название</th>
            <th>Тип</th>
            <th>Цена билета</th>
            <th>Статус</th>
            <th class="text-end pe-4">Действия</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="loto in lotteries" :key="loto.id">
            <td class="ps-4 fw-bold text-muted">#{{ loto.id }}</td>
            <td>
              <span class="fw-bold text-dark d-block">{{ loto.name }}</span>
              <small class="text-muted d-block text-truncate" style="max-width: 300px;">{{ loto.description }}</small>
            </td>
            <td>
                <span :class="['badge rounded-pill px-2.5 py-1.5', loto.type === 1 ? 'bg-primary-subtle text-primary' : 'bg-warning-subtle text-warning-dark']">
                  {{ loto.type === 1 ? '🔢 K из N' : '🎱 Бинго' }}
                </span>
            </td>
            <td class="fw-semibold">{{ loto.ticketPrice }} ₽</td>
            <td>
                <span :class="['badge rounded-pill px-3 py-2', loto.isPaused ? 'bg-danger-subtle text-danger' : 'bg-success-subtle text-success']">
                  {{ loto.isPaused ? 'Приостановлена' : 'Активна' }}
                </span>
            </td>
            <td class="text-end pe-4">
              <button
                @click="togglePause(loto)"
                :disabled="loto.processing"
                :class="['btn btn-sm rounded-3 px-3 fw-bold border-0 transition-all', loto.isPaused ? 'btn-success text-white' : 'btn-outline-danger']"
              >
                <span v-if="loto.processing" class="spinner-border spinner-border-sm me-1"></span>
                {{ loto.isPaused ? '▶ Возобновить' : '⏸ Приостановить' }}
              </button>
            </td>
          </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { apiRequest } from '@/api/client';
import Swal from 'sweetalert2';

const lotteries = ref([]);
const loading = ref(true);

const fetchLotteries = async () => {
  try {
    lotteries.value = await apiRequest('/lotteries');
    // Добавляем локальное состояние загрузки индивидуально для каждой строчки
    lotteries.value = lotteries.value.map(l => ({ ...l, processing: false }));
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Ошибка загрузки', text: error.message });
  } finally {
    loading.value = false;
  }
};

const togglePause = async (loto) => {
  // 1. Защитный барьер: спрашиваем админа, уверен ли он
  const confirmResult = await Swal.fire({
    title: loto.isPaused ? 'Возобновить лотерею?' : 'Приостановить лотерею?',
    text: loto.isPaused
      ? `Лотерея "${loto.name}" снова станет доступна для покупки билетов и генерации тиражей.`
      : `Автоматический цикл для "${loto.name}" будет заморожен. Текущие открытые тиражи приостановятся.`,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: loto.isPaused ? '#198754' : '#dc3545', // Зеленая для старта, красная для стопа
    cancelButtonColor: '#6c757d',
    confirmButtonText: loto.isPaused ? '▶ Да, запустить' : '⏸ Да, приостановить',
    cancelButtonText: 'Отмена'
  });

  // Если админ нажал "Отмена" — тихо выходим
  if (!confirmResult.isConfirmed) return;

  // 2. Включаем спиннер на кнопке
  loto.processing = true;
  try {
    const isPausedNow = await apiRequest(`/lotteries/${loto.id}/toggle-pause`, 'POST');
    loto.isPaused = isPausedNow;

    // Динамический текст в зависимости от реального статуса
    Swal.fire({
      icon: 'success',
      title: isPausedNow ? 'Цикл приостановлен' : 'Цикл запущен',
      text: isPausedNow
        ? `Лотерея "${loto.name}" успешно извлечена из ротации генерации тиражей.`
        : `Лотерея "${loto.name}" успешно вернулась в активную игру!`,
      timer: 2500,
      showConfirmButton: false
    });
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Ошибка управления', text: error.message });
  } finally {
    loto.processing = false;
  }
};

onMounted(fetchLotteries);
</script>

<style scoped>
.rounded-4 { border-radius: 1.25rem !important; }
.bg-primary-subtle { background-color: rgba(13, 110, 253, 0.12) !important; }
.bg-warning-subtle { background-color: rgba(255, 193, 7, 0.15) !important; }
.text-warning-dark { color: #856404 !important; }
.bg-success-subtle { background-color: rgba(25, 135, 84, 0.12) !important; }
.bg-danger-subtle { background-color: rgba(220, 53, 69, 0.12) !important; }
.transition-all { transition: all 0.2s ease-in-out; }
</style>