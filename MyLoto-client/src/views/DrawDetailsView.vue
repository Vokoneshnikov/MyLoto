<template>
  <div class="container py-5">
    <nav aria-label="breadcrumb" class="mb-4">
      <router-link to="/" class="text-decoration-none text-muted small">← К списку тиражей</router-link>
    </nav>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="draw" class="row g-5">
      <div class="col-lg-8">
        <div class="card border-0 shadow-sm rounded-4 p-4">
          <h2 class="fw-bold mb-4">Выберите числа</h2>

          <div class="d-flex flex-wrap gap-2 mb-4">
            <button
              v-for="n in maxRange" :key="n"
              @click="toggleNumber(n)"
              :class="[
                'btn btn-number',
                selectedNumbers.includes(n) ? 'btn-primary shadow' : 'btn-outline-secondary'
              ]"
            >
              {{ n }}
            </button>
          </div>

          <div class="d-flex gap-3">
            <button @click="handleLuckyPick" class="btn btn-outline-primary py-3 px-4 rounded-3 flex-grow-1">
              ✨ Мне повезёт! (Рандом)
            </button>
            <button @click="selectedNumbers = []" class="btn btn-link text-muted">Очистить</button>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card border-0 shadow-sm rounded-4 p-4 bg-light h-100">
          <h4 class="fw-bold mb-3">{{ draw.lotteryName }}</h4>
          <hr>
          <div class="mb-3 d-flex justify-content-between">
            <span class="text-muted">Тираж:</span>
            <span class="fw-bold">#{{ draw.id }}</span>
          </div>
          <div class="mb-3 d-flex justify-content-between">
            <span class="text-muted">Выбрано чисел:</span>
            <span class="fw-bold">{{ selectedNumbers.length }}</span>
          </div>
          <div class="mb-4 d-flex justify-content-between align-items-center">
            <span class="text-muted">К оплате:</span>
            <span class="h3 fw-bold text-primary mb-0">{{ draw.ticketPrice }} ₽</span>
          </div>

          <button
            @click="buyTicket"
            :disabled="buying"
            class="btn btn-dark w-100 py-3 rounded-3 fw-bold mb-3"
          >
            <span v-if="buying" class="spinner-border spinner-border-sm me-2"></span>
            Оплатить и купить
          </button>

          <p class="text-center small text-muted">
            После покупки билет появится в вашем личном кабинете.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { apiRequest } from '@/api/client';

const route = useRoute();
const router = useRouter();
const draw = ref(null);
const loading = ref(true);
const buying = ref(false);
const selectedNumbers = ref([]);

// Для демо: определяем диапазон.
// В идеале эти данные должны приходить из draw.lotteryConfig
const maxRange = ref(45);

const fetchDrawDetails = async () => {
  try {
    // В реальном API лучше иметь GET /api/draws/{id}
    // Если его нет, найдем в общем списке активных для демо
    const activeDraws = await apiRequest('/draws/active');
    draw.value = activeDraws.find(d => d.id == route.params.id);

    if (draw.value?.lotteryName.includes('Бинго')) {
      maxRange.value = 90;
    }
  } catch (error) {
    console.error(error);
  } finally {
    loading.value = false;
  }
};

const toggleNumber = (n) => {
  const index = selectedNumbers.value.indexOf(n);
  if (index > -1) {
    selectedNumbers.value.splice(index, 1);
  } else {
    selectedNumbers.value.push(n);
  }
};

const handleLuckyPick = async () => {
  try {
    // Твой новый эндпоинт!
    const numbers = await apiRequest(`/tickets/${draw.value.id}/random`);
    selectedNumbers.value = numbers;
  } catch (error) {
    alert("Ошибка генерации: " + error.message);
  }
};

const buyTicket = async () => {
  buying.value = true;
  try {
    await apiRequest('/tickets/buy', 'POST', {
      drawId: draw.value.id,
      chosenNumbers: selectedNumbers.value
    });
    alert("Билет успешно куплен!");
    router.push('/profile'); // Уходим в профиль смотреть билеты
  } catch (error) {
    alert(error.message);
  } finally {
    buying.value = false;
  }
};

onMounted(fetchDrawDetails);
</script>

<style scoped>
.btn-number {
  width: 45px;
  height: 45px;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 12px;
  font-weight: bold;
}
.rounded-4 { border-radius: 1.25rem !important; }
</style>