<template>
  <div class="container py-5">
    <nav aria-label="breadcrumb" class="mb-4">
      <router-link to="/home" class="text-decoration-none text-muted small">← К списку тиражей</router-link>
    </nav>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="draw" class="row g-5">
      <div class="col-lg-8">
        <div class="card border-0 shadow-sm rounded-4 p-4">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="fw-bold mb-0">Выберите числа</h2>
            <span :class="['badge rounded-pill px-3 py-2', isLimitReached ? 'bg-success' : 'bg-warning text-dark']">
              {{ statusMessage }}
            </span>
          </div>

          <div class="d-flex flex-wrap gap-2 mb-4">
            <button
              v-for="n in maxRange" :key="n"
              @click="toggleNumber(n)"
              :disabled="isLimitReached && !selectedNumbers.includes(n)"
              :class="[
                'btn btn-number',
                selectedNumbers.includes(n) ? 'btn-primary shadow' : '',
                !selectedNumbers.includes(n) && isLimitReached ? 'btn-light text-muted opacity-50' : 'btn-outline-secondary'
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
            <span class="text-muted">Выбрано:</span>
            <span :class="['fw-bold', selectedNumbers.length === requiredCount ? 'text-success' : 'text-danger']">
              {{ selectedNumbers.length }} из {{ requiredCount }}
            </span>
          </div>
          <div class="mb-4 d-flex justify-content-between align-items-center">
            <span class="text-muted">К оплате:</span>
            <span class="h3 fw-bold text-primary mb-0">{{ draw.ticketPrice }} ₽</span>
          </div>

          <button
            @click="buyTicket"
            :disabled="buying || selectedNumbers.length !== requiredCount"
            class="btn btn-dark w-100 py-3 rounded-3 fw-bold mb-3"
          >
            <span v-if="buying" class="spinner-border spinner-border-sm me-2"></span>
            {{ selectedNumbers.length === requiredCount ? 'Оплатить и купить' : `Нужно еще ${requiredCount - selectedNumbers.length}` }}
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
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { apiRequest } from '@/api/client';
import Swal from 'sweetalert2'; // Подключаем красивые уведомления

const route = useRoute();
const router = useRouter();
const draw = ref(null);
const loading = ref(true);
const buying = ref(false);
const selectedNumbers = ref([]);

const maxRange = ref(0);
const requiredCount = ref(0);

const isLimitReached = computed(() => selectedNumbers.value.length >= requiredCount.value);

const statusMessage = computed(() => {
  const diff = requiredCount.value - selectedNumbers.value.length;
  if (diff > 0) return `Выберите еще ${diff} ${getNoun(diff, 'число', 'числа', 'чисел')}`;
  return 'Комбинация готова!';
});

const fetchDrawDetails = async () => {
  try {
    draw.value = await apiRequest(`/draws/${route.params.id}`);

    if (draw.value.lotteryType === 'Bingo') {
      maxRange.value = draw.value.maxBallValue;
      requiredCount.value = draw.value.rows * draw.value.columns;
    } else if (draw.value.lotteryType === 'KOutOfN') {
      maxRange.value = draw.value.maxNumber;
      requiredCount.value = draw.value.numbersToChoose;
    }
  } catch (error) {
    console.error(error);
    Swal.fire({ icon: 'error', title: 'Ошибка', text: 'Ошибка загрузки тиража: ' + error.message });
  } finally {
    loading.value = false;
  }
};

const toggleNumber = (n) => {
  const index = selectedNumbers.value.indexOf(n);
  if (index > -1) {
    selectedNumbers.value.splice(index, 1);
  } else if (!isLimitReached.value) {
    selectedNumbers.value.push(n);
  }
};

const handleLuckyPick = async () => {
  try {
    const numbers = await apiRequest(`/tickets/${draw.value.id}/random`);
    selectedNumbers.value = numbers;
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Упс!', text: 'Ошибка генерации: ' + error.message });
  }
};

const buyTicket = async () => {
  // Финальный рубеж защиты: проверяем массив перед отправкой
  if (selectedNumbers.value.length !== requiredCount.value) {
    Swal.fire({
      icon: 'warning',
      title: 'Неполная комбинация',
      text: `Пожалуйста, выберите ровно ${requiredCount.value} чисел перед оплатой.`
    });
    return;
  }

  buying.value = true;
  try {
    await apiRequest('/tickets/buy', 'POST', {
      drawId: draw.value.id,
      chosenNumbers: selectedNumbers.value
    });

    await Swal.fire({
      icon: 'success',
      title: 'Успешно!',
      text: 'Билет успешно куплен и добавлен в ваш личный кабинет.',
      confirmButtonColor: '#212529',
      timer: 3000
    });

    router.push('/home');
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Ошибка оплаты', text: error.message });
  } finally {
    buying.value = false;
  }
};

function getNoun(number, one, two, five) {
  let n = Math.abs(number);
  n %= 100;
  if (n >= 5 && n <= 20) return five;
  n %= 10;
  if (n === 1) return one;
  if (n >= 2 && n <= 4) return two;
  return five;
}

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
  transition: all 0.2s ease;
}

.btn-number:disabled {
  cursor: not-allowed;
}

.rounded-4 { border-radius: 1.25rem !important; }

.btn-primary.shadow {
  transform: scale(1.1);
}
</style>