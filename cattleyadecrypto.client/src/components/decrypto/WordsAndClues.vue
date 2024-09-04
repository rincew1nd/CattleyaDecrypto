<script setup lang="ts">
import { type Team } from '../types/DecryptoTypes'
const { teamInfo } = defineProps<{ teamInfo: Team }>();
</script>

<template>
  <div class="words block-border">
    <div class="block br bl" v-for="(clues, key) in teamInfo.words">
      <span class="header p-10">{{clues}}</span>
      <hr/>
      <div v-for="clue in teamInfo.clues[key]">
        <p class="p-10">{{clue}}</p>
        <hr/>
      </div>
    </div>
  </div>
</template>

<style scoped>
.words {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
  text-align: center;
}
.p-10 {
  padding: 0.5rem;
}
</style>