<!-- src/components/Sidebar.vue -->
<template>
  <div class="h-full bg-white shadow-lg border-r border-gray-200">
    <!-- Logo/Brand -->
    <div class="p-4 border-b border-gray-200">
      <h1 class="text-xl font-bold text-primary-600">Meal Planner</h1>
    </div>

    <!-- Navigation -->
    <nav class="mt-4">
      <div class="px-2 space-y-1">
        <!-- Dashboard -->
        <router-link
            to="/"
            class="nav-item"
            :class="{ 'nav-item-active': $route.name === 'dashboard' }"
        >
          <svg class="nav-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M8 5a2 2 0 012-2h4a2 2 0 012 2v6H8V5z" />
          </svg>
          Dashboard
        </router-link>

        <!-- Recipes -->
        <router-link
            to="/recipes"
            class="nav-item"
            :class="{ 'nav-item-active': $route.path.startsWith('/recipes') }"
        >
          <svg class="nav-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
          </svg>
          Recipes
        </router-link>

        <!-- Recipe Finder -->
        <router-link
            to="/recipe-finder"
            class="nav-item"
            :class="{ 'nav-item-active': $route.name === 'recipe-finder' }"
        >
          <svg class="nav-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          Recipe Finder
        </router-link>

        <!-- Meal Plans -->
        <router-link
            to="/meal-plans"
            class="nav-item"
            :class="{ 'nav-item-active': $route.path.startsWith('/meal-plans') }"
        >
          <svg class="nav-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          Meal Plans
        </router-link>

        <!-- Shopping Lists -->
        <router-link
            to="/shopping-lists"
            class="nav-item"
            :class="{ 'nav-item-active': $route.path.startsWith('/shopping-lists') }"
        >
          <svg class="nav-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5H21M7 13v8a2 2 0 002 2h6a2 2 0 002-2v-8m-8 0V9a2 2 0 012-2h4a2 2 0 012 2v4.01" />
          </svg>
          Shopping Lists
        </router-link>

        <!-- Pantry -->
        <router-link
            to="/pantry"
            class="nav-item"
            :class="{ 'nav-item-active': $route.name === 'pantry' }"
        >
          <svg class="nav-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
          </svg>
          Pantry
        </router-link>

        <!-- Ingredients -->
        <router-link
            to="/ingredients"
            class="nav-item"
            :class="{ 'nav-item-active': $route.name === 'ingredients' }"
        >
          <svg class="nav-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
          </svg>
          Ingredients
        </router-link>
      </div>
    </nav>

    <!-- User section at bottom -->
    <div class="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-200">
      <div class="flex items-center space-x-3">
        <div class="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center">
          <svg class="w-5 h-5 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
          </svg>
        </div>
        <div class="flex-1 min-w-0">
          <p class="text-sm font-medium text-gray-900 truncate">
            {{ user?.username || 'User' }}
          </p>
          <button
              @click="handleLogout"
              class="text-xs text-gray-500 hover:text-red-600 transition-colors"
          >
            Sign out
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const authStore = useAuthStore();

const user = computed(() => authStore.user);

const handleLogout = async () => {
  await authStore.logout();
  router.push('/login');
};
</script>

<style scoped>
.nav-item {
  display: flex;
  align-items: center;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  border-radius: 0.375rem;
  color: #4b5563;
  transition: all 0.15s ease-in-out;
  text-decoration: none;
}

.nav-item:hover {
  background-color: #f9fafb;
  color: #111827;
}

.nav-item-active {
  background-color: #ecfdf5;
  color: #047857;
  border-right: 2px solid #059669;
}

.nav-icon {
  margin-right: 0.75rem;
  height: 1.25rem;
  width: 1.25rem;
  flex-shrink: 0;
}

.nav-item-active .nav-icon {
  color: #059669;
}

/* Use CSS custom properties for primary colors if not defined */
:root {
  --primary-50: #ecfdf5;
  --primary-100: #dcfce7;
  --primary-600: #059669;
  --primary-700: #047857;
}

.text-primary-600 {
  color: var(--primary-600);
}

.bg-primary-100 {
  background-color: var(--primary-100);
}

.text-primary-700 {
  color: var(--primary-700);
}

.bg-primary-50 {
  background-color: var(--primary-50);
}

.border-primary-600 {
  border-color: var(--primary-600);
}
</style>