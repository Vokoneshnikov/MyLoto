import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    user: JSON.parse(localStorage.getItem('user')) || null
  }),
  getters: {
    isLoggedIn: (state) => !!state.token,
    balance: (state) => state.user?.balance || 0,

    // Геттер читает поле role, которое мы теперь сохраняем
    isAdmin: (state) => state.user?.role === 'Moderator'
  },
  actions: {
    setAuth(authData) {
      this.token = authData.token

      // ИСПРАВЛЕНИЕ: Добавляем authData.role в объект user
      this.user = {
        login: authData.login,
        balance: authData.balance,
        role: authData.role // <-- Вот этого поля не хватало!
      }

      localStorage.setItem('token', authData.token)
      localStorage.setItem('user', JSON.stringify(this.user))
    },
    logout() {
      this.token = null
      this.user = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    }
  }
})