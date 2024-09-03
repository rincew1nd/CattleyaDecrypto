<script lang="ts">
import {defineComponent, type PropType} from 'vue'

export interface WordsAndCluesObj {
  [word: string]: string[];
}

export default defineComponent({
  props: {
    wordsAndClues: {
      type: Object as PropType<WordsAndCluesObj>,
      required: true
    }
  },
  setup(props) {
    console.log('Received object:', props.wordsAndClues)
    return {}
  }
})
</script>

<template>
  <div class="words block-border">
    <div class="block br bl" v-for="(clues, key) in wordsAndClues">
      <span class="header">{{key}}</span>
      <hr/>
      <div v-for="clue in clues">
        <p>{{clue}}</p>
        <hr/>
      </div>
    </div>
  </div>
</template>

<style scoped>
.words {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
  text-align: center;
}
</style>