<script setup lang="ts">
import {ref, onMounted, computed, watch} from "vue";

import WordsAndClues from "./WordsAndClues.vue";
import TeamInfo from "./TeamInfo.vue";
import TeamJoin from "./TeamJoin.vue";
import {type DecryptoMatch, DecryptoTeam} from "@/components/types/DecryptoTypes";

import DecryptoDataService from '../services/DecryptoDataService'
import { DecryptoMessageService } from '../services/DecryptoMessageService'

let loading = ref<boolean>(false);
let matchInfo = ref<DecryptoMatch | null>(null);
let team = ref<DecryptoTeam | null>(null);
const props = defineProps<{ id: string }>();

const decryptoMessageService = new DecryptoMessageService();

function updateMatch(match:DecryptoMatch) {
  matchInfo.value = match;
  let userId = DecryptoDataService.getPlayerId();
  Object.entries(matchInfo.value.teams).forEach(([key1, val1]) => {
    Object.entries(val1.players).forEach(([key2, val2]) => {
      if (key2 === userId) {
        team.value = DecryptoTeam[key1];
      }
    })
  })
}

onMounted(() => {
  matchInfo.value = null;
  loading.value = true;
  let matchId:string = '';

  DecryptoDataService.getMatch(props.id)
    .then(r => {
      updateMatch(r);
      loading.value = false;
      matchId = r.id;
    })
    .then(() => {
      return decryptoMessageService.start();
    })
    .then(() => {
      decryptoMessageService.joinDecryptoMatch(matchId, updateMatch);
    })
    .catch(err => {
      console.error('Error fetching match data:', err);
      loading.value = false;
    });
})
</script>

<template>
  <div v-if="loading" class="title">
    Loading match data...
  </div>
  <div v-if="matchInfo" class="content">
    <p class="title">Round #{{matchInfo.round}}</p>
    <div class="split">
      <div class="team">
        <TeamInfo :teamInfo="matchInfo.teams[DecryptoTeam.Blue]" :color="DecryptoTeam.Blue"/>
      </div>
      <div class="team">
        <TeamInfo :teamInfo="matchInfo.teams[DecryptoTeam.Red]" :color="DecryptoTeam.Red"/>
      </div>
    </div>
    <div v-if="team" class="split">
      <WordsAndClues :teamInfo="matchInfo.teams[DecryptoTeam.Blue]"/>
      <WordsAndClues :teamInfo="matchInfo.teams[DecryptoTeam.Red]"/>
    </div>
    <div v-else class="split">
      <TeamJoin :team="DecryptoTeam.Blue" :id="matchInfo?.id"/>
      <TeamJoin :team="DecryptoTeam.Red" :id="matchInfo?.id"/>
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