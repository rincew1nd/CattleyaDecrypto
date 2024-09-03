import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import { router } from './router'

import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';
import Toast from "primevue/toast";
import ToastService from "primevue/toastservice";

const app = createApp(App);

app.use(router);

app.use(PrimeVue, {
    theme: {
        preset: Aura
    }
});

app.component("Toast", Toast);
app.use(ToastService);

app.mount('#app')