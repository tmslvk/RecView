import { createApp } from 'vue/dist/vue.esm-bundler'
import axios from 'axios'
import router from './router'
import App from './App.vue'
import {store} from './storage.js'

createApp(App).use(router).use(store).mount('#app')