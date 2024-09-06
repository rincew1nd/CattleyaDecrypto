<script setup lang="ts">
import { DecryptoTeamEnum } from '../types/DecryptoTypes'
import { MatchInfo } from '../services/MatchManagerService'

const { team } = defineProps<{ team: DecryptoTeamEnum }>();
</script>

<template>
  <div v-if="MatchInfo" class="words block-border">
    <div class="block br bl" v-for="(clues, key) in MatchInfo.teams[team].words">
      <span class="header p-10">{{clues}}</span>
      <hr/>
      <div v-for="clue in MatchInfo.teams[team].clues[key]">
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