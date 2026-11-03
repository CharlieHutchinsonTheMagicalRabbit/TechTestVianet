<script setup lang="ts">
import type { ManufacturerDetailDto } from '../../types';

defineProps<{
  isOpen: boolean;
  manufacturer: ManufacturerDetailDto | null;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'close'): void;
}>();
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/75 backdrop-blur-sm overflow-y-auto">
    <div class="bg-slate-900 border border-slate-700 rounded-xl shadow-2xl max-w-2xl w-full p-6 text-slate-100 my-8">
      <div class="flex items-center justify-between pb-4 border-b border-slate-800 mb-5">
        <div v-if="manufacturer">
          <h3 class="text-2xl font-bold bg-gradient-to-r from-fuchsia-400 to-cyan-400 bg-clip-text text-transparent">
            {{ manufacturer.name }}
          </h3>
          <p class="text-xs text-slate-400 mt-0.5">
            {{ manufacturer.hqCity }}, {{ manufacturer.hqCountry }} &bull; Founded {{ manufacturer.foundedYear }}
          </p>
        </div>
        <div v-else>
          <h3 class="text-xl font-bold">Manufacturer Details</h3>
        </div>
        <button
          type="button"
          class="text-slate-400 hover:text-white transition"
          @click="emit('close')"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <div v-if="loading" class="flex justify-center py-12">
        <svg class="animate-spin h-8 w-8 text-cyan-400" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
        </svg>
      </div>

      <div v-else-if="manufacturer" class="space-y-6">
        <!-- Overview Stats -->
        <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
          <div class="bg-slate-800/60 p-3 rounded-lg border border-slate-700/60">
            <span class="text-xs text-slate-400">Headquarters</span>
            <p class="text-sm font-semibold truncate">{{ manufacturer.hqCity }}, {{ manufacturer.hqCountry }}</p>
          </div>
          <div class="bg-slate-800/60 p-3 rounded-lg border border-slate-700/60">
            <span class="text-xs text-slate-400">Founded</span>
            <p class="text-sm font-semibold text-fuchsia-400">{{ manufacturer.foundedYear }}</p>
          </div>
          <div class="bg-slate-800/60 p-3 rounded-lg border border-slate-700/60">
            <span class="text-xs text-slate-400">Coordinates</span>
            <p class="text-xs font-mono text-cyan-400 truncate">{{ manufacturer.latitude.toFixed(4) }}, {{ manufacturer.longitude.toFixed(4) }}</p>
          </div>
          <div class="bg-slate-800/60 p-3 rounded-lg border border-slate-700/60">
            <span class="text-xs text-slate-400">Consoles</span>
            <p class="text-sm font-semibold text-emerald-400">{{ manufacturer.consoles.length }}</p>
          </div>
        </div>

        <!-- Consoles Catalogue -->
        <div>
          <h4 class="text-sm font-bold uppercase tracking-wider text-slate-400 mb-3">
            Hardware Catalogue
          </h4>

          <div v-if="manufacturer.consoles.length === 0" class="text-center py-8 bg-slate-800/30 rounded-lg border border-slate-800 text-slate-400 text-sm">
            No consoles registered yet for {{ manufacturer.name }}.
          </div>

          <div v-else class="space-y-2 max-h-64 overflow-y-auto pr-1">
            <div
              v-for="c in manufacturer.consoles"
              :key="c.id"
              class="flex items-center justify-between p-3 rounded-lg bg-slate-800/40 border border-slate-800 hover:border-slate-700 transition"
            >
              <div>
                <p class="text-sm font-semibold text-slate-100">{{ c.name }}</p>
                <div class="flex items-center space-x-2 text-xs text-slate-400 mt-0.5">
                  <span>Released {{ c.releaseYear }}</span>
                  <span>&bull;</span>
                  <span class="px-1.5 py-0.5 rounded bg-slate-700 text-slate-300 font-mono">Gen {{ c.generation }}</span>
                </div>
              </div>

              <div class="text-right">
                <span class="text-xs font-bold text-amber-400 block">{{ c.unitsSoldInMillions }}M units</span>
                <span
                  class="text-[10px] uppercase font-semibold px-2 py-0.5 rounded-full"
                  :class="c.isDiscontinued ? 'bg-slate-700/80 text-slate-400' : 'bg-emerald-500/20 text-emerald-400'"
                >
                  {{ c.isDiscontinued ? 'Legacy' : 'Active' }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="flex justify-end pt-4 border-t border-slate-800 mt-6">
        <button
          type="button"
          class="px-5 py-2 rounded-lg text-sm font-medium bg-slate-800 hover:bg-slate-700 text-white transition border border-slate-700"
          @click="emit('close')"
        >
          Close
        </button>
      </div>
    </div>
  </div>
</template>
