<script setup lang="ts">
import { AuthStorage } from './AuthStorage.js'
import Button from 'primevue/button'
import Toolbar from 'primevue/toolbar'
import {router} from "@/router";

function newGameButton() {
  router.push('decrypto/testasdf');
}
</script>

<template>
  <Toolbar>
    <template #start>
      <Button @click='newGameButton' label="New Game" text plain />
      <Button label="Join Game" text plain />
    </template>
    <template #end>
      <p class="nickname">{{ AuthStorage.name }}</p>
      <Button text plain>{{ AuthStorage.isLogin ? 'Logout' : 'Login' }}</Button>
    </template>
  </Toolbar>
</template>

<style scoped>
.nickname {
  min-width: 100px;
  text-align: center;
  font-size: 1rem;
}
</style>