<script setup lang="ts">
import { ref } from "vue";

import { MatchInfo, Team, joinMatch } from '../services/MatchManagerService'

import WordsAndClues from "./WordsAndClues.vue";
import TeamInfo from "./TeamInfo.vue";
import TeamJoin from "./TeamJoin.vue";
import {  DecryptoTeamEnum } from "@/components/types/DecryptoTypes";

let loading = ref<boolean>(true);
const props = defineProps<{ id: string }>();

joinMatch(props.id).then(() => {
  loading.value = false;
});
</script>

<template>
  <div v-if="loading" class="title">
    Loading match data...
  </div>
  <div v-if="MatchInfo" class="content">
    <p class="title">Round #{{MatchInfo.round}}</p>
    <div class="split">
      <div class="team">
        <TeamInfo :teamInfo="MatchInfo.teams[DecryptoTeamEnum.Blue]" :color="DecryptoTeamEnum.Blue"/>
      </div>
      <div class="team">
        <TeamInfo :teamInfo="MatchInfo.teams[DecryptoTeamEnum.Red]" :color="DecryptoTeamEnum.Red"/>
      </div>
    </div>
    <div v-if="Team" class="split">
      <WordsAndClues :teamInfo="MatchInfo.teams[DecryptoTeamEnum.Blue]"/>
      <WordsAndClues :teamInfo="MatchInfo.teams[DecryptoTeamEnum.Red]"/>
    </div>
    <div v-else class="split">
      <TeamJoin :team="DecryptoTeamEnum.Blue" :id="props.id"/>
      <TeamJoin :team="DecryptoTeamEnum.Red" :id="props.id"/>
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