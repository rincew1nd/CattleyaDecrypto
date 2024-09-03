<script setup lang="ts">
import { HubConnectionBuilder } from "@microsoft/signalr";

import { useToast } from 'primevue/usetoast';
const toast = useToast();

const connection = new HubConnectionBuilder().withUrl("/messageHub").build();

connection.on("ReceiveMessage", function (user, message) {
  toast.add({ severity: "success", summary: "PrimeVue", detail: message, life: 3000 });
});

connection.start().then(function () {
}).catch(function (err) {
  return console.error(err.toString());
});
</script>

<template>
  <Toast />
  <button>Test</button>
</template>