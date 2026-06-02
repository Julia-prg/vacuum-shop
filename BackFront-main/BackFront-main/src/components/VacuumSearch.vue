<script setup lang="ts">
const searchText = defineModel<string>('searchText', { default: '' })
</script>

<template>
  <input
    v-model="searchText"
    type="search"
    placeholder="Wyszukaj odkurzacz"
    class="input input-bordered w-full max-w-md"
  />
</template>
