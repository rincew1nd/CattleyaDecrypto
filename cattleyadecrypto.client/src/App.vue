<script setup lang="ts">
import Navbar from "@/components/decrypto/Navbar.vue";
</script>

<template>
  <div>
    <header>
      <Navbar />
    </header>

    <router-view></router-view>
  </div>
</template>
