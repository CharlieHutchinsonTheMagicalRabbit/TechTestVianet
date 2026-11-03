<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useManufacturerStore } from '../stores/manufacturerStore';
import type {
  ManufacturerDto,
  CreateManufacturerRequest,
  UpdateManufacturerRequest
} from '../types';
import ManufacturerModal from '../components/manufacturers/ManufacturerModal.vue';
import ManufacturerDetailModal from '../components/manufacturers/ManufacturerDetailModal.vue';
import ConfirmModal from '../components/common/ConfirmModal.vue';

const manufacturerStore = useManufacturerStore();

const isModalOpen = ref(false);
const editingManufacturer = ref<ManufacturerDto | null>(null);

const isDetailModalOpen = ref(false);

const isDeleteModalOpen = ref(false);
const manufacturerToDelete = ref<ManufacturerDto | null>(null);

onMounted(async () => {
  await manufacturerStore.fetchManufacturers();
});

function openCreateModal() {
  editingManufacturer.value = null;
  isModalOpen.value = true;
}

function openEditModal(manufacturer: ManufacturerDto) {
  editingManufacturer.value = manufacturer;
  isModalOpen.value = true;
}

async function openDetailModal(manufacturer: ManufacturerDto) {
  isDetailModalOpen.value = true;
  await manufacturerStore.fetchManufacturerById(manufacturer.id);
}

function openDeleteModal(manufacturer: ManufacturerDto) {
  manufacturerToDelete.value = manufacturer;
  isDeleteModalOpen.value = true;
}

async function handleSaveCreate(data: CreateManufacturerRequest) {
  const result = await manufacturerStore.createManufacturer(data);
  if (result) {
    isModalOpen.value = false;
  }
}

async function handleSaveUpdate(id: string, data: UpdateManufacturerRequest) {
  const result = await manufacturerStore.updateManufacturer(id, data);
  if (result) {
    isModalOpen.value = false;
  }
}

async function handleConfirmDelete() {
  if (!manufacturerToDelete.value) return;
  const success = await manufacturerStore.deleteManufacturer(manufacturerToDelete.value.id);
  if (success) {
    isDeleteModalOpen.value = false;
    manufacturerToDelete.value = null;
  } else {
    // If failed (e.g. consoles still exist), close delete prompt as toast already showed reason
    isDeleteModalOpen.value = false;
  }
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header & Action Row -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h1 class="text-3xl font-extrabold tracking-tight text-white flex items-center gap-3">
          <span class="p-2 rounded-xl bg-fuchsia-500/10 border border-fuchsia-500/20 text-fuchsia-400">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
            </svg>
          </span>
          Console Manufacturers
        </h1>
        <p class="text-slate-400 text-sm mt-1">
          Pioneering game industry companies, global headquarters, and hardware release statistics.
        </p>
      </div>

      <button
        type="button"
        class="inline-flex items-center justify-center space-x-2 px-4 py-2.5 rounded-xl font-semibold text-sm bg-gradient-to-r from-fuchsia-600 to-pink-600 hover:from-fuchsia-500 hover:to-pink-500 text-white shadow-lg shadow-fuchsia-500/20 transition active:scale-95"
        @click="openCreateModal"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        <span>Add Manufacturer</span>
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="manufacturerStore.loading && manufacturerStore.manufacturers.length === 0" class="flex items-center justify-center py-24 text-slate-400">
      <svg class="animate-spin h-8 w-8 text-fuchsia-400 mr-3" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
      </svg>
      <span class="font-medium">Loading manufacturers...</span>
    </div>

    <!-- Empty State -->
    <div v-else-if="manufacturerStore.manufacturers.length === 0" class="bg-slate-900 border border-slate-800 rounded-2xl p-12 text-center shadow-xl">
      <div class="inline-flex p-4 rounded-full bg-slate-800/50 text-slate-500 mb-3">
        <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
        </svg>
      </div>
      <h3 class="text-lg font-bold text-slate-300">No manufacturers registered</h3>
      <p class="text-slate-500 text-sm mt-1 max-w-sm mx-auto">
        Create your first manufacturer to begin cataloguing gaming hardware.
      </p>
    </div>

    <!-- Manufacturers Cards Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="m in manufacturerStore.manufacturers"
        :key="m.id"
        class="bg-slate-900/90 border border-slate-800 rounded-2xl p-6 shadow-xl hover:border-slate-700 transition flex flex-col justify-between group backdrop-blur"
      >
        <div>
          <!-- Card Top Bar -->
          <div class="flex items-start justify-between gap-3 mb-4">
            <div>
              <h3 class="text-xl font-bold text-white group-hover:text-fuchsia-300 transition">
                {{ m.name }}
              </h3>
              <p class="text-xs text-slate-400 flex items-center space-x-1 mt-0.5">
                <svg class="w-3.5 h-3.5 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                <span>{{ m.hqCity }}, {{ m.hqCountry }}</span>
              </p>
            </div>

            <span class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-bold bg-fuchsia-500/10 text-fuchsia-400 border border-fuchsia-500/20">
              Est. {{ m.foundedYear }}
            </span>
          </div>

          <!-- Badges & Stats -->
          <div class="space-y-2 mb-6">
            <div class="flex items-center justify-between text-xs py-1.5 px-3 rounded-lg bg-slate-800/60 border border-slate-800">
              <span class="text-slate-400">Total Consoles:</span>
              <span class="font-bold text-cyan-400">{{ m.consoleCount }} systems</span>
            </div>

            <div class="flex items-center justify-between text-xs py-1.5 px-3 rounded-lg bg-slate-800/60 border border-slate-800">
              <span class="text-slate-400">HQ Coordinates:</span>
              <span class="font-mono text-slate-300 text-[11px]">
                {{ m.latitude.toFixed(2) }}°, {{ m.longitude.toFixed(2) }}°
              </span>
            </div>
          </div>
        </div>

        <!-- Action Buttons Footer -->
        <div class="flex items-center justify-between pt-4 border-t border-slate-800/80 gap-2">
          <button
            type="button"
            class="flex-1 py-1.5 px-3 rounded-lg text-xs font-semibold bg-slate-800 hover:bg-slate-700 text-slate-200 transition border border-slate-700 text-center"
            @click="openDetailModal(m)"
          >
            View Catalogue
          </button>

          <div class="flex items-center space-x-1">
            <button
              type="button"
              class="p-2 rounded-lg text-slate-400 hover:text-cyan-400 hover:bg-slate-800 transition"
              title="Edit Manufacturer"
              @click="openEditModal(m)"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
            </button>

            <button
              type="button"
              class="p-2 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-slate-800 transition"
              title="Delete Manufacturer"
              @click="openDeleteModal(m)"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modals -->
    <ManufacturerModal
      :is-open="isModalOpen"
      :manufacturer-to-edit="editingManufacturer"
      :loading="manufacturerStore.loading"
      @close="isModalOpen = false"
      @save-create="handleSaveCreate"
      @save-update="handleSaveUpdate"
    />

    <ManufacturerDetailModal
      :is-open="isDetailModalOpen"
      :manufacturer="manufacturerStore.currentManufacturer"
      :loading="manufacturerStore.loading"
      @close="isDetailModalOpen = false"
    />

    <ConfirmModal
      :is-open="isDeleteModalOpen"
      title="Delete Manufacturer"
      :message="`Are you sure you want to delete '${manufacturerToDelete?.name}'? This will be rejected if the manufacturer still has consoles in the catalogue.`"
      confirm-text="Delete"
      is-destructive
      :loading="manufacturerStore.loading"
      @cancel="isDeleteModalOpen = false"
      @confirm="handleConfirmDelete"
    />
  </div>
</template>
