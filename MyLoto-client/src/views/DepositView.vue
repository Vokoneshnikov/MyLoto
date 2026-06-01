<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-6 col-lg-5">
        <div class="card border-0 shadow-sm rounded-4 overflow-hidden">
          <div class="card-header bg-dark text-white p-4 text-center">
            <h4 class="fw-bold mb-0">Пополнение баланса</h4>
          </div>

          <div class="card-body p-4 p-md-5">
            <label class="form-label small fw-bold text-muted text-uppercase">Выберите сумму</label>

            <div class="d-flex flex-wrap gap-2 mb-4">
              <button
                v-for="amount in presets"
                :key="amount"
                @click="selectPreset(amount)"
                :class="['btn rounded-3 flex-grow-1 py-2 fw-bold', selectedAmount === amount ? 'btn-primary' : 'btn-outline-primary']"
              >
                {{ amount }} ₽
              </button>
            </div>

            <div class="mb-4">
              <label class="form-label small fw-bold text-muted text-uppercase">Или введите свою</label>
              <div class="input-group input-group-lg has-validation">
                <span :class="['input-group-text bg-white border-end-0 text-muted', errors.amount ? 'border-danger' : '']">₽</span>
                <input
                  v-model.number="selectedAmount"
                  type="number"
                  :class="['form-control border-start-0 ps-0', errors.amount ? 'is-invalid' : '']"
                  placeholder="0.00"
                  @input="clearError"
                >
                <div v-if="errors.amount" class="invalid-feedback d-block">{{ errors.amount }}</div>
              </div>
            </div>

            <div class="mb-4">
              <label class="form-label small fw-bold text-muted text-uppercase mb-3">Способ оплаты</label>
              <div class="list-group">
                <label class="list-group-item d-flex align-items-center gap-3 rounded-3 mb-2 border pointer">
                  <input class="form-check-input flex-shrink-0" type="radio" name="payMethod" checked>
                  <span class="d-flex align-items-center gap-2">
                    <i class="bi bi-credit-card fs-4"></i>
                    Банковская карта (Stripe)
                  </span>
                </label>
                <label class="list-group-item d-flex align-items-center gap-3 rounded-3 opacity-50 border pointer">
                  <input class="form-check-input flex-shrink-0" type="radio" name="payMethod" disabled>
                  <span class="d-flex align-items-center gap-2">
                    <i class="bi bi-wallet2 fs-4"></i>
                    Криптовалюта (Скоро)
                  </span>
                </label>
              </div>
            </div>

            <button
              @click="handleDeposit"
              :disabled="loading"
              class="btn btn-dark btn-lg w-100 rounded-pill fw-bold py-3 mt-2 shadow"
            >
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              Пополнить на {{ selectedAmount || 0 }} ₽
            </button>

            <router-link to="/profile" class="btn btn-link w-100 text-muted mt-3 text-decoration-none small">
              Отмена
            </router-link>
          </div>
        </div>

        <div class="text-center mt-4 text-muted small">
          <i class="bi bi-shield-lock me-1"></i> Все транзакции защищены шифрованием SSL
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { apiRequest } from '@/api/client';
import Swal from 'sweetalert2';

const router = useRouter();
const loading = ref(false);
const selectedAmount = ref(500);
const presets = [100, 500, 1000, 5000];

// Локальный стейт ошибок
const errors = reactive({
  amount: ''
});

const selectPreset = (amount) => {
  selectedAmount.value = amount;
  errors.amount = '';
};

const clearError = () => {
  errors.amount = '';
};

// Финансовый валидатор
const validateForm = () => {
  errors.amount = '';
  const amt = selectedAmount.value;

  if (amt === '' || amt === null || amt === undefined) {
    errors.amount = 'Укажите сумму пополнения';
    return false;
  }
  if (isNaN(amt) || amt <= 0) {
    errors.amount = 'Сумма должна быть строго больше нуля';
    return false;
  }
  if (amt < 50) {
    errors.amount = 'Минимальная сумма пополнения — 50 ₽ (ограничение Stripe)';
    return false;
  }
  if (amt > 150000) {
    errors.amount = 'Максимальная сумма разового пополнения — 150 000 ₽';
    return false;
  }

  // Проверка на "копеечный спам": не более 2 знаков после запятой
  if (amt.toString().includes('.')) {
    const decimalPlaces = amt.toString().split('.')[1].length;
    if (decimalPlaces > 2) {
      errors.amount = 'Сумма не может содержать более 2 знаков после запятой';
      return false;
    }
  }

  return true;
};

const handleDeposit = async () => {
  if (!validateForm()) return; // Защитный гвард

  loading.value = true;
  try {
    const baseUrl = window.location.origin;

    const response = await apiRequest('/profile/deposit', 'POST', {
      Amount: selectedAmount.value,
      SuccessUrl: `${baseUrl}/profile?payment=success`,
      CancelUrl: `${baseUrl}/profile/deposit`
    });

    // Редирект на защищенный шлюз Stripe
    if (response && response.url) {
      window.location.href = response.url;
    } else {
      window.location.href = response;
    }

  } catch (e) {
    console.error(e);
    Swal.fire({
      icon: 'error',
      title: 'Ошибка платежной сессии',
      text: e.message || 'Не удалось связаться со Stripe. Попробуйте позже.'
    });
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.pointer { cursor: pointer; }
.rounded-4 { border-radius: 1.25rem !important; }
.input-group-text { border-radius: 0.75rem 0 0 0.75rem !important; }
.form-control { border-radius: 0 0.75rem 0.75rem 0 !important; }

/* Красивая подсветка левой иконки при ошибке */
.border-danger {
  border-color: #dc3545 !important;
}
</style>