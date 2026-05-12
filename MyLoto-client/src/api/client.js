import { useAuthStore } from '@/stores/auth';

const BASE_URL = 'http://localhost:5115/api';

export async function apiRequest(endpoint, method = 'GET', body = null) {
  const auth = useAuthStore();
  const settings = {
    method,
    headers: { 'Content-Type': 'application/json' }
  };

  if (auth.token) {
    settings.headers['Authorization'] = `Bearer ${auth.token}`;
  }

  if (body) {
    settings.body = JSON.stringify(body);
  }

  const response = await fetch(`${BASE_URL}${endpoint}`, settings);

  // Читаем ответ как текст, а не сразу как JSON
  const text = await response.text();
  let result = null;

  if (text) {
    try {
      result = JSON.parse(text);
    } catch (e) {
      console.error("Сервер вернул не JSON формат:", text);
      throw new Error("Ошибка формата данных на сервере");
    }
  }

  // Если статус не успешный (не 2xx)
  if (!response.ok) {
    // Пытаемся достать описание ошибки из нашего Result<T> или берем статус
    const errorDescription = result?.error?.description || result?.message || `Ошибка сервера (${response.status})`;
    throw new Error(errorDescription);
  }

  // --- ЛОГИКА ПРОВЕРКИ ФОРМАТА ТВОЕГО BACKEND ---

  // 1. Если бэкенд вернул просто массив (как в твоем случае с рандомом)
  if (Array.isArray(result)) {
    return result;
  }

  // 2. Если бэкенд вернул объект Result<T>
  if (result && typeof result === 'object' && 'isSuccess' in result) {
    if (!result.isSuccess) {
      throw new Error(result.error?.description || 'Ошибка запроса');
    }
    // Если всё ок, возвращаем value (там будет наш List<int>)
    return result.value;
  }

  return result;
}