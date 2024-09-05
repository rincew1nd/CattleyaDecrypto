<script setup lang="ts">
import {ref} from "vue";

import type {DecryptoTeamEnum} from "@/components/types/DecryptoTypes";
import DecryptoDataService, { type GiveCluesRequest } from '../services/DecryptoDataService'

const order = [1, 3, 0];
const props = defineProps<{ id: string, team:DecryptoTeamEnum, words:Record<number, string> }>();

const isShown = ref(true)
const clue1 = ref('')
const clue2 = ref('')
const clue3 = ref('')

function submitClues() {
  DecryptoDataService.submitClues({
    matchId: props.id,
    team: props.team,
    order: order,
    clues: [clue1.value, clue2.value, clue3.value]
  } as GiveCluesRequest)
      .then(isSuccessful => {
        if (isSuccessful) {
          isShown.value = false;
        }
      })
}
</script>

<template>
  <hr/>
  <div v-if="isShown" class="block-center content">
    <p>Clues:</p>
    <div class="clues-block-grid block-border">
      <p>{{words[order[0]]}}</p>
      <input v-model="clue1" placeholder="edit me" />
      <p>{{words[order[1]]}}</p>
      <input v-model="clue2" placeholder="edit me" />
      <p>{{words[order[2]]}}</p>
      <input v-model="clue3" placeholder="edit me" />
    </div>
    <button @click="submitClues">Submit</button>
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
.clues-block-grid > p, .content > p, .content > button {
  text-align: center;
  font-size: 1rem;
}
</style>