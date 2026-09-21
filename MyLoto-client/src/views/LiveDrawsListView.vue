<template>
  <div class="container py-5">
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-5">
      <div>
        <h1 class="display-5 fw-bold text-dark mb-1">Прямые трансляции</h1>
        <p class="lead text-muted">Следите за выпадением чисел в реальном времени</p>
      </div>
      <div class="mt-3 mt-md-0">
        <span class="badge rounded-pill bg-danger-subtle text-danger border border-danger px-3 py-2 fw-bold animate-pulse">
          🔴 {{ liveDraws.length }} трансляций в эфире
        </span>
      </div>
    </div>

    <div v-if="loading" class="row g-4">
      <div v-for="i in 2" :key="i" class="col-12 col-md-6 col-lg-4">
        <div class="card border-0 shadow-sm placeholder-glow" style="height: 350px;">
          <div class="placeholder col-12 h-100 rounded-4"></div>
        </div>
      </div>
    </div>

    <div v-else class="row g-4">
      <div v-if="liveDraws.length === 0" class="col-12 text-center py-5">
        <div class="text-muted mb-3 display-3">📺</div>
        <h4 class="fw-bold text-secondary">В эфире пока затишье</h4>
        <p class="text-muted">Сейчас нет идущих розыгрышей. Вы можете приобрести билет на вкладке активных тиражей.</p>
        <router-link to="/home" class="btn btn-outline-dark px-4 py-2 rounded-3 mt-2">
          Перейти к покупке билетов
        </router-link>
      </div>

      <div v-for="draw in liveDraws" :key="draw.id" class="col-12 col-md-6 col-lg-4">
        <div class="card h-100 border-0 shadow-sm hover-card-live rounded-4 overflow-hidden position-relative border-danger-glow">

          <span class="badge bg-danger position-absolute top-0 end-0 m-3 px-3 py-2 fw-bold shadow-sm badge-live">
            ЭФИР 🔴
          </span>

          <div class="bg-danger" style="height: 6px;"></div>

          <div class="card-body p-4 d-flex flex-column">
            <div class="d-flex justify-content-between align-items-start mb-3">
              <div>
                <h6 class="text-uppercase text-danger fw-bold small mb-1 tracking-wider">
                  {{ isBingo(draw.lotteryName) ? 'Бинго лотерея' : 'Числовая лотерея' }}
                </h6>
                <h3 class="card-title h4 fw-bold mb-0 me-5 text-dark">{{ draw.lotteryName }}</h3>
              </div>
            </div>

            <div class="rounded-4 p-3 mb-4 text-center bg-danger-subtle-custom">
              <span class="d-block small fw-bold text-uppercase text-danger opacity-75">Разыгрываемый джекпот</span>
              <div class="h2 fw-bold mb-0 text-danger">{{ formatCurrency(draw.jackpot) }}</div>
            </div>

            <div class="d-flex align-items-center text-danger small fw-bold mb-4">
              <i class="bi bi-broadcast me-2 animate-ping"></i>
              <span>Шары выпадают прямо сейчас!</span>
            </div>

            <div class="mt-auto">
              <router-link
                :to="{ name: 'DrawLive', params: { id: draw.id } }"
                class="btn btn-danger w-100 py-3 rounded-3 fw-bold d-flex align-items-center justify-content-center shadow-sm btn-live-action"
              >
                <i class="bi bi-play-circle-fill me-2 fs-5"></i> Смотреть трансляцию
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { apiRequest } from '@/api/client';

const liveDraws = ref([]);
const loading = ref(true);

const isBingo = (name) => name?.toLowerCase().includes('бинго');

const fetchLiveDraws = async () => {
  try {
    liveDraws.value = await apiRequest('/draws/live');
  } catch (error) {
    console.error("Ошибка загрузки эфиров:", error);
  } finally {
    loading.value = false;
  }
};

const formatCurrency = (val) => new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB', maximumFractionDigits: 0 }).format(val);

onMounted(fetchLiveDraws);
</script>

<style scoped>
.bg-danger-subtle-custom {
  background-color: rgba(220, 53, 69, 0.08);
}
.border-danger-glow {
  border: 1px solid rgba(220, 53, 69, 0.15) !important;
}
.hover-card-live {
  transition: all 0.25s ease-in-out;
}
.hover-card-live:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 24px rgba(220, 53, 69, 0.12) !important;
  border-color: rgba(220, 53, 69, 0.4) !important;
}
.animate-pulse {
  animation: pulse 2s infinite;
}
@keyframes pulse {
  0% { opacity: 0.8; }
  50% { opacity: 1; transform: scale(1.02); }
  100% { opacity: 0.8; }
}
.badge-live {
  letter-spacing: 0.5px;
  animation: blinker 1.8s linear infinite;
}
@keyframes blinker {
  50% { opacity: 0.7; }
}
</style>