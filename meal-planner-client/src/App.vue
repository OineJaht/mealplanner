<!-- src/App.vue - Overlay Sidebar Approach -->
<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <!-- Sidebar Toggle Button - Always visible when authenticated -->
    <MainLayout>
      <router-view />
    </MainLayout>
    <button
        v-if="authStore.isAuthenticated"
        @click="sidebarOpen = !sidebarOpen"
        class="fixed top-4 left-4 z-50 bg-green-600 text-white p-2 rounded-md shadow-lg hover:bg-green-700 transition-colors"
        title="Toggle Sidebar"
    >
      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
      </svg>
    </button>

    <!-- Sidebar Overlay -->
    <div
        v-if="authStore.isAuthenticated && sidebarOpen"
        class="fixed inset-0 z-40 flex"
    >
      <!-- Overlay Background -->
      <div
          class="fixed inset-0 bg-black bg-opacity-50"
          @click="sidebarOpen = false"
      ></div>

      <!-- Sidebar Content -->
      <div class="relative w-64 bg-white shadow-xl h-full overflow-y-auto">
        <!-- Sidebar Header -->
        <div class="p-4 border-b border-gray-200 flex items-center justify-between">
          <h2 class="text-lg font-semibold text-gray-900">Navigation</h2>
          <button
              @click="sidebarOpen = false"
              class="p-1 rounded-md text-gray-400 hover:text-gray-600"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Navigation Links -->
        <nav class="p-4 space-y-2">
          <router-link
              to="/"
              @click="sidebarOpen = false"
              class="sidebar-link"
              :class="{ 'sidebar-link-active': $route.name === 'dashboard' }"
          >
            <svg class="sidebar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v6H8V5z" />
            </svg>
            Dashboard
          </router-link>

          <router-link
              to="/recipes"
              @click="sidebarOpen = false"
              class="sidebar-link"
              :class="{ 'sidebar-link-active': $route.path.startsWith('/recipes') }"
          >
            <svg class="sidebar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
            </svg>
            Recipes
          </router-link>

          <router-link
              to="/recipe-finder"
              @click="sidebarOpen = false"
              class="sidebar-link"
              :class="{ 'sidebar-link-active': $route.name === 'recipe-finder' }"
          >
            <svg class="sidebar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            Recipe Finder
          </router-link>

          <router-link
              to="/meal-plans"
              @click="sidebarOpen = false"
              class="sidebar-link"
              :class="{ 'sidebar-link-active': $route.path.startsWith('/meal-plans') }"
          >
            <svg class="sidebar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            Meal Plans
          </router-link>

          <router-link
              to="/shopping-lists"
              @click="sidebarOpen = false"
              class="sidebar-link"
              :class="{ 'sidebar-link-active': $route.path.startsWith('/shopping-lists') }"
          >
            <svg class="sidebar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5H21M7 13v8a2 2 0 002 2h6a2 2 0 002-2v-8m-8 0V9a2 2 0 012-2h4a2 2 0 012 2v4.01" />
            </svg>
            Shopping Lists
          </router-link>

          <router-link
              to="/pantry"
              @click="sidebarOpen = false"
              class="sidebar-link"
              :class="{ 'sidebar-link-active': $route.name === 'pantry' }"
          >
            <svg class="sidebar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
            </svg>
            Pantry
          </router-link>

          <router-link
              to="/ingredients"
              @click="sidebarOpen = false"
              class="sidebar-link"
              :class="{ 'sidebar-link-active': $route.name === 'ingredients' }"
          >
            <svg class="sidebar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
            </svg>
            Ingredients
          </router-link>
        </nav>

        <!-- User section -->
        <div class="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-200 bg-white">
          <div class="flex items-center space-x-3">
            <div class="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 truncate">User</p>
              <button
                  @click="logout"
                  class="text-xs text-gray-500 hover:text-red-600 transition-colors"
              >
                Sign out
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Top Navigation (your existing nav) -->
    <nav v-if="authStore.isAuthenticated" class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex">
            <div class="flex-shrink-0 flex items-center ml-12"> <!-- ml-12 to make room for sidebar button -->
              <router-link to="/" class="text-xl font-bold text-green-600">
                MealPlanner
              </router-link>
            </div>
            <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
              <router-link to="/" class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors" active-class="border-green-500 text-gray-900">Dashboard</router-link>
              <router-link to="/recipes" class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors" active-class="border-green-500 text-gray-900">Recipes</router-link>
              <router-link to="/meal-plans" class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors" active-class="border-green-500 text-gray-900">Meal Plans</router-link>
              <router-link to="/shopping-lists" class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors" active-class="border-green-500 text-gray-900">Shopping Lists</router-link>
              <router-link to="/pantry" class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors" active-class="border-green-500 text-gray-900">Pantry</router-link>
              <router-link to="/recipe-finder" class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors" active-class="border-green-500 text-gray-900">Recipe Finder</router-link>
            </div>
          </div>

          <!-- Mobile menu button -->
          <div class="sm:hidden flex items-center">
            <button @click="mobileMenuOpen = !mobileMenuOpen" class="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-green-500">
              <Bars3Icon v-if="!mobileMenuOpen" class="block h-6 w-6" />
              <XMarkIcon v-else class="block h-6 w-6" />
            </button>
          </div>

          <!-- User menu -->
          <div class="hidden sm:ml-6 sm:flex sm:items-center">
            <Menu as="div" class="ml-3 relative">
              <div>
                <MenuButton class="bg-white flex text-sm rounded-full focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">
                  <span class="sr-only">Open user menu</span>
                  <div class="h-8 w-8 rounded-full bg-green-500 flex items-center justify-center">
                    <UserIcon class="h-5 w-5 text-white" />
                  </div>
                </MenuButton>
              </div>
              <transition enter-active-class="transition ease-out duration-200" enter-from-class="transform opacity-0 scale-95" enter-to-class="transform opacity-100 scale-100" leave-active-class="transition ease-in duration-75" leave-from-class="transform opacity-100 scale-100" leave-to-class="transform opacity-0 scale-95">
                <MenuItems class="origin-top-right absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 bg-white ring-1 ring-black ring-opacity-5 focus:outline-none z-50">
                  <MenuItem v-slot="{ active }">
                    <button @click="logout" :class="[active ? 'bg-gray-100' : '', 'block px-4 py-2 text-sm text-gray-700 w-full text-left']">Sign out</button>
                  </MenuItem>
                </MenuItems>
              </transition>
            </Menu>
          </div>
        </div>
      </div>

      <!-- Mobile menu -->
      <div v-show="mobileMenuOpen" class="sm:hidden">
        <div class="pt-2 pb-3 space-y-1">
          <router-link to="/" class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800 block pl-3 pr-4 py-2 border-l-4 text-base font-medium" active-class="bg-green-50 border-green-500 text-green-700" @click="mobileMenuOpen = false">Dashboard</router-link>
          <router-link to="/recipes" class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800 block pl-3 pr-4 py-2 border-l-4 text-base font-medium" active-class="bg-green-50 border-green-500 text-green-700" @click="mobileMenuOpen = false">Recipes</router-link>
          <router-link to="/meal-plans" class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800 block pl-3 pr-4 py-2 border-l-4 text-base font-medium" active-class="bg-green-50 border-green-500 text-green-700" @click="mobileMenuOpen = false">Meal Plans</router-link>
          <router-link to="/shopping-lists" class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800 block pl-3 pr-4 py-2 border-l-4 text-base font-medium" active-class="bg-green-50 border-green-500 text-green-700" @click="mobileMenuOpen = false">Shopping Lists</router-link>
          <router-link to="/pantry" class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800 block pl-3 pr-4 py-2 border-l-4 text-base font-medium" active-class="bg-green-50 border-green-500 text-green-700" @click="mobileMenuOpen = false">Pantry</router-link>
          <router-link to="/recipe-finder" class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800 block pl-3 pr-4 py-2 border-l-4 text-base font-medium" active-class="bg-green-50 border-green-500 text-green-700" @click="mobileMenuOpen = false">Recipe Finder</router-link>
        </div>
        <div class="pt-4 pb-3 border-t border-gray-200">
          <div class="flex items-center px-4">
            <div class="flex-shrink-0">
              <div class="h-10 w-10 rounded-full bg-green-500 flex items-center justify-center">
                <UserIcon class="h-6 w-6 text-white" />
              </div>
            </div>
            <div class="ml-3">
              <div class="text-base font-medium text-gray-800">User</div>
              <div class="text-sm font-medium text-gray-500">user@example.com</div>
            </div>
          </div>
          <div class="mt-3 space-y-1">
            <button @click="logout" class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100 w-full text-left">Sign out</button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main content -->
    <main class="flex-1">
      <router-view />
    </main>

    <!-- Toast notifications container -->
    <div id="toast-container"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/vue';
import { Bars3Icon, XMarkIcon, UserIcon } from '@heroicons/vue/24/outline';
import { useAuthStore } from '@/stores/auth';

import MainLayout from './components/MainLayout.vue';




const router = useRouter();
const authStore = useAuthStore();
const mobileMenuOpen = ref(false);
const sidebarOpen = ref(false);

const logout = async () => {
  await authStore.logout();
  mobileMenuOpen.value = false;
  sidebarOpen.value = false;
  router.push('/login');
};

const route = useRoute();

onMounted(() => {
  authStore.checkAuth();
});
</script>

<style>
* {
  box-sizing: border-box;
}

html, body {
  margin: 0;
  padding: 0;
  height: 100%;
  font-family: Inter, system-ui, sans-serif;
}

#app {
  min-height: 100vh;
}

.sidebar-link {
  display: flex;
  align-items: center;
  padding: 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  border-radius: 0.375rem;
  color: #374151;
  text-decoration: none;
  transition: all 0.15s ease-in-out;
}

.sidebar-link:hover {
  background-color: #f3f4f6;
  color: #111827;
}

.sidebar-link-active {
  background-color: #ecfdf5;
  color: #047857;
}

.sidebar-icon {
  margin-right: 0.75rem;
  height: 1.25rem;
  width: 1.25rem;
  flex-shrink: 0;
}

.sidebar-link-active .sidebar-icon {
  color: #059669;
}
</style>