import { createRouter, createWebHistory } from 'vue-router'
import LoginForm from './components/LoginForm.vue'

const router = createRouter({
    linkActiveClass: 'is-active',
    history: createWebHistory(),
    routes: [
        {
            path: '/Login',
            name: 'Login',
            component: LoginForm,
        },
    ]
})

router.beforeEach(async (to, from) => {
    if (
        !localStorage.getItem("token") &&
        to.name !== 'Login' && to.name !== 'Registration'
    ) {
        return { name: 'Login' }
    }
})

export default router