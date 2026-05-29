import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth' // Импортируем стор

import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ProfileView from '@/views/ProfileView.vue'
import EditProfileView from '@/views/EditProfileView.vue'
import TicketDetailsView from '@/views/TicketDetailsView.vue'
import DepositView from '@/views/DepositView.vue'
import CreateLotteryView from '@/views/CreateLotteryView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/admin/lotteries/create',
      name: 'CreateLottery',
      component: CreateLotteryView,
      // meta: { requiresAdmin: true } // Эта метка теперь будет работать
    },
    {
      path: '/home',
      name: 'home',
      component: HomeView
    },
    {
      path: '/draw/:id',
      name: 'DrawDetails',
      component: () => import('../views/DrawDetailsView.vue'),
      props: true
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView
    },
    {
      path: '/profile/edit',
      name: 'EditProfile',
      component: EditProfileView
    },
    {
      path: '/tickets/:id',
      name: 'TicketDetails',
      component: TicketDetailsView
    },
    {
      path: '/profile/deposit',
      name: 'Deposit',
      component: DepositView,
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfileView
    },
    {
      path: '/admin/lotteries',
      name: 'AdminLotteries',
      component: () => import('@/views/AdminLotteriesView.vue'),
      meta: { requiresAdmin: true }
    }
  ]
})

// --- ГЛОБАЛЬНЫЙ GUARD (ЗАЩИТНИК МАРШРУТОВ) ---
router.beforeEach((to, from, next) => {
  const auth = useAuthStore()

  // Если маршрут требует прав администратора
  if (to.meta.requiresAdmin) {
    // Проверяем: если не залогинен ИЛИ не админ
    if (!auth.isLoggedIn || !auth.isAdmin) {
      // Перенаправляем на главную (или можно на страницу 403 / логин)
      return next('/home')
    }
  }

  // Если всё ок, пропускаем дальше
  next()
})

export default router