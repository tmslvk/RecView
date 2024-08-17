import { createRouter, createWebHistory } from 'vue-router'
import LoginForm from './components/LoginForm.vue'
import MainPage from './components/MainPage.vue'
import SpotifyCallback from './components/SpotifyCallback.vue'
import RegistrationSpotifyAdditional from './components/RegistrationSpotifyAdditional.vue'
import Signup from './components/SignUp.vue'

const router = createRouter({
    linkActiveClass: 'is-active',
    history: createWebHistory(),
    routes: [
        {
            path: '/Login',
            name: 'Login',
            component: LoginForm,
        },
        {
            path: '/MainPage',
            name: 'MainPage',
            component: MainPage,
        },
        {
            path: '/callback',
            name: 'SpotifyCallback',
            component: SpotifyCallback
        },
        {
            path: '/register',
            name: 'RegistrationSpotify',
            component: RegistrationSpotifyAdditional
        },
        {
            path: '/Signup',
            name: 'Signup',
            component: Signup,
        }

    ]
})

router.beforeEach(async (to, from) => {
    if (
        !localStorage.getItem("token") &&
        to.name !== 'Login' && to.name !== 'SpotifyCallback' && to.name !== 'RegistrationSpotify' && to.name !== 'Signup'
    ) {
        return { name: 'Login' }
    }
})

export default router