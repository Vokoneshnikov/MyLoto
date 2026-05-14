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

            <!-- Пресеты сумм -->
            <div class="d-flex flex-wrap gap-2 mb-4">
              <button
                v-for="amount in presets"
                :key="amount"
                @click="selectedAmount = amount"
                :class="['btn rounded-3 flex-grow-1 py-2 fw-bold', selectedAmount === amount ? 'btn-primary' : 'btn-outline-primary']"
              >
                {{ amount }} ₽
              </button>
            </div>

            <!-- Поле ввода -->
            <div class="mb-4">
              <label class="form-label small fw-bold text-muted text-uppercase">Или введите свою</label>
              <div class="input-group input-group-lg">
                <span class="input-group-text bg-white border-end-0 text-muted">₽</span>
                <input
                  v-model.number="selectedAmount"
                  type="number"
                  class="form-control border-start-0 ps-0"
                  placeholder="0.00"
                >
              </div>
            </div>

            <!-- Заглушка методов оплаты -->
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
              :disabled="!selectedAmount || selectedAmount <= 0 || loading"
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
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { apiRequest } from '@/api/client';

const router = useRouter();
const loading = ref(false);
const selectedAmount = ref(500);
const presets = [100, 500, 1000, 5000];

const handleDeposit = async () => {
  loading.value = true;
  try {
    // 1. Формируем URL возврата (текущий адрес сайта)
    const baseUrl = window.location.origin;

    // 2. Отправляем все необходимые данные
    const response = await apiRequest('/profile/deposit', 'POST', {
      Amount: selectedAmount.value,
      SuccessUrl: `${baseUrl}/profile?payment=success`, // Куда вернуться при успехе
      CancelUrl: `${baseUrl}/profile/deposit`          // Куда вернуться при отмене
    });

    // 3. Важно: Бэкенд теперь возвращает URL сессии Stripe
    // Если в apiRequest ты возвращаешь чистое тело ответа:
    if (response && response.url) {
      // Перенаправляем пользователя на страницу оплаты Stripe
      window.location.href = response.url;
    } else {
      // Если бэкенд возвращает строку напрямую (зависит от твоего ToProcessResult)
      window.location.href = response;
    }

  } catch (e) {
    console.error(e);
    alert("Ошибка пополнения: " + e.message);
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
</style>