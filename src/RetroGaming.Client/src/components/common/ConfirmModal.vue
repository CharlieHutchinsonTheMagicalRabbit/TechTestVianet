<script setup lang="ts">
defineProps<{
  isOpen: boolean;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  isDestructive?: boolean;
  loading?: boolean;
}>();

const emit = defineEmits<{
  (e: 'confirm'): void;
  (e: 'cancel'): void;
}>();
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-sm animate-fade-in">
    <div
      class="bg-slate-900 border border-slate-700 rounded-xl shadow-2xl max-w-md w-full p-6 text-slate-100 transform transition-all"
    >
      <div class="flex items-center space-x-3 mb-4">
        <div
          class="p-2 rounded-lg"
          :class="isDestructive ? 'bg-rose-500/20 text-rose-400' : 'bg-cyan-500/20 text-cyan-400'"
        >
          <svg v-if="isDestructive" class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
          <svg v-else class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>
        <h3 class="text-lg font-bold">{{ title }}</h3>
      </div>

      <p class="text-sm text-slate-300 mb-6 leading-relaxed">
        {{ message }}
      </p>

      <div class="flex items-center justify-end space-x-3">
        <button
          type="button"
          :disabled="loading"
          class="px-4 py-2 rounded-lg text-sm font-medium text-slate-300 hover:text-white bg-slate-800 hover:bg-slate-700 transition border border-slate-700 disabled:opacity-50"
          @click="emit('cancel')"
        >
          {{ cancelText || 'Cancel' }}
        </button>
        <button
          type="button"
          :disabled="loading"
          class="px-4 py-2 rounded-lg text-sm font-medium transition flex items-center space-x-2 disabled:opacity-50"
          :class="isDestructive ? 'bg-rose-600 hover:bg-rose-500 text-white shadow-lg shadow-rose-900/40' : 'bg-cyan-600 hover:bg-cyan-500 text-white shadow-lg shadow-cyan-900/40'"
          @click="emit('confirm')"
        >
          <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
          </svg>
          <span>{{ confirmText || 'Confirm' }}</span>
        </button>
      </div>
    </div>
  </div>
</template>
