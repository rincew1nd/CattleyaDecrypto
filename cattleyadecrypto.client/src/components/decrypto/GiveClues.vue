<script setup lang="ts">
import { ref } from "vue";

import { MatchInfo, Team, NeedRiddler, IsRiddler } from '../services/MatchManagerService'
import DecryptoDataService, { type SubmitCluesRequest } from '../services/DecryptoDataService'

const clue1 = ref('')
const clue2 = ref('')
const clue3 = ref('')

function submitClues() {
  if (MatchInfo.value) {
    DecryptoDataService.submitClues({
      matchId: MatchInfo.value.id,
      team: Team.value,
      order: MatchInfo.value.roundClues[Team.value].order,
      clues: [clue1.value, clue2.value, clue3.value]
    } as SubmitCluesRequest)
  }
}

function assignPlayer() {
  if (MatchInfo.value) {
    DecryptoDataService.assignRiddler(MatchInfo.value.id);
  }
}
</script>

<template>
  <hr/>
  <div v-if="MatchInfo?.roundClues">
    <div v-if="NeedRiddler" class="block-center content">
      <button @click="assignPlayer">Give clues</button>
    </div>
    <div v-if="IsRiddler && MatchInfo && (MatchInfo.roundClues[Team]?.clues ?? null) === null"
         class="block-center content">
      <p>Clues:</p>
      <div v-if="MatchInfo.roundClues[Team].order" class="clues-block-grid block-border">
        <span>{{MatchInfo.teams[Team].words[MatchInfo.roundClues[Team]?.order[0] ?? 0]}}</span>
        <input v-model="clue1" placeholder="edit me" />
        <span>{{MatchInfo.teams[Team].words[MatchInfo.roundClues[Team]?.order[1] ?? 0]}}</span>
        <input v-model="clue2" placeholder="edit me" />
        <span>{{MatchInfo.teams[Team].words[MatchInfo.roundClues[Team]?.order[2] ?? 0]}}</span>
        <input v-model="clue3" placeholder="edit me" />
      </div>
      <button @click="submitClues">Submit</button>
    </div>
    <div v-if="!NeedRiddler && MatchInfo" class="content">
      <div v-if="(MatchInfo.roundClues[Team]?.clues ?? null) !== null">
        <p>Waiting for opponent team to make clues...</p>
      </div>
      <div v-else-if="!IsRiddler">
        <p>Waiting for your team to make clues...</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.content {
  margin-left: auto;
  margin-right: auto;
  width: 60%;
  text-align: center;
}
.clues-block-grid {
  display: grid;
  grid-template-columns: 1fr 3fr;
}
.clues-block-grid > span, .content > p, .content > button {
  text-align: center;
  font-size: 1rem;
  padding: 5px 0;
}
.content > button {
  padding: 5px;
}
hr {
  margin: 10px 0;
}
</style>