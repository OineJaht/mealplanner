<!-- src/components/MainLayout.vue -->
<template>
  <div class="h-screen flex overflow-hidden bg-gray-50">
    <!-- Sidebar -->
    <div class="w-64 flex-shrink-0">
      <Sidebar />
    </div>

    <!-- Main content area -->
    <div class="flex-1 overflow-hidden flex flex-col">
      <!-- Top header bar -->
      <header class="bg-white shadow-sm border-b border-gray-200 px-6 py-4">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-2xl font-semibold text-gray-900">{{ pageTitle }}</h1>
            <p v-if="pageSubtitle" class="text-sm text-gray-600 mt-1">{{ pageSubtitle }}</p>
          </div>

          <!-- Header actions slot -->
          <div class="flex items-center space-x-4">
            <slot name="header-actions"></slot>

            <!-- Debug button (only in development) -->
            <button
                v-if="isDevelopment"
                @click="testApiConnection"
                class="bg-yellow-100 text-yellow-800 px-3 py-1 rounded text-sm hover:bg-yellow-200 transition-colors"
                :disabled="isTestingApi"
            >
              {{ isTestingApi ? 'Testing...' : 'Test API' }}
            </button>
          </div>
        </div>
      </header>

      <!-- Main content -->
      <main class="flex-1 overflow-y-auto p-6">
        <div class="max-w-7xl mx-auto">
          <slot></slot>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useToast } from 'vue-toastification';
import Sidebar from './Sidebar.vue';
import { ApiService } from '@/services/api';

const route = useRoute();
const toast = useToast();

const isTestingApi = ref(false);
const isDevelopment = computed(() => import.meta.env.DEV);

const pageTitle = computed(() => {
  const routeTitleMap: Record<string, string> = {
    'dashboard': 'Dashboard',
    'recipes': 'Recipes',
    'recipe-create': 'Create Recipe',
    'recipe-edit': 'Edit Recipe',
    'recipe-detail': 'Recipe Details',
    'recipe-finder': 'Recipe Finder',
    'meal-plans': 'Meal Plans',
    'meal-plan-create': 'Create Meal Plan',
    'meal-plan-detail': 'Meal Plan Details',
    'shopping-lists': 'Shopping Lists',
    'shopping-list-detail': 'Shopping List Details',
    'pantry': 'Pantry',
    'ingredients': 'Ingredients',
  };

  return routeTitleMap[route.name as string] || 'Meal Planner';
});

const pageSubtitle = computed(() => {
  const routeSubtitleMap: Record<string, string> = {
    'dashboard': 'Overview of your meal planning',
    'recipes': 'Manage your recipe collection',
    'recipe-create': 'Add a new recipe to your collection',
    'recipe-edit': 'Update recipe details',
    'recipe-finder': 'Find recipes based on available ingredients',
    'meal-plans': 'Plan your meals for the week',
    'meal-plan-create': 'Create a new meal plan',
    'shopping-lists': 'Manage your shopping lists',
    'pantry': 'Track your ingredient inventory',
    'ingredients': 'Browse and manage ingredients',
  };

  return routeSubtitleMap[route.name as string] || '';
});

const testApiConnection = async () => {
  isTestingApi.value = true;
  try {
    await ApiService.testConnection();
    toast.success('API connection successful! Check console for details.');
  } catch (error) {
    toast.error('API connection failed! Check console for details.');
  } finally {
    isTestingApi.value = false;
  }
};
</script>

<style scoped>
/* Custom scrollbar for main content */
main::-webkit-scrollbar {
  width: 6px;
}

main::-webkit-scrollbar-track {
  background: #f1f1f1;
}

main::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

main::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}
</style>