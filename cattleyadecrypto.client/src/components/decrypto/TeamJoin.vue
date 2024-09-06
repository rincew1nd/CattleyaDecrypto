<script setup lang="ts">
import { DecryptoTeamEnum } from "@/components/types/DecryptoTypes";
import DecryptoDataService from "@/components/services/DecryptoDataService";
import { MatchInfo } from '../services/MatchManagerService'

const { team } = defineProps<{ team: DecryptoTeamEnum }>();

function joinTeam() {
  if (MatchInfo.value) {
    DecryptoDataService.joinTeam(MatchInfo.value.id, team);
  }
}
</script>

<template>
  <div class="center">
    <button :class="[team === DecryptoTeamEnum.Blue ? 'blue' : 'red']" @click=joinTeam>JOIN</button>
  </div>
</template>

<style scoped>
.center {
  text-align: center;
}
.center > button {
  width: 98%;
  padding: 0.5rem;
  border-radius: 10px;
}
</style>