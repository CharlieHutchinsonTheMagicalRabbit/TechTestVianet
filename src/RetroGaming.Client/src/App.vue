<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import ToastContainer from './components/common/ToastContainer.vue'
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 flex flex-col">
    <ToastContainer />
    <header class="border-b border-slate-800 bg-slate-900/80 backdrop-blur sticky top-0 z-40">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <div class="flex items-center space-x-3">
          <span class="text-2xl font-black bg-gradient-to-r from-cyan-400 via-fuchsia-500 to-amber-400 bg-clip-text text-transparent">
            RETRO VAULT
          </span>
          <span class="text-xs px-2 py-0.5 rounded bg-slate-800 text-slate-400 font-mono border border-slate-700">v1.0</span>
        </div>

        <nav class="flex space-x-1 sm:space-x-2">
          <RouterLink
            to="/consoles"
            class="px-3 py-2 rounded-md text-sm font-medium transition-colors"
            active-class="bg-cyan-500/20 text-cyan-400 border border-cyan-500/30"
          >
            Consoles
          </RouterLink>
          <RouterLink
            to="/manufacturers"
            class="px-3 py-2 rounded-md text-sm font-medium transition-colors"
            active-class="bg-fuchsia-500/20 text-fuchsia-400 border border-fuchsia-500/30"
          >
            Manufacturers
          </RouterLink>
          <RouterLink
            to="/map"
            class="px-3 py-2 rounded-md text-sm font-medium transition-colors"
            active-class="bg-amber-500/20 text-amber-400 border border-amber-500/30"
          >
            Map View
          </RouterLink>
        </nav>
      </div>
    </header>

    <main class="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <RouterView />
    </main>

    <footer class="border-t border-slate-800 py-4 text-center text-xs text-slate-500">
      Retro Gaming Consoles Management System &bull; Clean Architecture (.NET 10 + Vue 3)
    </footer>
  </div>
</template>
