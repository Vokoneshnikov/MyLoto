<template>
  <div v-if="profile" class="py-4">
    <div class="row">
      <div class="col-md-4">
        <div class="card shadow-sm border-0 mb-4 rounded-4">
          <div class="card-body text-center p-4">
            <div class="bg-primary text-white rounded-circle d-inline-flex align-items-center justify-content-center mb-3"
                 style="width: 80px; height: 80px; font-size: 2rem; font-weight: bold;">
              {{ profile.login[0].toUpperCase() }}
            </div>
            <h3 class="mb-0 fw-bold">{{ profile.firstName }} {{ profile.lastName }}</h3>
            <p class="text-muted small">@{{ profile.login }}</p>
            <div class="mt-2 py-1 px-3 bg-light rounded-pill d-inline-block border">
              <span class="small text-dark">📅 С нами с {{ formatDate(profile.joinedAt) }}</span>
            </div>
            <div class="mt-3">
              <router-link to="/profile/edit" class="btn btn-outline-secondary btn-sm rounded-pill px-3">
                <i class="bi bi-pencil me-1"></i> Редактировать
              </router-link>
            </div>
          </div>
        </div>

        <div class="card shadow-sm border-0 rounded-4">
          <div class="card-body p-4">
            <h6 class="text-uppercase text-muted small fw-bold mb-3">Детали аккаунта</h6>
            <div class="mb-3">
              <label class="text-muted small d-block">Email</label>
              <span class="fw-medium">{{ profile.email || '—' }}</span>
            </div>
            <div>
              <label class="text-muted small d-block">ID Пользователя</label>
              <span class="font-monospace text-muted small">#{{ profile.id }}</span>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-8">
        <div class="row g-3 mb-4">
          <div class="col-md-6">
            <div class="card shadow-sm border-0 h-100 bg-primary text-white rounded-4 overflow-hidden">
              <div class="card-body p-4 position-relative">
                <h6 class="text-white-50 text-uppercase small fw-bold">Кошелек</h6>
                <h2 class="display-6 fw-bold mb-0">{{ profile.balance.toLocaleString() }} ₽</h2>
                <router-link to="/profile/deposit" class="btn btn-light btn-sm rounded-pill px-3 fw-bold mt-2">
                  <i class="bi bi-plus-circle me-1"></i> Пополнить
                </router-link>
                <div class="position-absolute end-0 bottom-0 p-3 opacity-25">
                  <span style="font-size: 3rem;">💰</span>
                </div>
              </div>
            </div>
          </div>
          <div class="col-md-6">
            <div class="card shadow-sm border-0 h-100 rounded-4">
              <div class="card-body p-4">
                <h6 class="text-muted text-uppercase small fw-bold">Всего билетов</h6>
                <h2 class="display-6 fw-bold mb-0">{{ profile.totalTicketsCount || 0 }}</h2>
              </div>
            </div>
          </div>
        </div>

        <div class="card shadow-sm border-0 rounded-4 mb-4">
          <div class="card-header bg-white py-3 border-0 rounded-top-4">
            <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-center gap-3">
              <h5 class="mb-0 fw-bold">🎟️ Управление билетами</h5>

              <div class="btn-group p-1 bg-light rounded-pill border">
                <button @click="changeTab(false)"
                        :class="['btn btn-sm rounded-pill px-3', !isArchive ? 'bg-white text-dark shadow-sm fw-bold' : 'text-muted border-0 bg-transparent']">
                  Активные
                </button>
                <button @click="changeTab(true)"
                        :class="['btn btn-sm rounded-pill px-3', isArchive ? 'bg-white text-dark shadow-sm fw-bold' : 'text-muted border-0 bg-transparent']">
                  Архив билетов
                </button>
              </div>
            </div>

            <div v-if="isArchive" class="d-flex gap-2 mt-3 flex-wrap animate-fade-in">
              <button @click="changeArchiveFilter(null)"
                      :class="['btn btn-sm rounded-pill px-3', isWon === null ? 'btn-dark' : 'btn-light border']">
                Все в архиве
              </button>
              <button @click="changeArchiveFilter(true)"
                      :class="['btn btn-sm rounded-pill px-3', isWon === true ? 'btn-success text-white' : 'btn-light border']">
                🎉 Только выигрышные
              </button>
              <button @click="changeArchiveFilter(false)"
                      :class="['btn btn-sm rounded-pill px-3', isWon === false ? 'btn-secondary' : 'btn-light border']">
                Без выигрыша
              </button>
            </div>
          </div>

          <div class="card-body p-0">
            <div v-if="loadingTickets" class="text-center py-5">
              <div class="spinner-border text-primary spinner-border-sm me-2"></div>
              <span class="text-muted small">Обновляем список...</span>
            </div>

            <div v-else-if="tickets.length === 0" class="text-center py-5">
              <div class="mb-3 opacity-25" style="font-size: 3rem;">🎟️</div>
              <p class="text-muted">Билеты в данной категории не найдены.</p>
              <router-link v-if="!isArchive" to="/home" class="btn btn-outline-primary btn-sm rounded-pill px-4">
                Купить билет
              </router-link>
            </div>

            <div v-else class="list-group list-group-flush rounded-bottom-4">
              <div v-for="ticket in tickets" :key="ticket.ticketId" class="list-group-item p-4 border-bottom">
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3">
                  <div>
                    <div class="d-flex align-items-center flex-wrap gap-2 mb-2">
                      <span class="badge bg-dark rounded-pill">Билет #{{ ticket.ticketId }}</span>
                      <span class="text-muted small fw-medium">Тираж #{{ ticket.drawId }}</span>

                      <span v-if="!ticket.isChecked" class="badge bg-warning text-dark rounded-pill small">
                        ⏳ В игре ({{ ticket.drawStatus }})
                      </span>
                      <span v-else-if="ticket.winAmount > 0" class="badge bg-success text-white rounded-pill small">
                        🎉 Выигрыш: {{ ticket.winAmount.toLocaleString() }} ₽
                      </span>
                      <span v-else class="badge bg-light text-muted border rounded-pill small">
                        Проверен (Без выигрыша)
                      </span>
                    </div>

                    <div class="d-flex flex-wrap gap-1 mt-2">
                      <span v-for="num in ticket.selectedNumbers" :key="num"
                            :class="[
                              'badge rounded-circle d-flex align-items-center justify-content-center border transition-all',
                              ticket.drawWinningNumbers.includes(num)
                                ? 'bg-success text-white border-success shadow-sm scale-up'
                                : 'bg-light text-dark'
                            ]"
                            style="width: 38px; height: 38px; font-weight: bold; font-size: 0.95rem;">
                        {{ num }}
                      </span>
                    </div>
                  </div>

                  <div class="text-md-end d-flex gap-2 align-self-start align-self-md-center">
                    <router-link :to="'/tickets/' + ticket.ticketId" class="btn btn-light btn-sm rounded-pill px-3 border">
                      <i class="bi bi-eye"></i> Детали
                    </router-link>

                    <button v-if="!ticket.isChecked" @click="openGiftModal(ticket)" class="btn btn-outline-primary btn-sm rounded-pill px-3">
                      🎁 Подарить другу
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="modal-backdrop fade show"></div>
    <div v-if="showModal" class="modal fade show d-block" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg rounded-4">
          <div class="modal-header border-0">
            <h5 class="modal-title fw-bold">Подарить билет #{{ selectedTicket?.ticketId }}</h5>
            <button type="button" class="btn-close" @click="closeModal"></button>
          </div>
          <div class="modal-body">
            <p class="text-muted small mb-4">Введите логин счастливчика, которому вы хотите передать этот билет. После этого билет исчезнет из вашего профиля.</p>
            <div class="mb-3">
              <label class="form-label small fw-bold">Логин получателя</label>
              <input v-model="recipientLogin" type="text" class="form-control rounded-3" placeholder="Например: lucky_friend">
            </div>
          </div>
          <div class="modal-footer border-0">
            <button @click="closeModal" class="btn btn-light rounded-pill px-4">Отмена</button>
            <button @click="handleGift" :disabled="!recipientLogin || gifting" class="btn btn-primary rounded-pill px-4">
              <span v-if="gifting" class="spinner-border spinner-border-sm me-2"></span>
              Отправить подарок
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div v-else class="d-flex justify-content-center align-items-center" style="min-height: 400px;">
    <div class="spinner-border text-primary" role="status"></div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { apiRequest } from '@/api/client';

// Данные авторизованного профиля
const profile = ref(null);

// Параметры реактивной фильтрации билетов (Синхронизировано с бэкенд-валидатором!)
const tickets = ref([]);
const isArchive = ref(false);      // false = Активные, true = Архив
const isWon = ref(null);          // null = все, true = выиграли, false = проиграли
const loadingTickets = ref(false);

// Состояние модального окна подарка
const showModal = ref(false);
const selectedTicket = ref(null);
const recipientLogin = ref('');
const gifting = ref(false);

const formatDate = (dateString) => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
};

// Загрузка основной информации профиля
const fetchProfile = async () => {
  try {
    profile.value = await apiRequest('/profile/');
  } catch (e) {
    console.error("Ошибка загрузки профиля:", e.message);
  }
};

// Динамическая загрузка билетов с учетом выбранных фильтров
const fetchTickets = async () => {
  loadingTickets.value = true;
  try {
    // Формируем строку запроса к нашему обновленному Minimal API эндпоинту
    let queryPath = `/profile/tickets?isArchive=${isArchive.value}`;

    // Добавляем фильтр выигрыша только в том случае, если мы находимся во вкладке Архива
    if (isArchive.value && isWon.value !== null) {
      queryPath += `&isWon=${isWon.value}`;
    }

    tickets.value = await apiRequest(queryPath);
  } catch (e) {
    console.error("Ошибка фильтрации билетов:", e.message);
  } finally {
    loadingTickets.value = false;
  }
};

// Переключение между вкладками Активные / Архив
const changeTab = (archiveState) => {
  isArchive.value = archiveState;
  // Сбрасываем суб-фильтры выигрыша при смене глобальной вкладки, чтобы не нарушать правила валидатора
  isWon.value = null;
  fetchTickets();
};

// Смена фильтра выиграл/проиграл внутри архивной вкладки
const changeArchiveFilter = (wonState) => {
  isWon.value = wonState;
  fetchTickets();
};

const openGiftModal = (ticket) => {
  selectedTicket.value = ticket;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedTicket.value = null;
  recipientLogin.value = '';
};

const handleGift = async () => {
  gifting.value = true;
  try {
    await apiRequest(`/tickets/gift`, 'POST', {
      ticketId: selectedTicket.value.ticketId,
      recipientLogin: recipientLogin.value
    });

    alert(`Билет успешно отправлен пользователю ${recipientLogin.value}!`);
    closeModal();

    // Перезапрашиваем данные, чтобы актуализировать списки
    await fetchProfile();
    await fetchTickets();
  } catch (e) {
    alert("Ошибка: " + e.message);
  } finally {
    gifting.value = false;
  }
};

onMounted(() => {
  fetchProfile();
  fetchTickets(); // Запускаем первичный сбор активных билетов
});
</script>

<style scoped>
.rounded-4 { border-radius: 1rem !important; }
.rounded-top-4 { border-top-left-radius: 1rem !important; border-top-right-radius: 1rem !important; }
.rounded-bottom-4 { border-bottom-left-radius: 1rem !important; border-bottom-right-radius: 1rem !important; }
.modal-backdrop { z-index: 1040; }
.modal { z-index: 1050; }
.transition-all { transition: all 0.2s ease-in-out; }

/* Эффект легкого увеличения для угаданных бочонков */
.scale-up {
  transform: scale(1.08);
}

.animate-fade-in {
  animation: fadeIn 0.3s ease-in-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-5px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>