import { defineStore } from 'pinia';
import { ref } from 'vue';
import type {
  ConsoleDto,
  CreateConsoleRequest,
  UpdateConsoleRequest,
  ConsoleSearchQuery
} from '../types';
import { consolesApi, getErrorMessage } from '../services/api';
import { useToastStore } from './toastStore';

export const useConsoleStore = defineStore('console', () => {
  const consoles = ref<ConsoleDto[]>([]);
  const currentConsole = ref<ConsoleDto | null>(null);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const query = ref<ConsoleSearchQuery>({
    searchTerm: '',
    manufacturerId: undefined,
    generation: undefined,
    isDiscontinued: undefined,
    sortBy: 'Name',
    sortDescending: false
  });

  const toastStore = useToastStore();

  async function fetchConsoles(customQuery?: ConsoleSearchQuery) {
    loading.value = true;
    error.value = null;
    const activeQuery = customQuery ?? query.value;
    try {
      consoles.value = await consolesApi.getAll(activeQuery);
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to load consoles', msg);
    } finally {
      loading.value = false;
    }
  }

  async function fetchConsoleById(id: string) {
    loading.value = true;
    error.value = null;
    try {
      currentConsole.value = await consolesApi.getById(id);
      return currentConsole.value;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to load console details', msg);
      return null;
    } finally {
      loading.value = false;
    }
  }

  async function createConsole(data: CreateConsoleRequest): Promise<ConsoleDto | null> {
    loading.value = true;
    error.value = null;
    try {
      const created = await consolesApi.create(data);
      consoles.value.push(created);
      toastStore.success('Success', `Console "${created.name}" created successfully.`);
      return created;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to create console', msg);
      return null;
    } finally {
      loading.value = false;
    }
  }

  async function updateConsole(id: string, data: UpdateConsoleRequest): Promise<ConsoleDto | null> {
    loading.value = true;
    error.value = null;
    try {
      const updated = await consolesApi.update(id, data);
      const index = consoles.value.findIndex(c => c.id === id);
      if (index !== -1) {
        consoles.value[index] = updated;
      }
      if (currentConsole.value?.id === id) {
        currentConsole.value = updated;
      }
      toastStore.success('Success', `Console "${updated.name}" updated successfully.`);
      return updated;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to update console', msg);
      return null;
    } finally {
      loading.value = false;
    }
  }

  async function deleteConsole(id: string): Promise<boolean> {
    loading.value = true;
    error.value = null;
    try {
      await consolesApi.delete(id);
      consoles.value = consoles.value.filter(c => c.id !== id);
      if (currentConsole.value?.id === id) {
        currentConsole.value = null;
      }
      toastStore.success('Success', 'Console deleted successfully.');
      return true;
    } catch (err) {
      const msg = getErrorMessage(err);
      error.value = msg;
      toastStore.error('Failed to delete console', msg);
      return false;
    } finally {
      loading.value = false;
    }
  }

  function setSort(sortBy: string) {
    if (query.value.sortBy === sortBy) {
      query.value.sortDescending = !query.value.sortDescending;
    } else {
      query.value.sortBy = sortBy;
      query.value.sortDescending = false;
    }
    fetchConsoles();
  }

  return {
    consoles,
    currentConsole,
    loading,
    error,
    query,
    fetchConsoles,
    fetchConsoleById,
    createConsole,
    updateConsole,
    deleteConsole,
    setSort
  };
});
