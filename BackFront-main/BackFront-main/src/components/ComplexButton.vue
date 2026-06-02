<script setup lang="ts">
const _props = withDefaults(
  defineProps<{
    type?: 'button' | 'submit' | 'reset';
    colorClass?: string;
    loading?: boolean;
    disabled?: boolean;
  }>(),
  {
    type: 'button',
    colorClass: 'btn-primary',
    loading: false,
    disabled: false,
  }
);

const _emit = defineEmits<{
  (e: 'click', event: MouseEvent): void;
}>();

const _handleClick = (event: MouseEvent) => {
  if (_props.disabled || _props.loading) return;

  _emit('click', event);
};
</script>
<template>
  <button
    :type="_props.type"
    :disabled="_props.disabled || _props.loading"
    @click.prevent="_handleClick"
    class="btn rounded-lg normal-case transition-all duration-200 flex items-center justify-center gap-2"
    :class="[_props.colorClass, _props.loading ? 'loading' : '']"
  >
    <span v-if="_props.loading" class="loading loading-spinner loading-sm"></span>

    <slot />
  </button>
</template>
