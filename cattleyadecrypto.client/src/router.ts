import { createWebHistory, createRouter } from 'vue-router'

import About from "@/components/About.vue"
import Page404 from './components/Page404.vue'

import NewGameWindow from "@/components/decrypto/NewGameWindow.vue";
import MainBoard from './components/decrypto/MainBoard.vue'

const routes = [
    { path: '/', component: About },
    { path: '/decrypto/new', component: NewGameWindow },
    { path: '/decrypto/:id', component: MainBoard, props: true },
    { path: '/:pathMatch(.*)*', name: 'Not Found', component: Page404 },
]

export const router = createRouter({
    history: createWebHistory(),
    routes,
})