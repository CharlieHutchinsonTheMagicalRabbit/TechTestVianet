<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import type { ManufacturerDto, CreateManufacturerRequest, UpdateManufacturerRequest } from '../../types';

const props = defineProps<{
  isOpen: boolean;
  manufacturerToEdit: ManufacturerDto | null;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'close'): void;
  (e: 'save-create', data: CreateManufacturerRequest): void;
  (e: 'save-update', id: string, data: UpdateManufacturerRequest): void;
}>();

const currentYear = new Date().getFullYear();

const name = ref('');
const hqCity = ref('');
const hqCountry = ref('');
const foundedYear = ref<number>(1980);
const latitude = ref<number>(0);
const longitude = ref<number>(0);

const errors = ref<Record<string, string>>({});

const isEditing = computed(() => !!props.manufacturerToEdit);

watch(
  () => props.isOpen,
  (open) => {
    if (open) {
      errors.value = {};
      if (props.manufacturerToEdit) {
        name.value = props.manufacturerToEdit.name;
        hqCity.value = props.manufacturerToEdit.hqCity;
        hqCountry.value = props.manufacturerToEdit.hqCountry;
        foundedYear.value = props.manufacturerToEdit.foundedYear;
        latitude.value = props.manufacturerToEdit.latitude;
        longitude.value = props.manufacturerToEdit.longitude;
      } else {
        name.value = '';
        hqCity.value = '';
        hqCountry.value = '';
        foundedYear.value = 1980;
        latitude.value = 0;
        longitude.value = 0;
      }
    }
  }
);

function validate(): boolean {
  errors.value = {};

  if (!name.value.trim()) {
    errors.value.name = 'Manufacturer name is required.';
  } else if (name.value.length > 100) {
    errors.value.name = 'Name cannot exceed 100 characters.';
  }

  if (!hqCity.value.trim()) {
    errors.value.hqCity = 'HQ City is required.';
  } else if (hqCity.value.length > 100) {
    errors.value.hqCity = 'HQ City cannot exceed 100 characters.';
  }

  if (!hqCountry.value.trim()) {
    errors.value.hqCountry = 'HQ Country is required.';
  } else if (hqCountry.value.length > 100) {
    errors.value.hqCountry = 'HQ Country cannot exceed 100 characters.';
  }

  if (!foundedYear.value || foundedYear.value < 1800 || foundedYear.value > currentYear) {
    errors.value.foundedYear = `Founded year must be between 1800 and ${currentYear}.`;
  }

  if (latitude.value === undefined || latitude.value === null || latitude.value < -90 || latitude.value > 90) {
    errors.value.latitude = 'Latitude must be between -90 and 90.';
  }

  if (longitude.value === undefined || longitude.value === null || longitude.value < -180 || longitude.value > 180) {
    errors.value.longitude = 'Longitude must be between -180 and 180.';
  }

  return Object.keys(errors.value).length === 0;
}

function handleSubmit() {
  if (!validate()) return;

  const payload = {
    name: name.value.trim(),
    hqCity: hqCity.value.trim(),
    hqCountry: hqCountry.value.trim(),
    foundedYear: foundedYear.value,
    latitude: Number(latitude.value),
    longitude: Number(longitude.value)
  };

  if (isEditing.value && props.manufacturerToEdit) {
    emit('save-update', props.manufacturerToEdit.id, payload);
  } else {
    emit('save-create', payload);
  }
}
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/75 backdrop-blur-sm overflow-y-auto">
    <div class="bg-slate-900 border border-slate-700 rounded-xl shadow-2xl max-w-lg w-full p-6 text-slate-100 my-8">
      <div class="flex items-center justify-between pb-4 border-b border-slate-800 mb-5">
        <h3 class="text-xl font-bold bg-gradient-to-r from-fuchsia-400 to-amber-400 bg-clip-text text-transparent">
          {{ isEditing ? 'Edit Manufacturer' : 'Add New Manufacturer' }}
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
        <!-- Name -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Company Name *
          </label>
          <input
            v-model="name"
            type="text"
            maxlength="100"
            placeholder="e.g. Sega Corporation"
            class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-fuchsia-500"
            :class="{ 'border-rose-500': errors.name }"
          />
          <p v-if="errors.name" class="text-rose-400 text-xs mt-1">{{ errors.name }}</p>
        </div>

        <!-- Grid: City & Country -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
              HQ City *
            </label>
            <input
              v-model="hqCity"
              type="text"
              maxlength="100"
              placeholder="e.g. Tokyo"
              class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-fuchsia-500"
              :class="{ 'border-rose-500': errors.hqCity }"
            />
            <p v-if="errors.hqCity" class="text-rose-400 text-xs mt-1">{{ errors.hqCity }}</p>
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
              HQ Country *
            </label>
            <input
              v-model="hqCountry"
              type="text"
              maxlength="100"
              placeholder="e.g. Japan"
              class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-fuchsia-500"
              :class="{ 'border-rose-500': errors.hqCountry }"
            />
            <p v-if="errors.hqCountry" class="text-rose-400 text-xs mt-1">{{ errors.hqCountry }}</p>
          </div>
        </div>

        <!-- Founded Year -->
        <div>
          <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
            Founded Year *
          </label>
          <input
            v-model.number="foundedYear"
            type="number"
            :min="1800"
            :max="currentYear"
            class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-fuchsia-500"
            :class="{ 'border-rose-500': errors.foundedYear }"
          />
          <p v-if="errors.foundedYear" class="text-rose-400 text-xs mt-1">{{ errors.foundedYear }}</p>
        </div>

        <!-- Geo-Coordinates -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
              Latitude (-90 to 90) *
            </label>
            <input
              v-model.number="latitude"
              type="number"
              step="0.0001"
              min="-90"
              max="90"
              placeholder="35.6264"
              class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-fuchsia-500"
              :class="{ 'border-rose-500': errors.latitude }"
            />
            <p v-if="errors.latitude" class="text-rose-400 text-xs mt-1">{{ errors.latitude }}</p>
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">
              Longitude (-180 to 180) *
            </label>
            <input
              v-model.number="longitude"
              type="number"
              step="0.0001"
              min="-180"
              max="180"
              placeholder="139.7402"
              class="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none focus:ring-2 focus:ring-fuchsia-500"
              :class="{ 'border-rose-500': errors.longitude }"
            />
            <p v-if="errors.longitude" class="text-rose-400 text-xs mt-1">{{ errors.longitude }}</p>
          </div>
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
            class="px-5 py-2 rounded-lg text-sm font-medium bg-fuchsia-600 hover:bg-fuchsia-500 text-white transition shadow-lg shadow-fuchsia-900/40 flex items-center space-x-2"
          >
            <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
            </svg>
            <span>{{ isEditing ? 'Save Changes' : 'Create Manufacturer' }}</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
