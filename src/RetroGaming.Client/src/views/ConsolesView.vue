<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useConsoleStore } from '../stores/consoleStore';
import { useManufacturerStore } from '../stores/manufacturerStore';
import type { ConsoleDto, CreateConsoleRequest, UpdateConsoleRequest } from '../types';
import ConsoleModal from '../components/consoles/ConsoleModal.vue';
import ConfirmModal from '../components/common/ConfirmModal.vue';

const consoleStore = useConsoleStore();
const manufacturerStore = useManufacturerStore();

const isModalOpen = ref(false);
const editingConsole = ref<ConsoleDto | null>(null);

const isDeleteModalOpen = ref(false);
const consoleToDelete = ref<ConsoleDto | null>(null);

onMounted(async () => {
  await Promise.all([
    consoleStore.fetchConsoles(),
    manufacturerStore.fetchManufacturers()
  ]);
});

function handleSearch() {
  consoleStore.fetchConsoles();
}

function handleSort(column: string) {
  consoleStore.setSort(column);
}

function openCreateModal() {
  editingConsole.value = null;
  isModalOpen.value = true;
}

function openEditModal(console: ConsoleDto) {
  editingConsole.value = console;
  isModalOpen.value = true;
}

function openDeleteModal(console: ConsoleDto) {
  consoleToDelete.value = console;
  isDeleteModalOpen.value = true;
}

async function handleSaveCreate(data: CreateConsoleRequest) {
  const result = await consoleStore.createConsole(data);
  if (result) {
    isModalOpen.value = false;
    await consoleStore.fetchConsoles();
    await manufacturerStore.fetchManufacturers();
  }
}

async function handleSaveUpdate(id: string, data: UpdateConsoleRequest) {
  const result = await consoleStore.updateConsole(id, data);
  if (result) {
    isModalOpen.value = false;
    await consoleStore.fetchConsoles();
  }
}

async function handleConfirmDelete() {
  if (!consoleToDelete.value) return;
  const success = await consoleStore.deleteConsole(consoleToDelete.value.id);
  if (success) {
    isDeleteModalOpen.value = false;
    consoleToDelete.value = null;
    await manufacturerStore.fetchManufacturers();
  }
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header & Action Row -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h1 class="text-3xl font-extrabold tracking-tight text-white flex items-center gap-3">
          <span class="p-2 rounded-xl bg-cyan-500/10 border border-cyan-500/20 text-cyan-400">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </span>
          Retro Consoles Catalogue
        </h1>
        <p class="text-slate-400 text-sm mt-1">
          Explore, filter, and manage legendary retro gaming systems across hardware generations.
        </p>
      </div>

      <button
        type="button"
        class="inline-flex items-center justify-center space-x-2 px-4 py-2.5 rounded-xl font-semibold text-sm bg-gradient-to-r from-cyan-500 to-blue-600 hover:from-cyan-400 hover:to-blue-500 text-white shadow-lg shadow-cyan-500/20 transition active:scale-95"
        @click="openCreateModal"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        <span>Add Console</span>
      </button>
    </div>

    <!-- Filter & Search Bar -->
    <div class="bg-slate-900/90 border border-slate-800 rounded-2xl p-4 shadow-xl backdrop-blur space-y-4">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3">
        <!-- Search -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Search
          </label>
          <div class="relative">
            <input
              v-model="consoleStore.query.searchTerm"
              type="text"
              placeholder="Search by name..."
              class="w-full bg-slate-800/80 border border-slate-700/80 rounded-xl pl-9 pr-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-cyan-500"
              @input="handleSearch"
            />
            <svg class="w-4 h-4 text-slate-400 absolute left-3 top-2.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </div>
        </div>

        <!-- Manufacturer Filter -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Manufacturer
          </label>
          <select
            v-model="consoleStore.query.manufacturerId"
            class="w-full bg-slate-800/80 border border-slate-700/80 rounded-xl px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
            @change="handleSearch"
          >
            <option :value="undefined">All Manufacturers</option>
            <option
              v-for="m in manufacturerStore.manufacturers"
              :key="m.id"
              :value="m.id"
            >
              {{ m.name }}
            </option>
          </select>
        </div>

        <!-- Generation Filter -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Generation
          </label>
          <select
            v-model="consoleStore.query.generation"
            class="w-full bg-slate-800/80 border border-slate-700/80 rounded-xl px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
            @change="handleSearch"
          >
            <option :value="undefined">All Generations</option>
            <option :value="1">Gen 1 (1972-1977)</option>
            <option :value="2">Gen 2 (1976-1984)</option>
            <option :value="3">Gen 3 (1983-1990)</option>
            <option :value="4">Gen 4 (1987-1996)</option>
            <option :value="5">Gen 5 (1993-2002)</option>
            <option :value="6">Gen 6 (1998-2006)</option>
            <option :value="7">Gen 7 (2005-2013)</option>
            <option :value="8">Gen 8 (2012-2020)</option>
            <option :value="9">Gen 9 (2020+)</option>
          </select>
        </div>

        <!-- Discontinued Filter -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Status
          </label>
          <select
            v-model="consoleStore.query.isDiscontinued"
            class="w-full bg-slate-800/80 border border-slate-700/80 rounded-xl px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
            @change="handleSearch"
          >
            <option :value="undefined">All Statuses</option>
            <option :value="true">Discontinued / Legacy</option>
            <option :value="false">Active / In Production</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-2xl">
      <div v-if="consoleStore.loading" class="flex items-center justify-center py-20 text-slate-400">
        <svg class="animate-spin h-8 w-8 text-cyan-400 mr-3" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
        </svg>
        <span class="font-medium">Loading retro consoles...</span>
      </div>

      <div v-else-if="consoleStore.consoles.length === 0" class="text-center py-20 px-4">
        <div class="inline-flex p-4 rounded-full bg-slate-800/50 text-slate-500 mb-3">
          <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>
        <h3 class="text-lg font-bold text-slate-300">No consoles found</h3>
        <p class="text-slate-500 text-sm mt-1 max-w-sm mx-auto">
          Try clearing filters or search terms, or create a new console.
        </p>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="border-b border-slate-800 bg-slate-950/60 text-xs font-semibold text-slate-400 uppercase tracking-wider">
              <th
                class="py-3.5 px-4 cursor-pointer hover:text-cyan-400 transition select-none"
                @click="handleSort('Name')"
              >
                <div class="flex items-center space-x-1.5">
                  <span>Name</span>
                  <span v-if="consoleStore.query.sortBy === 'Name'" class="text-cyan-400">
                    {{ consoleStore.query.sortDescending ? '↓' : '↑' }}
                  </span>
                </div>
              </th>
              <th
                class="py-3.5 px-4 cursor-pointer hover:text-cyan-400 transition select-none"
                @click="handleSort('Manufacturer')"
              >
                <div class="flex items-center space-x-1.5">
                  <span>Manufacturer</span>
                  <span v-if="consoleStore.query.sortBy === 'Manufacturer'" class="text-cyan-400">
                    {{ consoleStore.query.sortDescending ? '↓' : '↑' }}
                  </span>
                </div>
              </th>
              <th
                class="py-3.5 px-4 cursor-pointer hover:text-cyan-400 transition select-none"
                @click="handleSort('ReleaseYear')"
              >
                <div class="flex items-center space-x-1.5">
                  <span>Release Year</span>
                  <span v-if="consoleStore.query.sortBy === 'ReleaseYear'" class="text-cyan-400">
                    {{ consoleStore.query.sortDescending ? '↓' : '↑' }}
                  </span>
                </div>
              </th>
              <th
                class="py-3.5 px-4 cursor-pointer hover:text-cyan-400 transition select-none"
                @click="handleSort('Generation')"
              >
                <div class="flex items-center space-x-1.5">
                  <span>Gen</span>
                  <span v-if="consoleStore.query.sortBy === 'Generation'" class="text-cyan-400">
                    {{ consoleStore.query.sortDescending ? '↓' : '↑' }}
                  </span>
                </div>
              </th>
              <th
                class="py-3.5 px-4 cursor-pointer hover:text-cyan-400 transition select-none"
                @click="handleSort('UnitsSoldInMillions')"
              >
                <div class="flex items-center space-x-1.5">
                  <span>Units Sold</span>
                  <span v-if="consoleStore.query.sortBy === 'UnitsSoldInMillions'" class="text-cyan-400">
                    {{ consoleStore.query.sortDescending ? '↓' : '↑' }}
                  </span>
                </div>
              </th>
              <th class="py-3.5 px-4">Status</th>
              <th class="py-3.5 px-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-800/60 text-sm">
            <tr
              v-for="c in consoleStore.consoles"
              :key="c.id"
              class="hover:bg-slate-800/40 transition group"
            >
              <td class="py-3 px-4 font-semibold text-slate-100 group-hover:text-cyan-300 transition">
                {{ c.name }}
              </td>
              <td class="py-3 px-4 text-slate-300">
                <span class="inline-flex items-center px-2 py-0.5 rounded-md text-xs font-medium bg-slate-800 text-fuchsia-300 border border-slate-700">
                  {{ c.manufacturerName }}
                </span>
              </td>
              <td class="py-3 px-4 text-slate-300 font-mono">
                {{ c.releaseYear }}
              </td>
              <td class="py-3 px-4">
                <span class="inline-flex items-center justify-center w-6 h-6 rounded bg-slate-800 text-xs font-bold text-cyan-400 border border-cyan-500/20">
                  {{ c.generation }}
                </span>
              </td>
              <td class="py-3 px-4 font-bold text-amber-400 font-mono">
                {{ c.unitsSoldInMillions.toFixed(2) }}M
              </td>
              <td class="py-3 px-4">
                <span
                  class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-semibold"
                  :class="c.isDiscontinued ? 'bg-slate-800 text-slate-400 border border-slate-700' : 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/30'"
                >
                  {{ c.isDiscontinued ? 'Legacy' : 'Active' }}
                </span>
              </td>
              <td class="py-3 px-4 text-right">
                <div class="inline-flex items-center space-x-1">
                  <button
                    type="button"
                    class="p-1.5 rounded-lg text-slate-400 hover:text-cyan-400 hover:bg-slate-800 transition"
                    title="Edit Console"
                    @click="openEditModal(c)"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>
                  <button
                    type="button"
                    class="p-1.5 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-slate-800 transition"
                    title="Delete Console"
                    @click="openDeleteModal(c)"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modals -->
    <ConsoleModal
      :is-open="isModalOpen"
      :console-to-edit="editingConsole"
      :manufacturers="manufacturerStore.manufacturers"
      :loading="consoleStore.loading"
      @close="isModalOpen = false"
      @save-create="handleSaveCreate"
      @save-update="handleSaveUpdate"
    />

    <ConfirmModal
      :is-open="isDeleteModalOpen"
      title="Delete Console"
      :message="`Are you sure you want to delete '${consoleToDelete?.name}'? This action cannot be undone.`"
      confirm-text="Delete"
      is-destructive
      :loading="consoleStore.loading"
      @cancel="isDeleteModalOpen = false"
      @confirm="handleConfirmDelete"
    />
  </div>
</template>
