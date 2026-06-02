<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div class="min-h-screen bg-base-200">
    <header class="navbar bg-base-100 shadow-sm">
      <div class="flex-1 px-4">
        <span class="text-xl font-bold">VacuumPro</span>
      </div>
      <nav class="flex-none gap-2 px-4">
        <RouterLink class="btn btn-ghost" to="/">Katalog</RouterLink>
        <RouterLink class="btn btn-ghost" to="/cart">Koszyk</RouterLink>
      </nav>
    </header>
    <main class="p-6">
      <RouterView />
    </main>
  </div>
</template>

<style scoped></style>
