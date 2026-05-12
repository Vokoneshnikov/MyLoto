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
                <div class="position-absolute end-0 bottom-0 p-3 opacity-25">
                  <span style="font-size: 3rem;">💰</span>
                </div>
              </div>
            </div>
          </div>
          <div class="col-md-6">
            <div class="card shadow-sm border-0 h-100 rounded-4">
              <div class="card-body p-4">
                <h6 class="text-muted text-uppercase small fw-bold">Активные билеты</h6>
                <h2 class="display-6 fw-bold mb-0">{{ profile.totalTicketsCount || 0 }}</h2>
              </div>
            </div>
          </div>
        </div>

        <div class="card shadow-sm border-0 rounded-4">
          <div class="card-header bg-white py-3 d-flex justify-content-between align-items-center">
            <h5 class="mb-0 fw-bold">Мои билеты</h5>
            <span class="badge bg-light text-dark border">Всего: {{ profile.totalTicketsCount || 0 }}</span>
          </div>
          <div class="card-body p-0">
            <div v-if="!profile.totalTicketsCount" class="text-center py-5">
              <div class="mb-3 opacity-25" style="font-size: 3rem;">🎟️</div>
              <p class="text-muted">У вас пока нет купленныкупитьх билетов.</p>
              <router-link to="/home" class="btn btn-outline-primary btn-sm rounded-pill px-4">Купить первый билет</router-link>
            </div>

            <div v-else class="list-group list-group-flush">
              <div v-for="ticket in tickets" :key="ticket.ticketId" class="list-group-item p-4 border-bottom-0">
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3">
                  <div>
                    <div class="d-flex align-items-center gap-2 mb-2">
                      <span class="badge bg-dark rounded-pill">Билет #{{ ticket.ticketId }}</span>
                      <span class="text-muted small">Тираж: {{ ticket.lotteryName }}</span>
                    </div>
                    <div class="d-flex flex-wrap gap-1">
                      <span v-for="num in ticket.chosenNumbers" :key="num"
                            class="badge rounded-circle bg-light text-dark border d-flex align-items-center justify-content-center"
                            style="width: 35px; height: 35px; font-weight: bold;">
                        {{ num }}
                      </span>
                    </div>
                  </div>
                  <div class="text-md-end">
                    <button @click="openGiftModal(ticket)" class="btn btn-outline-primary btn-sm rounded-pill px-3">
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
            <h5 class="modal-title fw-bold">Подарить билет #{{ selectedTicket?.id }}</h5>
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

const profile = ref(null);
const tickets = ref([]);
const showModal = ref(false);
const selectedTicket = ref(null);
const recipientLogin = ref('');
const gifting = ref(false);

const formatDate = (dateString) => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
};

const fetchProfile = async () => {
  try {
    profile.value = await apiRequest('/profile/');
    tickets.value = await apiRequest('/profile/tickets');
    console.log("Данные профиля из API:", profile);
    console.log("Данные билетов из API:", tickets);
  } catch (e) {
    console.error("Ошибка загрузки:", e.message);
  }
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
    await fetchProfile(); // Обновляем профиль, чтобы билет исчез из списка
  } catch (e) {
    alert("Ошибка: " + e.message);
  } finally {
    gifting.value = false;
  }
};

onMounted(fetchProfile);
</script>

<style scoped>
.rounded-4 { border-radius: 1rem !important; }
.modal-backdrop { z-index: 1040; }
.modal { z-index: 1050; }
</style>