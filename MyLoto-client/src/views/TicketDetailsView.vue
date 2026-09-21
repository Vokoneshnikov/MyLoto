<template>
  <div class="container py-5">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary"></div>
    </div>

    <div v-else-if="ticket" class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <div class="mb-4">
          <router-link to="/profile" class="btn btn-link text-decoration-none p-0 text-dark">
            <i class="bi bi-arrow-left me-2"></i> Вернуться в профиль
          </router-link>
        </div>

        <div class="card border-0 shadow-lg rounded-4 overflow-hidden ticket-card">
          <div class="card-header p-4 border-0 position-relative"
               :class="!ticket.isChecked ? 'bg-primary text-white' : (ticket.winAmount > 0 ? 'bg-success text-white' : 'bg-secondary text-white')">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="text-white-50 text-uppercase small fw-bold mb-1">Лотерейный билет</h6>
                <h3 class="fw-bold mb-0">#{{ ticket.ticketId }}</h3>
              </div>
              <div class="text-end">
                <span class="badge bg-white rounded-pill px-3 py-2"
                      :class="!ticket.isChecked ? 'text-primary' : (ticket.winAmount > 0 ? 'text-success' : 'text-secondary')">
                  {{ !ticket.isChecked ? 'В игре' : (ticket.winAmount > 0 ? 'Выиграл' : 'Без выигрыша') }}
                </span>
              </div>
            </div>
            <div class="ticket-cutout left"></div>
            <div class="ticket-cutout right"></div>
          </div>

          <div class="card-body p-4 p-md-5">
            <div class="mb-5 text-center">
              <h5 class="text-muted mb-3">Ваша комбинация:</h5>
              <div class="d-flex flex-wrap justify-content-center gap-2">
                <div v-for="num in ticket.selectedNumbers" :key="num"
                     :class="[
                       'lotto-ball shadow-sm transition-all',
                       ticket.drawWinningNumbers.includes(num) ? 'matched text-white border-success' : ''
                     ]">
                  {{ num }}
                </div>
              </div>
            </div>

            <div class="row g-4 mb-4">
              <div class="col-6">
                <label class="text-muted small d-block mb-1">Тираж</label>
                <span class="fw-bold d-block">№ {{ ticket.drawId }}</span>
              </div>
              <div class="col-6 text-end">
                <label class="text-muted small d-block mb-1">Статус розыгрыша</label>
                <span class="fw-bold d-block text-capitalize">{{ ticket.drawStatus }}</span>
              </div>
              <div class="col-6">
                <label class="text-muted small d-block mb-1">Сумма выигрыша</label>
                <span class="fw-bold d-block" :class="ticket.winAmount > 0 ? 'text-success' : 'text-dark'">
                  {{ ticket.winAmount.toLocaleString() }} ₽
                </span>
              </div>
              <div class="col-6 text-end">
                <label class="text-muted small d-block mb-1">ID Транзакции</label>
                <span class="font-monospace small text-muted">TXN-{{ ticket.ticketId * 7 }}</span>
              </div>
            </div>

            <hr class="my-4 border-dashed">

            <div class="d-grid gap-2">
              <button @click="window.print()" class="btn btn-outline-dark rounded-pill">
                <i class="bi bi-printer me-2"></i> Распечатать билет
              </button>
              <button class="btn btn-primary rounded-pill">
                <i class="bi bi-share me-2"></i> Поделиться результатами
              </button>
            </div>
          </div>

          <div class="card-footer bg-light p-3 text-center border-0">
            <small class="text-muted">Этот билет является цифровым подтверждением участия в розыгрыше.</small>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-5">
      <h3>Билет не найден</h3>
      <p class="text-muted small">Не удалось найти билет с #{{ $route.params.id }} в вашей истории.</p>
      <router-link to="/profile" class="btn btn-primary mt-3 rounded-pill px-4">Вернуться в профиль</router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { apiRequest } from '@/api/client';

const route = useRoute();
const ticket = ref(null);
const loading = ref(true);

onMounted(async () => {
  try {
    const id = route.params.id;

    // 🔥 ИСПРАВЛЕНО: Так как эндпоинт требует параметры, запрашиваем параллельно активные и архивные билеты
    const [activeTickets, archiveTickets] = await Promise.all([
      apiRequest('/profile/tickets?isArchive=false'),
      apiRequest('/profile/tickets?isArchive=true')
    ]);

    // Объединяем оба списка в один пул для поиска
    const allTickets = [...activeTickets, ...archiveTickets];

    // 🔥 ИСПРАВЛЕНО: Ищем билет по новому свойству ticketId (из нашей DTO рекорда)
    ticket.value = allTickets.find(t => t.ticketId == id);

    console.log("Найденный билет для детализации:", ticket.value);
  } catch (e) {
    console.error("Ошибка загрузки деталей билета:", e);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.ticket-card { border-radius: 1.5rem !important; }

.lotto-ball {
  width: 50px;
  height: 50px;
  background: radial-gradient(circle at 30% 30%, #ffffff, #f0f0f0);
  border: 2px solid #0d6efd;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 1.2rem;
  color: #0d6efd;
}

/* Стиль для совпавшего победного бочонка */
.lotto-ball.matched {
  background: radial-gradient(circle at 30% 30%, #198754, #146c43);
  border-color: #198754;
  color: white !important;
  transform: scale(1.05);
}

.ticket-cutout {
  position: absolute;
  bottom: -15px;
  width: 30px;
  height: 30px;
  background: #ffffff; /* Если фон страницы изменится, поменяй цвет тут */
  border-radius: 50%;
}
.ticket-cutout.left { left: -15px; }
.ticket-cutout.right { right: -15px; }

.border-dashed { border-style: dashed !important; }
.transition-all { transition: all 0.2s ease-in-out; }
</style>