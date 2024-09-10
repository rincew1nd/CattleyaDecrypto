<script setup lang="ts">
import {MatchInfo} from '../services/MatchManagerService'
import {computed} from "vue";
import {DecryptoMatchState, DecryptoTeamEnum} from "@/components/types/DecryptoTypes";

const wonTeam = computed(() => {
  if (MatchInfo.value?.state === DecryptoMatchState.Finished) {
    return MatchInfo.value.wonTeam === DecryptoTeamEnum.Unknown
      ? 'TIE'
      : MatchInfo.value.wonTeam === DecryptoTeamEnum.Red
        ? 'RED'
        : 'BLUE';
  }
  return 'ERROR';
})
</script>

<template>
  <div>
    <p class="text-notification">MATCH FINISHED: {{wonTeam}}</p>
  </div>
</template>