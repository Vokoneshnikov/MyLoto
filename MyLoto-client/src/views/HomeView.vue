<template>
  <div class="container py-5">
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-5">
      <div>
        <h1 class="display-5 fw-bold text-dark mb-1">Активные тиражи</h1>
        <p class="lead text-muted">Выберите игру и испытайте свою удачу</p>
      </div>
      <div class="mt-3 mt-md-0">
        <span class="badge rounded-pill bg-success-subtle text-success border border-success px-3 py-2">
          {{ draws.length }} игр доступно
        </span>
      </div>
    </div>

    <div v-if="loading" class="row g-4">
      <div v-for="i in 3" :key="i" class="col-12 col-md-6 col-lg-4">
        <div class="card border-0 shadow-sm placeholder-glow" style="height: 350px;">
          <div class="placeholder col-12 h-100 rounded-4"></div>
        </div>
      </div>
    </div>

    <div v-else class="row g-4">
      <div v-for="draw in draws" :key="draw.id" class="col-12 col-md-6 col-lg-4">
        <div class="card h-100 border-0 shadow-sm hover-card rounded-4 overflow-hidden">
          <div :class="isBingo(draw.lotteryName) ? 'bg-primary' : 'bg-warning'" style="height: 6px;"></div>

          <div class="card-body p-4 d-flex flex-column">
            <div class="d-flex justify-content-between align-items-start mb-3">
              <div>
                <h6 class="text-uppercase text-muted fw-bold small mb-1">
                  {{ isBingo(draw.lotteryName) ? 'Бинго' : 'Тиражная' }}
                </h6>
                <h3 class="card-title h4 fw-bold mb-0">{{ draw.lotteryName }}</h3>
              </div>
              <div class="badge bg-light text-primary rounded-3 p-2 border">
                <span class="d-block small text-muted fw-normal">Билет</span>
                <span class="fw-bold">{{ draw.ticketPrice }} ₽</span>
              </div>
            </div>

            <div class="rounded-4 p-3 mb-4 text-center"
                 :class="isBingo(draw.lotteryName) ? 'bg-primary-subtle' : 'bg-warning-subtle'">
              <span class="d-block small fw-bold text-uppercase opacity-75">Джекпот</span>
              <div class="h2 fw-bold mb-0 text-dark">{{ formatCurrency(draw.jackpot) }}</div>
            </div>

            <div class="d-flex align-items-center text-muted small mb-4">
              <i class="bi bi-clock me-2"></i>
              <span>До конца продаж: {{ formatDate(draw.salesEndTime) }}</span>
            </div>

            <router-link
              :to="{ name: 'DrawDetails', params: { id: draw.id } }"
              class="btn btn-dark w-100 py-3 rounded-3 fw-bold mt-auto"
            >
              Участвовать
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { apiRequest } from '@/api/client';

const draws = ref([]);
const loading = ref(true);

const isBingo = (name) => name?.toLowerCase().includes('бинго');

const fetchDraws = async () => {
  try {
    draws.value = await apiRequest('/draws/active');
  } catch (error) {
    console.error("Ошибка загрузки:", error);
  } finally {
    loading.value = false;
  }
};

const formatCurrency = (val) => new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB', maximumFractionDigits: 0 }).format(val);
const formatDate = (date) => new Date(date).toLocaleString('ru-RU', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' });

onMounted(fetchDraws);
</script>