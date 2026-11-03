<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import type { ConsoleDto, CreateConsoleRequest, UpdateConsoleRequest, ManufacturerDto } from '../../types';
import { validateNumberInRange, validateYear, MIN_YEAR } from '../../utils/validation';

const props = defineProps<{
  isOpen: boolean;
  consoleToEdit: ConsoleDto | null;
  manufacturers: ManufacturerDto[];
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'close'): void;
  (e: 'save-create', data: CreateConsoleRequest): void;
  (e: 'save-update', id: string, data: UpdateConsoleRequest): void;
}>();

const currentYear = new Date().getFullYear();

const manufacturerId = ref('');
const name = ref('');
const releaseYear = ref<number>(1990);
const generation = ref<number>(4);
const unitsSoldInMillions = ref<number>(0);
const isDiscontinued = ref<boolean>(true);

const errors = ref<Record<string, string>>({});

const isEditing = computed(() => !!props.consoleToEdit);

const selectedManufacturer = computed(() =>
  props.manufacturers.find(m => m.id === manufacturerId.value)
);

watch(
  () => props.isOpen,
  (open) => {
    if (open) {
      errors.value = {};
      if (props.consoleToEdit) {
        manufacturerId.value = props.consoleToEdit.manufacturerId;
        name.value = props.consoleToEdit.name;
        releaseYear.value = props.consoleToEdit.releaseYear;
        generation.value = props.consoleToEdit.generation;
        unitsSoldInMillions.value = props.consoleToEdit.unitsSoldInMillions;
        isDiscontinued.value = props.consoleToEdit.isDiscontinued;
      } else {
        manufacturerId.value = props.manufacturers.length > 0 ? props.manufacturers[0].id : '';
        name.value = '';
        releaseYear.value = 1990;
        generation.value = 4;
        unitsSoldInMillions.value = 0;
        isDiscontinued.value = true;
      }
    }
  }
);

function validate(): boolean {
  errors.value = {};

  if (!isEditing.value && !manufacturerId.value) {
    errors.value.manufacturerId = 'Manufacturer is required.';
  }

  if (!name.value.trim()) {
    errors.value.name = 'Console name is required.';
  } else if (name.value.length > 100) {
    errors.value.name = 'Console name cannot exceed 100 characters.';
  }

  const releaseYearError = validateYear(releaseYear.value, 'Release year', MIN_YEAR, currentYear + 1);
  if (releaseYearError) {
    errors.value.releaseYear = releaseYearError;
  } else if (selectedManufacturer.value && releaseYear.value < selectedManufacturer.value.foundedYear) {
    errors.value.releaseYear = `Release year cannot predate ${selectedManufacturer.value.name}'s founding year (${selectedManufacturer.value.foundedYear}).`;
  }

  const generationError = validateNumberInRange(generation.value, 'Generation', 1, 15);
  if (generationError) {
    errors.value.generation = generationError;
  }

  const unitsSoldError = validateNumberInRange(unitsSoldInMillions.value, 'Units sold', 0, Number.MAX_SAFE_INTEGER);
  if (unitsSoldError) {
    errors.value.unitsSoldInMillions = unitsSoldError;
  }

  return Object.keys(errors.value).length === 0;
}

function handleSubmit() {
  if (!validate()) return;

  if (isEditing.value && props.consoleToEdit) {
    const updatePayload: UpdateConsoleRequest = {
      name: name.value.trim(),
      releaseYear: releaseYear.value,
      generation: generation.value,
      unitsSoldInMillions: Number(unitsSoldInMillions.value),
      isDiscontinued: isDiscontinued.value
    };
    emit('save-update', props.consoleToEdit.id, updatePayload);
  } else {
    const createPayload: CreateConsoleRequest = {
      manufacturerId: manufacturerId.value,
      name: name.value.trim(),
      releaseYear: releaseYear.value,
      generation: generation.value,
      unitsSoldInMillions: Number(unitsSoldInMillions.value),
      isDiscontinued: isDiscontinued.value
    };
    emit('save-create', createPayload);
  }
}
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/75 backdrop-blur-sm overflow-y-auto">
    <div class="bg-slate-900 border border-slate-700 rounded-xl shadow-2xl max-w-lg w-full p-6 text-slate-100 my-8">
      <div class="flex items-center justify-between pb-4 border-b border-slate-800 mb-5">
        <h3 class="text-xl font-bold bg-gradient-to-r from-cyan-400 to-fuchsia-400 bg-clip-text text-transparent">
          {{ isEditing ? 'Edit Console' : 'Add New Console' }}
        </h3>
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

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <!-- Manufacturer -->
        <div v-if="!isEditing">
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Manufacturer *
          </label>
          <select
            v-model="manufacturerId"
            class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
            :class="{ 'border-rose-500': errors.manufacturerId }"
          >
            <option disabled value="">Select a manufacturer</option>
            <option
              v-for="m in manufacturers"
              :key="m.id"
              :value="m.id"
            >
              {{ m.name }} (Founded {{ m.foundedYear }})
            </option>
          </select>
          <p v-if="errors.manufacturerId" class="text-rose-400 text-xs mt-1">{{ errors.manufacturerId }}</p>
        </div>

        <div v-else class="bg-slate-800/50 p-3 rounded-lg border border-slate-800">
          <span class="text-xs text-slate-400">Manufacturer</span>
          <p class="text-sm font-semibold text-cyan-400">{{ consoleToEdit?.manufacturerName }}</p>
        </div>

        <!-- Name -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Console Name *
          </label>
          <input
            v-model="name"
            type="text"
            maxlength="100"
            placeholder="e.g. Super Nintendo Entertainment System"
            class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-cyan-500"
            :class="{ 'border-rose-500': errors.name }"
          />
          <p v-if="errors.name" class="text-rose-400 text-xs mt-1">{{ errors.name }}</p>
        </div>

        <!-- Grid: Release Year & Generation -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
              Release Year *
            </label>
            <input
              v-model.number="releaseYear"
              type="number"
              :min="1970"
              :max="currentYear + 1"
              class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
              :class="{ 'border-rose-500': errors.releaseYear }"
            />
            <p v-if="errors.releaseYear" class="text-rose-400 text-xs mt-1">{{ errors.releaseYear }}</p>
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
              Generation *
            </label>
            <input
              v-model.number="generation"
              type="number"
              min="1"
              max="15"
              class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
              :class="{ 'border-rose-500': errors.generation }"
            />
            <p v-if="errors.generation" class="text-rose-400 text-xs mt-1">{{ errors.generation }}</p>
          </div>
        </div>

        <!-- Units Sold -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Units Sold (Millions) *
          </label>
          <input
            v-model.number="unitsSoldInMillions"
            type="number"
            step="0.01"
            min="0"
            placeholder="0.00"
            class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
            :class="{ 'border-rose-500': errors.unitsSoldInMillions }"
          />
          <p v-if="errors.unitsSoldInMillions" class="text-rose-400 text-xs mt-1">{{ errors.unitsSoldInMillions }}</p>
        </div>

        <!-- Is Discontinued -->
        <div class="flex items-center space-x-3 pt-2">
          <input
            id="isDiscontinued"
            v-model="isDiscontinued"
            type="checkbox"
            class="w-4 h-4 rounded text-cyan-500 bg-slate-800 border-slate-700 focus:ring-cyan-500 focus:ring-offset-slate-900"
          />
          <label for="isDiscontinued" class="text-sm font-medium text-slate-300">
            Console is Discontinued (Legacy Hardware)
          </label>
        </div>

        <!-- Action Buttons -->
        <div class="flex items-center justify-end space-x-3 pt-4 border-t border-slate-800 mt-6">
          <button
            type="button"
            :disabled="loading"
            class="px-4 py-2 rounded-lg text-sm font-medium text-slate-300 hover:text-white bg-slate-800 hover:bg-slate-700 transition border border-slate-700"
            @click="emit('close')"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="loading"
            class="px-5 py-2 rounded-lg text-sm font-medium bg-cyan-600 hover:bg-cyan-500 text-white transition shadow-lg shadow-cyan-900/40 flex items-center space-x-2"
          >
            <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
            </svg>
            <span>{{ isEditing ? 'Save Changes' : 'Create Console' }}</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
