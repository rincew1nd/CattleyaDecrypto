<script lang="ts">
import {DecryptoStore} from './DecryptoStore.js';
import TeamInfo, {type TeamInfoObj} from "@/components/decrypto/TeamInfo.vue";
import WordsAndClues, {type WordsAndCluesObj} from "@/components/decrypto/WordsAndClues.vue";
import {defineComponent} from "vue";

export default defineComponent({
  computed: {
    DecryptoStore() {
      return DecryptoStore
    }
  },
  components: {WordsAndClues, TeamInfo },
  data() {
    return {
      teams: DecryptoStore.teams.map(team => ({
        name: team.name,
        players: team.players,
        interceptions: team.interceptions,
        miscommunications: team.miscommunications
      } as TeamInfoObj)),
      wordsAndClues: DecryptoStore.teams.map(team => {
        let words:WordsAndCluesObj = {};
        for (const property in team.words) {
          words[property] = team.words[property];
        }
        return words;
      })
    }
  }
})
</script>

<template>
  <div>
    <p>{{$route.params.id}}</p>
    <p class="p-card p-card-title"></p>
    <p class="round">Round #{{DecryptoStore.roundNumber}}</p>
    <div class="split">
      <div class="blue team">
        <TeamInfo :teamInfo="teams[0]"/>
      </div>
      <div class="red team">
        <TeamInfo :teamInfo="teams[1]"/>
      </div>
    </div>
    <div class="split">
      <WordsAndClues :wordsAndClues="wordsAndClues[0]"/>
      <WordsAndClues :wordsAndClues="wordsAndClues[1]"/>
    </div>
  </div>
</template>

<style scoped>
.round {
  text-align: center;
  font-size: 2rem;
}
.split {
  display: grid;
  grid-template-columns: 1fr 1fr;
}
</style>