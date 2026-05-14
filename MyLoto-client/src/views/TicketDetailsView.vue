<template>
  <div class="container py-5">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary"></div>
    </div>

    <div v-else-if="ticket" class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <!-- Кнопка Назад -->
        <div class="mb-4">
          <router-link to="/profile" class="btn btn-link text-decoration-none p-0 text-dark">
            <i class="bi bi-arrow-left me-2"></i> Вернуться в профиль
          </router-link>
        </div>

        <!-- Карточка билета -->
        <div class="card border-0 shadow-lg rounded-4 overflow-hidden ticket-card">
          <div class="card-header bg-primary text-white p-4 border-0 position-relative">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="text-white-50 text-uppercase small fw-bold mb-1">Лотерейный билет</h6>
                <h3 class="fw-bold mb-0">#{{ ticket.ticketId }}</h3>
              </div>
              <div class="text-end">
                <span class="badge bg-white text-primary rounded-pill px-3 py-2">Активен</span>
              </div>
            </div>
            <!-- Декоративные круги по бокам (перфорация) -->
            <div class="ticket-cutout left"></div>
            <div class="ticket-cutout right"></div>
          </div>

          <div class="card-body p-4 p-md-5">
            <div class="mb-5 text-center">
              <h5 class="text-muted mb-3">Ваша комбинация:</h5>
              <div class="d-flex flex-wrap justify-content-center gap-2">
                <div v-for="num in ticket.chosenNumbers" :key="num"
                     class="lotto-ball shadow-sm">
                  {{ num }}
                </div>
              </div>
            </div>

            <div class="row g-4 mb-4">
              <div class="col-6">
                <label class="text-muted small d-block mb-1">Тираж</label>
                <span class="fw-bold d-block">{{ ticket.lotteryName }}</span>
              </div>
              <div class="col-6 text-end">
                <label class="text-muted small d-block mb-1">Дата покупки</label>
                <span class="fw-bold d-block">{{ formatDate(ticket.purchasedAt) }}</span>
              </div>
              <div class="col-6">
                <label class="text-muted small d-block mb-1">Стоимость</label>
                <span class="fw-bold d-block text-success">{{ ticket.price || 100 }} ₽</span>
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
                <i class="bi bi-share me-2"></i> Поделиться в соцсетях
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
      <router-link to="/profile" class="btn btn-primary mt-3">Вернуться</router-link>
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

const formatDate = (dateString) => {
  if (!dateString) return 'Неизвестно';
  return new Date(dateString).toLocaleDateString('ru-RU', {
    day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit'
  });
};

onMounted(async () => {
  try {
    const id = route.params.id;
    // Предполагаем, что у тебя есть эндпоинт для получения одного билета
    // Если его нет, можно отфильтровать из общего списка профиля
    const allTickets = await apiRequest('/profile/tickets');
    ticket.value = allTickets.find(t => t.ticketId == id);
  } catch (e) {
    console.error("Ошибка загрузки билета:", e);
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

.ticket-cutout {
  position: absolute;
  bottom: -15px;
  width: 30px;
  height: 30px;
  background: #f8f9fa; /* Должен совпадать с цветом фона страницы */
  border-radius: 50%;
}
.ticket-cutout.left { left: -15px; }
.ticket-cutout.right { right: -15px; }

.border-dashed { border-style: dashed !important; }
</style>