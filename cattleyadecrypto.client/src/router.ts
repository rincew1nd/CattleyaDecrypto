import { createMemoryHistory, createRouter } from 'vue-router'

import MainPage from './components/MainPage.vue'
import MainBoard from './components/decrypto/MainBoard.vue'
import SignalRExample from './components/SignalRExample.vue'

const routes = [
    { path: '/', component: MainPage },
    { path: '/decrypto/:id', component: MainBoard },
    { path: '/test', component: SignalRExample }
]

export const router = createRouter({
    history: createMemoryHistory(),
    routes,
})