<template>
  <div class="draw-live-container">
    <h2>Трансляция тиража #{{ drawId }}</h2>

    <div class="status-indicator" :class="status.toLowerCase()">
      <span v-if="status === 'Pending'">⏳ Ожидание старта...</span>
      <span v-else-if="status === 'InProgress'">🔥 Трансляция идет</span>
      <span v-else-if="status === 'Completed'">✅ Тираж завершен</span>
    </div>

    <div class="balls-container">
      <transition-group name="ball">
        <div
          v-for="(number, index) in drawnNumbers"
          :key="`${drawId}-${number}-${index}`"
          class="lottery-ball"
        >
          {{ number }}
        </div>
      </transition-group>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { useRoute } from 'vue-router';
import * as signalR from '@microsoft/signalr';

import { apiRequest } from '@/api/client';
import { useAuthStore } from '@/stores/auth';

const route = useRoute();
const auth = useAuthStore();

// Гарантируем, что ID тиража всегда является числом
const drawId = Number(route.params.id);

const status = ref('Pending');
const drawnNumbers = ref([]);
let connection = null;

// 1. Загрузка статуса для "опоздавших"
const loadInitialStatus = async () => {
  try {
    const response = await apiRequest(`/draws/${drawId}/live-status`, 'GET');
    status.value = response.status;
    drawnNumbers.value = response.drawnNumbers || [];
  } catch (error) {
    console.error('Ошибка при загрузке статуса тиража:', error.message);
  }
};

// 2. Подключение к SignalR
const connectToSignalR = async () => {
  connection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7162/hubs/draw', {
      accessTokenFactory: () => auth.token
    })
    .withAutomaticReconnect()
    .build();

  connection.on('ReceiveNumber', (payload) => {
    console.log('--- ПОЛУЧЕН СИГНАЛ ---');
    console.log('Данные от бэкенда (сырые):', payload);

    const incomingNumber = payload.Number || payload.number;

    if (incomingNumber) {
      // ИСПРАВЛЕНО: Защита от дублирования шаров на стыке HTTP и WebSockets
      if (!drawnNumbers.value.includes(incomingNumber)) {
        console.log(`Добавляем шар №${incomingNumber} на экран`);
        drawnNumbers.value.push(incomingNumber);
      }
      status.value = 'InProgress';
    } else {
      console.error('Пришел сигнал "ReceiveNumber", но в нем нет числа!', payload);
    }
  });

  try {
    await connection.start();
    console.log('Подключено к SignalR хабу');

    // Передаем уже приведенное к числу значение drawId
    await connection.invoke('JoinDrawGroup', drawId);
  } catch (error) {
    console.error('Ошибка подключения к SignalR:', error);
  }
};

onMounted(async () => {
  await loadInitialStatus();
  // Если тираж еще не завершен, открываем сокет (для состояний Pending и InProgress)
  if (status.value !== 'Completed') {
    await connectToSignalR();
  }
});

onBeforeUnmount(() => {
  if (connection) {
    // Безопасное отключение: предотвращает утечки памяти
    connection.invoke('LeaveDrawGroup', drawId).catch(() => {});
    connection.stop();
  }
});
</script>

<style scoped>
.draw-live-container {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  padding: 20px;
}

.status-indicator {
  margin-bottom: 20px;
  font-weight: bold;
  font-size: 1.2rem;
}

.status-indicator.pending { color: #f39c12; }
.status-indicator.inprogress { color: #e74c3c; }
.status-indicator.completed { color: #27ae60; }

.balls-container {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  justify-content: center;
  min-height: 100px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 12px;
}

.lottery-ball {
  width: 60px;
  height: 60px;
  background: radial-gradient(circle at 30% 30%, #ffffff, #e74c3c);
  color: white;
  font-size: 24px;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}

.ball-enter-active,
.ball-leave-active {
  transition: all 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.ball-enter-from {
  opacity: 0;
  transform: scale(0) translateY(-50px);
}
.ball-leave-to {
  opacity: 0;
  transform: scale(0);
}
</style>