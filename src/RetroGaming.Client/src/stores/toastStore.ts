import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { ToastMessage } from '../types';

export const useToastStore = defineStore('toast', () => {
  const toasts = ref<ToastMessage[]>([]);

  function addToast(toast: Omit<ToastMessage, 'id'>) {
    const id = Math.random().toString(36).substring(2, 9);
    const newToast: ToastMessage = {
      id,
      timeout: 4000,
      ...toast
    };

    toasts.value.push(newToast);

    if (newToast.timeout && newToast.timeout > 0) {
      setTimeout(() => {
        removeToast(id);
      }, newToast.timeout);
    }
  }

  function removeToast(id: string) {
    toasts.value = toasts.value.filter(t => t.id !== id);
  }

  function success(title: string, message?: string) {
    addToast({ type: 'success', title, message });
  }

  function error(title: string, message?: string) {
    addToast({ type: 'error', title, message });
  }

  function info(title: string, message?: string) {
    addToast({ type: 'info', title, message });
  }

  function warning(title: string, message?: string) {
    addToast({ type: 'warning', title, message });
  }

  return {
    toasts,
    addToast,
    removeToast,
    success,
    error,
    info,
    warning
  };
});
