<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import * as L from 'leaflet';
import { useMapStore } from '../stores/mapStore';
import { useConsoleStore } from '../stores/consoleStore';
import { useManufacturerStore } from '../stores/manufacturerStore';
import type { ManufacturerMapLocationDto } from '../types';
import ManufacturerDetailModal from '../components/manufacturers/ManufacturerDetailModal.vue';

const router = useRouter();
const mapStore = useMapStore();
const consoleStore = useConsoleStore();
const manufacturerStore = useManufacturerStore();

const mapContainer = ref<HTMLElement | null>(null);
let map: L.Map | null = null;
const markers = new Map<string, L.Marker>();

const isDetailModalOpen = ref(false);

onMounted(async () => {
  await mapStore.fetchLocations();
  await nextTick();
  initMap();
});

onBeforeUnmount(() => {
  if (map) {
    map.remove();
    map = null;
  }
});

function createCustomIcon(consoleCount: number) {
  return L.divIcon({
    className: 'custom-retro-marker',
    html: `
      <div class="relative flex items-center justify-center">
        <span class="animate-ping absolute inline-flex h-8 w-8 rounded-full bg-cyan-400 opacity-40"></span>
        <div class="relative flex items-center justify-center w-8 h-8 rounded-full bg-slate-900 border-2 border-cyan-400 shadow-[0_0_12px_rgba(6,182,212,0.8)] text-cyan-300 font-bold text-xs">
          ${consoleCount}
        </div>
      </div>
    `,
    iconSize: [32, 32],
    iconAnchor: [16, 16],
    popupAnchor: [0, -18]
  });
}

function initMap() {
  if (!mapContainer.value || map) return;

  // Global centered map
  map = L.map(mapContainer.value, {
    center: [25, 0],
    zoom: 2,
    minZoom: 2,
    maxZoom: 18,
    zoomControl: false
  });

  L.control.zoom({ position: 'bottomright' }).addTo(map);

  // High performance dark tiles
  L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
    subdomains: 'abcd',
    maxZoom: 20
  }).addTo(map);

  renderMarkers();
}

function renderMarkers() {
  if (!map) return;

  markers.forEach(m => m.remove());
  markers.clear();

  mapStore.locations.forEach((loc) => {
    const customIcon = createCustomIcon(loc.consoleCount);
    const marker = L.marker([loc.latitude, loc.longitude], { icon: customIcon }).addTo(map!);

    const popupHtml = document.createElement('div');
    popupHtml.className = 'p-1';
    popupHtml.innerHTML = `
      <div class="space-y-2">
        <div class="border-b border-slate-700 pb-2">
          <h4 class="text-base font-bold text-white">${loc.name}</h4>
          <p class="text-xs text-slate-400 flex items-center gap-1 mt-0.5">
            <span>📍</span>
            <span>${loc.locationString}</span>
          </p>
        </div>
        <div class="flex items-center justify-between text-xs py-1">
          <span class="text-slate-400">Total Consoles:</span>
          <span class="font-bold text-cyan-400 px-2 py-0.5 rounded bg-cyan-950/60 border border-cyan-800">${loc.consoleCount} systems</span>
        </div>
        <div class="flex gap-2 pt-1">
          <button id="view-consoles-${loc.id}" class="flex-1 py-1.5 px-2 bg-cyan-600 hover:bg-cyan-500 text-white rounded text-xs font-semibold text-center transition">
            View Consoles
          </button>
          <button id="view-details-${loc.id}" class="flex-1 py-1.5 px-2 bg-slate-800 hover:bg-slate-700 text-slate-200 rounded text-xs font-semibold text-center transition border border-slate-700">
            Details
          </button>
        </div>
      </div>
    `;

    marker.bindPopup(popupHtml, { maxWidth: 280 });

    marker.on('popupopen', () => {
      const btnConsoles = document.getElementById(`view-consoles-${loc.id}`);
      if (btnConsoles) {
        btnConsoles.onclick = () => {
          consoleStore.query.manufacturerId = loc.id;
          router.push('/consoles');
        };
      }

      const btnDetails = document.getElementById(`view-details-${loc.id}`);
      if (btnDetails) {
        btnDetails.onclick = async () => {
          isDetailModalOpen.value = true;
          await manufacturerStore.fetchManufacturerById(loc.id);
        };
      }
    });

    markers.set(loc.id, marker);
  });
}

function flyToLocation(loc: ManufacturerMapLocationDto) {
  mapStore.selectLocation(loc);
  if (map) {
    map.flyTo([loc.latitude, loc.longitude], 6, { duration: 1.5 });
    const marker = markers.get(loc.id);
    if (marker) {
      setTimeout(() => {
        marker.openPopup();
      }, 1500);
    }
  }
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h1 class="text-3xl font-extrabold tracking-tight text-white flex items-center gap-3">
          <span class="p-2 rounded-xl bg-amber-500/10 border border-amber-500/20 text-amber-400">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3.055 11H5a2 2 0 012 2v1a2 2 0 002 2 2 2 0 012 2v2.945M8 3.935V5.5A2.5 2.5 0 0010.5 8h.5a2 2 0 012 2 2 2 0 104 0 2 2 0 012-2h1.064M15 20.488V18a2 2 0 012-2h3.064M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </span>
          Global Hardware Origins
        </h1>
        <p class="text-slate-400 text-sm mt-1">
          Geographic visualization of gaming giants and manufacturer headquarters across the globe.
        </p>
      </div>

      <div class="flex items-center space-x-2 text-xs text-slate-400">
        <span class="inline-block w-3 h-3 rounded-full bg-cyan-400 animate-pulse"></span>
        <span>Pins display total registered consoles</span>
      </div>
    </div>

    <!-- Map & Sidebar Layout -->
    <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
      <!-- Interactive Leaflet Map Container -->
      <div class="lg:col-span-3 bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-2xl relative min-h-[500px] h-[600px]">
        <div ref="mapContainer" class="w-full h-full z-0"></div>

        <div v-if="mapStore.loading" class="absolute inset-0 bg-slate-950/80 backdrop-blur-sm flex items-center justify-center z-10">
          <svg class="animate-spin h-8 w-8 text-amber-400 mr-3" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
          </svg>
          <span class="font-medium text-slate-200">Plotting headquarters...</span>
        </div>
      </div>

      <!-- Quick Location Selector Sidebar -->
      <div class="bg-slate-900/90 border border-slate-800 rounded-2xl p-5 shadow-xl backdrop-blur flex flex-col h-[600px]">
        <h3 class="text-sm font-bold uppercase tracking-wider text-slate-400 mb-3 flex items-center justify-between">
          <span>Headquarters</span>
          <span class="text-xs px-2 py-0.5 rounded bg-slate-800 text-amber-400 font-mono">
            {{ mapStore.locations.length }}
          </span>
        </h3>

        <div class="flex-1 overflow-y-auto space-y-2 pr-1">
          <button
            v-for="loc in mapStore.locations"
            :key="loc.id"
            type="button"
            class="w-full text-left p-3 rounded-xl border transition flex items-center justify-between group"
            :class="mapStore.selectedLocation?.id === loc.id ? 'bg-cyan-500/10 border-cyan-500/50 shadow-lg shadow-cyan-900/20' : 'bg-slate-800/40 border-slate-800 hover:border-slate-700 hover:bg-slate-800/80'"
            @click="flyToLocation(loc)"
          >
            <div>
              <p class="text-sm font-bold text-slate-200 group-hover:text-cyan-400 transition">
                {{ loc.name }}
              </p>
              <p class="text-xs text-slate-400 mt-0.5">
                {{ loc.locationString }}
              </p>
            </div>

            <div class="text-right">
              <span class="inline-flex items-center justify-center px-2 py-1 rounded-lg bg-slate-900 text-xs font-bold text-amber-400 border border-slate-700">
                {{ loc.consoleCount }}
              </span>
            </div>
          </button>
        </div>
      </div>
    </div>

    <!-- Manufacturer Detail Modal -->
    <ManufacturerDetailModal
      :is-open="isDetailModalOpen"
      :manufacturer="manufacturerStore.currentManufacturer"
      :loading="manufacturerStore.loading"
      @close="isDetailModalOpen = false"
    />
  </div>
</template>
