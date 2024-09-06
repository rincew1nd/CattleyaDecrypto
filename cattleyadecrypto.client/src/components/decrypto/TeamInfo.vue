<script setup lang="ts">
import { DecryptoTeamEnum } from '../types/DecryptoTypes'
import { MatchInfo } from '../services/MatchManagerService'

const { team } = defineProps<{ team: DecryptoTeamEnum }>();
</script>

<template>
  <div class="block-border">
    <p :class="[team == DecryptoTeamEnum.Blue ? 'blue' : 'red', 'name']">TEAM {{team}}</p>
    <hr/>
    <div v-if="MatchInfo" class="info">
      <div class="br block">
        <p class="header">Players</p>
        <hr/>
        <div class="block-center">
          <p v-for="player in MatchInfo.teams[team].players">{{player}}</p>
        </div>
      </div>
      <div class="bl block">
        <div>
          <p class="header">Score</p>
          <hr/>
        </div>
        <div class="block-center">
          <p>Interceptions: {{ MatchInfo.teams[team].interceptionCount }}</p>
          <p>Miscommunications: {{ MatchInfo.teams[team].miscommunicationCount }}</p>
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