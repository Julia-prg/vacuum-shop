<script setup lang="ts" generic="T extends { id: number; name: string; description?: string; price: number; picture?: string; power?: number }">
import { computed } from 'vue';
const _props = defineProps<{
  item: T;
}>();
const _emit = defineEmits<{
  (e: 'addToCart', item: T): void;
}>();
const _productImage = computed(() => {
  return _props.item.picture || 'https://images.unsplash.com/photo-1558317374-067fb5f30001?q=80&w=400';
});
</script>
<template>
  <div class="card card-compact w-80 bg-base-100 shadow-xl border border-base-200 transition-transform duration-300 hover:scale-105">

    <figure class="px-4 pt-4 h-48 bg-neutral-100 flex items-center justify-center overflow-hidden rounded-t-xl">
      <img
        :src="_productImage"
        :alt="_props.item.name"
        class="object-contain h-full w-full max-h-40"
      />
    </figure>

    <div class="card-body flex flex-col justify-between">
      <div>
        <h2 class="card-title text-secondary font-bold text-lg">
          {{ _props.item.name }}
        </h2>

        <div v-if="_props.item.power" class="badge badge-accent gap-1 my-1">
          Moc: {{ _props.item.power }}W
        </div>

        <p class="text-sm text-base-content/70 line-clamp-3 my-2">
          {{ _props.item.description || 'Brak opisu produktu.' }}
        </p>
      </div>

      <div class="card-actions justify-between items-center mt-4 pt-2 border-t border-base-100">
        <span class="text-xl font-extrabold text-primary">
          {{ _props.item.price.toFixed(2) }} PLN
        </span>

        <button
          @click="_emit('addToCart', _props.item)"
          class="btn btn-primary btn-sm normal-case flex gap-2 rounded-lg"
        >
          <span>Do koszyka</span>
        </button>
      </div>
    </div>
  </div>
</template>
