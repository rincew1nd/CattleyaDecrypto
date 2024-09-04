<script setup lang="ts">
import Button from 'primevue/button'
import Toolbar from 'primevue/toolbar'
import {router} from "@/router";

function newGameButton() {
  router.push('/decrypto/new');
}
</script>

<template>
  <Toolbar>
    <template #start>
      <Button @click='newGameButton' label="New Game" text plain />
    </template>
    <template v-if="$route.params.id" #center>
      <h2>Game: {{$route.params.id}}</h2>
    </template>
    <template #end>
      <p class="nickname">{{ 'TODO' }}</p>
      <Button text plain>{{ 'TODO' }}</Button>
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