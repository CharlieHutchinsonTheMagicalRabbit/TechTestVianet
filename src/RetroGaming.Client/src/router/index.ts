import { createRouter, createWebHistory } from 'vue-router';
import ConsolesView from '../views/ConsolesView.vue';
import ManufacturersView from '../views/ManufacturersView.vue';
import MapView from '../views/MapView.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/consoles'
    },
    {
      path: '/consoles',
      name: 'consoles',
      component: ConsolesView
    },
    {
      path: '/manufacturers',
      name: 'manufacturers',
      component: ManufacturersView
    },
    {
      path: '/map',
      name: 'map',
      component: MapView
    }
  ]
});

export default router;
