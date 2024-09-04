<script setup lang="ts">
import DecryptoDataService from "@/components/services/DecryptoDataService";
import {router} from "@/router";

function createGame() {
  DecryptoDataService.createMatch()
      .then(game => {
        router.push(`/decrypto/${game.id}`);
      });
}
</script>

<template>
  <div class="content">
    <p>TODO Parameters</p>
    <button @click="createGame">Start new game</button>
  </div>
</template>

<style scoped>
.content {
  text-align: center;
}
</style>