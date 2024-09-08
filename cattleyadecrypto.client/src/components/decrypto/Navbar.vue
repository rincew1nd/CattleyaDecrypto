<script setup lang="ts">
import {ref} from "vue";
import {router} from "@/router";

import Button from 'primevue/button'
import Toolbar from 'primevue/toolbar'

import DecryptoDataService from "../services/DecryptoDataService"
import { useEventsBus } from '../services/UseEventBus';

const showModal = ref(false);
const newName = ref(DecryptoDataService.userAuthData.value?.name ?? '');

function newGameButton() {
  router.push('/decrypto/new');
}
function changeUserEvent() {
  DecryptoDataService
      .login(newName.value)
      .then(() => useEventsBus().emit('NameChanged', null))
      .then(() => showModal.value = false);
}
</script>

<template>
  <Toolbar>
    <template #start>
      <Button @click='newGameButton' label="New Game" text plain />
    </template>
    <template v-if="$route.params.id" #center>
      <h2>Game: {{$route.params.id}}</h2>
    </template>
    <template #end>
      <p class="nickname">{{ DecryptoDataService.userAuthData.value?.name }}</p>
      <Button @click="showModal = true">Change Name</Button>
    </template>
  </Toolbar>

  <Transition name="modal">
    <div v-if="showModal" class="modal-mask">
      <div class="modal-wrapper">
        <div class="modal-container">
          <div class="center modal-header">
            <slot name="body">
              Change name
            </slot>
            <a class='modal-default-button' @click="showModal = false">X</a>
          </div>
          <div class="modal-body">
            <input style="width:100%; height: 1.5rem" v-model="newName"/>
          </div>
          <div class="center modal-footer">
            <button @click="changeUserEvent">SEND</button>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.nickname {
  min-width: 100px;
  text-align: center;
  font-size: 1rem;
  padding: 0 20px;
}
.modal-mask {
  position: fixed;
  z-index: 9998;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgb(from var(--color-text) r g b / 0.25);
  display: table;
  transition: opacity 0.3s ease;
}
.modal-wrapper {
  display: table-cell;
  vertical-align: middle;
}
.modal-container {
  width: 300px;
  margin: 0px auto;
  padding: 20px 30px;
  border-radius: 2px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
  transition: all 0.3s ease;
  background: var(--color-background);
}
.modal-header h3 {
  margin-top: 0;
  color: #42b983;
}
.modal-body {
  margin: 20px 0;
}
.modal-default-button {
  float: right;
}
.modal-enter-from .modal-container,
.modal-leave-to .modal-container {
  transform: scale(1.1);
}
.center {
  text-align: center;
}
</style>