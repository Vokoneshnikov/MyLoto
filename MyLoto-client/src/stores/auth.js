import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    user: JSON.parse(localStorage.getItem('user')) || null
  }),
  getters: {
    isLoggedIn: (state) => !!state.token,
    balance: (state) => state.user?.balance || 0
  },
  actions: {
    setAuth(authData) {
      this.token = authData.token
      this.user = { login: authData.login, balance: authData.balance }
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