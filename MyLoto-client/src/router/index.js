import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ProfileView from '@/views/ProfileView.vue'
import EditProfileView from '@/views/EditProfileView.vue'
import TicketDetailsView from '@/views/TicketDetailsView.vue'
import DepositView from '@/views/DepositView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/home',
      name: 'home',
      component: HomeView
    },
    {
      path: '/draw/:id',
      name: 'DrawDetails',
      component: () => import('../views/DrawDetailsView.vue'),
      props: true // Позволяет принимать id как prop
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
    }
  ]
})

export default router