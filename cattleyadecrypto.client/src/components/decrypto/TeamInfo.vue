<script lang="ts">
import {defineComponent, type PropType} from 'vue'

export interface TeamInfoObj {
  name: string
  players: string[]
  miscommunications: number
  interceptions: number
}

export default defineComponent({
  props: {
    teamInfo: {
      type: Object as PropType<TeamInfoObj>,
      required: true
    }
  }
})
</script>

<template>
  <div class="block-border">
    <p class="name">Team: {{teamInfo.name}}</p>
    <hr/>
    <div class="info">
      <div class="br">
        <p class="header">Players</p>
        <hr/>
        <div>
          <p v-for="nickname in teamInfo.players">{{nickname}}</p>
        </div>
      </div>
      <div class="bl block">
        <div>
          <p class="header">Score</p>
          <hr/>
        </div>
        <div class="block-center">
          <p>Interceptions: {{teamInfo.interceptions}}</p>
          <p>Miscommunications: {{teamInfo.miscommunications}}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.info {
  display: grid;
  grid-template-columns: 1fr 1fr;
  text-align: center;
}
.name {
  text-align: center;
  font-size: 1.5rem;
  background: v-bind('teamInfo.name');
}
</style>