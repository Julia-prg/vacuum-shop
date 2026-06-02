<script setup lang="ts">
import { vacuums as vacuumsData } from '@/assets/vacuums'
import VacuumSearch from '@/components/VacuumSearch.vue'
import ProductCard from '@/components/ProductCard.vue'
import { computed, ref } from 'vue'
import type { Vacuum } from '@/types/Vacuum'

const vacuums = ref<Vacuum[]>([...vacuumsData])
const searchText = ref('')

const filteredVacuums = computed(() => {
  const query = searchText.value.toLowerCase().trim()
  if (!query) return vacuums.value
  return vacuums.value.filter(vacuum => vacuum.name.toLowerCase().includes(query))
})
const handleAddToCart = (item: Vacuum) => {
  alert(`Dodano do koszyka: ${item.name}`)
}
</script>

<template>
  <section class="p-4 max-w-7xl mx-auto">
    <h2 class="text-3xl font-bold mb-6 text-primary">Katalog odkurzaczy</h2>
    <VacuumSearch v-model:search-text="searchText" class="mb-10" />

    <div v-if="filteredVacuums.length === 0" class="alert alert-error shadow-lg">
      <i class="fa-solid fa-circle-xmark"></i>
      <span>Brak produktów pasujących do wyszukiwania.</span>
    </div>

    <div v-else class="flex flex-wrap gap-8 justify-center items-stretch">

      <ProductCard
        v-for="v in filteredVacuums"
        :key="v.id"
        :item="v"
        @add-to-cart="handleAddToCart"
      />

    </div>
  </section>
</template>
