import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { ManufacturerMapLocationDto } from '../types';
import { mapApi, getErrorMessage } from '../services/api';
import { useToastStore } from './toastStore';

export const useMapStore = defineStore('map', () => {
  const locations = ref<ManufacturerMapLocationDto[]>([]);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const selectedLocation = ref<ManufacturerMapLocationDto | null>(null);

  const toastStore = useToastStore();

  async function fetchLocations() {
    loading.value = true;
    error.value = null;
    try {
      locations.value = await mapApi.getLocations();
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to load map locations', msg);
    } finally {
      loading.value = false;
    }
  }

  function selectLocation(location: ManufacturerMapLocationDto | null) {
    selectedLocation.value = location;
  }

  return {
    locations,
    loading,
    error,
    selectedLocation,
    fetchLocations,
    selectLocation
  };
});
