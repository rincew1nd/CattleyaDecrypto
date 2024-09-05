<script setup lang="ts">
import {DecryptoTeamEnum} from "@/components/types/DecryptoTypes";
import DecryptoDataService from "@/components/services/DecryptoDataService";

const { team, id } = defineProps<{ team: DecryptoTeamEnum, id: string }>();

function joinTeam() {
  DecryptoDataService.joinTeam(id, team);
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