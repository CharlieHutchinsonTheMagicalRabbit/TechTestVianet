import { defineStore } from 'pinia';
import { ref } from 'vue';
import type {
  ManufacturerDto,
  ManufacturerDetailDto,
  CreateManufacturerRequest,
  UpdateManufacturerRequest
} from '../types';
import { manufacturersApi, getErrorMessage } from '../services/api';
import { useToastStore } from './toastStore';

export const useManufacturerStore = defineStore('manufacturer', () => {
  const manufacturers = ref<ManufacturerDto[]>([]);
  const currentManufacturer = ref<ManufacturerDetailDto | null>(null);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);

  const toastStore = useToastStore();

  async function fetchManufacturers() {
    loading.value = true;
    error.value = null;
    try {
      manufacturers.value = await manufacturersApi.getAll();
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to load manufacturers', msg);
    } finally {
      loading.value = false;
    }
  }

  async function fetchManufacturerById(id: string) {
    loading.value = true;
    error.value = null;
    try {
      currentManufacturer.value = await manufacturersApi.getById(id);
      return currentManufacturer.value;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to load manufacturer details', msg);
      return null;
    } finally {
      loading.value = false;
    }
  }

  async function createManufacturer(data: CreateManufacturerRequest): Promise<ManufacturerDto | null> {
    loading.value = true;
    error.value = null;
    try {
      const created = await manufacturersApi.create(data);
      manufacturers.value.push(created);
      toastStore.success('Success', `Manufacturer "${created.name}" created successfully.`);
      return created;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to create manufacturer', msg);
      return null;
    } finally {
      loading.value = false;
    }
  }

  async function updateManufacturer(id: string, data: UpdateManufacturerRequest): Promise<ManufacturerDto | null> {
    loading.value = true;
    error.value = null;
    try {
      const updated = await manufacturersApi.update(id, data);
      const index = manufacturers.value.findIndex(m => m.id === id);
      if (index !== -1) {
        manufacturers.value[index] = updated;
      }
      if (currentManufacturer.value?.id === id) {
        currentManufacturer.value = {
          ...currentManufacturer.value,
          ...updated
        };
      }
      toastStore.success('Success', `Manufacturer "${updated.name}" updated successfully.`);
      return updated;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to update manufacturer', msg);
      return null;
    } finally {
      loading.value = false;
    }
  }

  async function deleteManufacturer(id: string): Promise<boolean> {
    loading.value = true;
    error.value = null;
    try {
      await manufacturersApi.delete(id);
      manufacturers.value = manufacturers.value.filter(m => m.id !== id);
      if (currentManufacturer.value?.id === id) {
        currentManufacturer.value = null;
      }
      toastStore.success('Success', 'Manufacturer deleted successfully.');
      return true;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Cannot Delete Manufacturer', msg);
      return false;
    } finally {
      loading.value = false;
    }
  }

  return {
    manufacturers,
    currentManufacturer,
    loading,
    error,
    fetchManufacturers,
    fetchManufacturerById,
    createManufacturer,
    updateManufacturer,
    deleteManufacturer
  };
});
