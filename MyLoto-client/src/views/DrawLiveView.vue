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

// Получаем ID тиража из URL (например, /draws/5/live)
const drawId = route.params.id;

const status = ref('Pending');
const drawnNumbers = ref([]);
let connection = null;

// 1. Загрузка статуса для "опоздавших"
const loadInitialStatus = async () => {
  try {
    const response = await apiRequest(`/draws/${drawId}/live-status`, 'GET');
    // Благодаря твоему apiRequest, здесь response — это сразу объект с Value
    status.value = response.status;
    drawnNumbers.value = response.drawnNumbers || [];
  } catch (error) {
    console.error('Ошибка при загрузке статуса тиража:', error.message);
  }
};

// 2. Подключение к SignalR
const connectToSignalR = async () => {
  // Обрати внимание на URL — он должен вести на твой хаб
  connection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7162/hubs/draw', {
      // Передаем токен на случай, если твой Hub закрыт атрибутом [Authorize]
      accessTokenFactory: () => auth.token
    })
    .withAutomaticReconnect()
    .build();

  // --- ИСПРАВЛЕННЫЙ БЛОК ПРИЕМА SignalR ---
  connection.on('ReceiveNumber', (payload) => {
    console.log('--- ПОЛУЧЕН СИГНАЛ ---');
    console.log('Данные от бэкенда (сырые):', payload);

    // Достаем число. SignalR JS клиент обычно делает camelCase!
    // Проверяем payload.number (с маленькой буквы)
    const incomingNumber = payload.Number || payload.number;

    if (incomingNumber) {
      console.log(`Добавляем шар №${incomingNumber} на экран`);
      drawnNumbers.value.push(incomingNumber);
      status.value = 'InProgress';
    } else {
      console.error('Пришел сигнал "ReceiveNumber", но в нем нет числа!', payload);
    }
  });

  try {
    await connection.start();
    console.log('Подключено к SignalR хабу');

    // Добавляемся в группу конкретного тиража
    await connection.invoke('JoinDrawGroup', Number(route.params.id));
  } catch (error) {
    console.error('Ошибка подключения к SignalR:', error);
  }
};

// Жизненный цикл: при входе на страницу
onMounted(async () => {
  await loadInitialStatus();
  // Если тираж еще не завершен, открываем сокет
  if (status.value !== 'Completed') {
    await connectToSignalR();
  }
});

// Жизненный цикл: при уходе со страницы
onBeforeUnmount(() => {
  if (connection) {
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

/* Анимация появления бочонка (Vue Transition) */
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