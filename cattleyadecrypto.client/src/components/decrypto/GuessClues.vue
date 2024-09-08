<script setup lang="ts">

import {ref} from "vue";
import {DecryptoMatchState} from "../types/DecryptoTypes";

import {MatchInfo, OppositeTeam, Team} from '../services/MatchManagerService'
import DecryptoDataService, {type SolveCluesRequest} from "../services/DecryptoDataService";

const cluesOrder = ref([0, 0, 0])
const team = MatchInfo.value?.state === DecryptoMatchState.SolveClues ? Team : OppositeTeam;

function sendGuesses() {
  if (Array.from(new Set(cluesOrder.value)).length === 3) {
    if (MatchInfo.value?.state === DecryptoMatchState.SolveClues) {
      DecryptoDataService.solveClues({
        matchId: MatchInfo.value.id,
        team: Team.value,
        order: cluesOrder.value
      } as SolveCluesRequest);
      return;
    }
    if (MatchInfo.value?.state === DecryptoMatchState.Intercept)
    {
      DecryptoDataService.intercept({
        matchId: MatchInfo.value.id,
        team: Team.value,
        order: cluesOrder.value
      } as SolveCluesRequest);
      return;
    }
    alert("Wrong match status to solve clues or intercept");
    return;
  }
  alert("You have to pick 3 different words")
}
</script>

<template>
  <hr/>
  <div v-if="MatchInfo?.roundClues && (MatchInfo.teams[team]?.words ?? null) !== null" class="content">
    <p v-if="MatchInfo.state === DecryptoMatchState.SolveClues">COMMUNICATION</p>
    <p v-if="MatchInfo.state === DecryptoMatchState.Intercept">INTERCEPTION</p>
    <table>
      <tr>
        <th class="t-clue">Clue</th>
        <th class="t-word" v-for="word in MatchInfo.teams[team].words">
          {{word}}
        </th>
      </tr>
      <tr v-for="(clue, clueIndex) in MatchInfo.roundClues[team].clues">
        <td class="t-clue">{{clue}}</td>
        <td class="t-word" v-for="(word, wordIndex) in MatchInfo.teams[team].words" >
          <input type="radio" @click="cluesOrder[clueIndex] = wordIndex" :name="clueIndex.toString()"/>
        </td>
      </tr>
    </table>
    <button @click="sendGuesses">SEND</button>
  </div>
</template>

<style scoped>
p {
  font-size: 1.5rem;
  padding: 10px;
}
.content {
  width: 50%;
  text-align: center;
  margin-left: auto;
  margin-right: auto;
}
table {
  width: 100%;
  margin: 10px 0px;
}
.t-clue {
  width: 40%;
}
.t-word {
  width: 10%;
}
table, th, td {
  border: 1px solid;
}
</style>