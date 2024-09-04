<script setup lang="ts">
import { DecryptoTeam, type Team } from '../types/DecryptoTypes'

const { teamInfo, color } = defineProps<{ teamInfo: Team, color: DecryptoTeam }>();
</script>

<template>
  <div class="block-border">
    <p :class="[color == DecryptoTeam.Blue ? 'blue' : 'red', 'name']">TEAM {{color}}</p>
    <hr/>
    <div class="info">
      <div class="br block">
        <p class="header">Players</p>
        <hr/>
        <div class="block-center">
          <p v-for="player in teamInfo?.players">{{player}}</p>
        </div>
      </div>
      <div class="bl block">
        <div>
          <p class="header">Score</p>
          <hr/>
        </div>
        <div class="block-center">
          <p>Interceptions: {{ teamInfo.interceptionCount }}</p>
          <p>Miscommunications: {{ teamInfo.miscommunicationCount }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.info {
  display: grid;
  grid-template-columns: 1fr 1fr;
  text-align: center;
}
.name {
  text-align: center;
  font-size: 1.5rem;
}
</style>