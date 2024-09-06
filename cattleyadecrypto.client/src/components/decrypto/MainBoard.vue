<script setup lang="ts">
import { ref } from "vue";

import { MatchInfo, Team, joinMatch } from '../services/MatchManagerService'

import WordsAndClues from "./WordsAndClues.vue";
import TeamInfo from "./TeamInfo.vue";
import TeamJoin from "./TeamJoin.vue";
import GiveClues from "./GiveClues.vue";
import {DecryptoMatchState, DecryptoTeamEnum} from "@/components/types/DecryptoTypes";

let loading = ref<boolean>(true);
const props = defineProps<{ id: string }>();

joinMatch(props.id).then(() => { loading.value = false; });
</script>

<template>
  <div v-if="loading" class="title">
    Loading match data...
  </div>
  <div v-if="MatchInfo">
    <p class="title">Round #{{MatchInfo.round}} | {{MatchInfo.state}}</p>
    <div class="split">
      <div class="team">
        <TeamInfo :team="DecryptoTeamEnum.Blue"/>
      </div>
      <div class="team">
        <TeamInfo :team="DecryptoTeamEnum.Red"/>
      </div>
    </div>
    <div v-if="Team" class="split">
      <WordsAndClues :team="DecryptoTeamEnum.Blue"/>
      <WordsAndClues :team="DecryptoTeamEnum.Red"/>
    </div>
    <div v-if="Team == DecryptoTeamEnum.Unknown" class="split">
      <TeamJoin :team="DecryptoTeamEnum.Blue"/>
      <TeamJoin :team="DecryptoTeamEnum.Red"/>
    </div>
    <div v-if="MatchInfo.state === DecryptoMatchState.GiveClues && Team">
      <GiveClues/>
    </div>
  </div>
</template>

<style scoped>
.title {
  text-align: center;
  font-size: 2rem;
}
.split {
  display: grid;
  grid-template-columns: 1fr 1fr;
}
</style>