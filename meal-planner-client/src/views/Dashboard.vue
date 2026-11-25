<!-- src/views/Dashboard.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <h1 class="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p class="mt-2 text-sm text-gray-600">Welcome to your meal planning hub</p>
      </div>

      <!-- Quick stats -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <BookOpenIcon class="h-6 w-6 text-gray-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Total Recipes</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ recipesStore.recipes.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <CalendarIcon class="h-6 w-6 text-gray-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Meal Plans</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ mealPlansStore.mealPlans.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <ShoppingBagIcon class="h-6 w-6 text-gray-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Shopping Lists</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ shoppingListsStore.shoppingLists.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <ArchiveBoxIcon class="h-6 w-6 text-gray-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Pantry Items</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ pantryStore.pantryItems.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick actions -->
      <div class="bg-white shadow rounded-lg mb-8">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Quick Actions</h3>
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <router-link
                to="/recipes/create"
                class="relative group bg-white p-6 focus-within:ring-2 focus-within:ring-inset focus-within:ring-green-500 rounded-lg border border-gray-200 hover:border-green-300 transition-colors"
            >
              <div>
                <span class="rounded-lg inline-flex p-3 bg-green-50 text-green-700 ring-4 ring-white">
                  <PlusIcon class="h-6 w-6" />
                </span>
              </div>
              <div class="mt-4">
                <h3 class="text-lg font-medium text-gray-900">
                  <span class="absolute inset-0" aria-hidden="true"></span>
                  Create Recipe
                </h3>
                <p class="mt-2 text-sm text-gray-500">Add a new recipe to your collection</p>
              </div>
            </router-link>

            <router-link
                to="/meal-plans/create"
                class="relative group bg-white p-6 focus-within:ring-2 focus-within:ring-inset focus-within:ring-green-500 rounded-lg border border-gray-200 hover:border-green-300 transition-colors"
            >
              <div>
                <span class="rounded-lg inline-flex p-3 bg-blue-50 text-blue-700 ring-4 ring-white">
                  <CalendarDaysIcon class="h-6 w-6" />
                </span>
              </div>
              <div class="mt-4">
                <h3 class="text-lg font-medium text-gray-900">
                  <span class="absolute inset-0" aria-hidden="true"></span>
                  Plan Meals
                </h3>
                <p class="mt-2 text-sm text-gray-500">Create a new meal plan</p>
              </div>
            </router-link>

            <router-link
                to="/recipe-finder"
                class="relative group bg-white p-6 focus-within:ring-2 focus-within:ring-inset focus-within:ring-green-500 rounded-lg border border-gray-200 hover:border-green-300 transition-colors"
            >
              <div>
                <span class="rounded-lg inline-flex p-3 bg-purple-50 text-purple-700 ring-4 ring-white">
                  <MagnifyingGlassIcon class="h-6 w-6" />
                </span>
              </div>
              <div class="mt-4">
                <h3 class="text-lg font-medium text-gray-900">
                  <span class="absolute inset-0" aria-hidden="true"></span>
                  Find Recipes
                </h3>
                <p class="mt-2 text-sm text-gray-500">What can I cook with my pantry?</p>
              </div>
            </router-link>

            <router-link
                to="/pantry"
                class="relative group bg-white p-6 focus-within:ring-2 focus-within:ring-inset focus-within:ring-green-500 rounded-lg border border-gray-200 hover:border-green-300 transition-colors"
            >
              <div>
                <span class="rounded-lg inline-flex p-3 bg-orange-50 text-orange-700 ring-4 ring-white">
                  <ArchiveBoxIcon class="h-6 w-6" />
                </span>
              </div>
              <div class="mt-4">
                <h3 class="text-lg font-medium text-gray-900">
                  <span class="absolute inset-0" aria-hidden="true"></span>
                  Manage Pantry
                </h3>
                <p class="mt-2 text-sm text-gray-500">Update your ingredient inventory</p>
              </div>
            </router-link>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 gap-8 lg:grid-cols-2">
        <!-- Recent Recipes -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg leading-6 font-medium text-gray-900">Recent Recipes</h3>
              <router-link
                  to="/recipes"
                  class="text-sm font-medium text-green-600 hover:text-green-500"
              >
                View all
              </router-link>
            </div>
            <div v-if="recipesStore.isLoading" class="animate-pulse">
              <div class="space-y-3">
                <div class="h-4 bg-gray-200 rounded w-3/4"></div>
                <div class="h-4 bg-gray-200 rounded w-1/2"></div>
                <div class="h-4 bg-gray-200 rounded w-5/6"></div>
              </div>
            </div>
            <div v-else-if="recipesStore.recentRecipes.length === 0" class="text-center py-6">
              <BookOpenIcon class="mx-auto h-12 w-12 text-gray-400" />
              <h3 class="mt-2 text-sm font-medium text-gray-900">No recipes yet</h3>
              <p class="mt-1 text-sm text-gray-500">Get started by creating your first recipe.</p>
              <div class="mt-6">
                <router-link
                    to="/recipes/create"
                    class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
                >
                  <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
                  New Recipe
                </router-link>
              </div>
            </div>
            <ul v-else class="divide-y divide-gray-200">
              <li v-for="recipe in recipesStore.recentRecipes" :key="recipe.id" class="py-4 flex">
                <div class="flex-1 min-w-0">
                  <router-link :to="`/recipes/${recipe.id}`" class="focus:outline-none">
                    <p class="text-sm font-medium text-gray-900 hover:text-green-600">
                      {{ recipe.name }}
                    </p>
                    <p class="text-sm text-gray-500 truncate">{{ recipe.description }}</p>
                  </router-link>
                </div>
                <div class="flex-shrink-0 ml-4">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                    {{ recipe.prepTimeMinutes }}min
                  </span>
                </div>
              </li>
            </ul>
          </div>
        </div>

        <!-- Alerts & Notifications -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Alerts & Notifications</h3>

            <!-- Expiring items -->
            <div v-if="pantryStore.expiringItems.length > 0" class="rounded-md bg-yellow-50 p-4 mb-4">
              <div class="flex">
                <div class="flex-shrink-0">
                  <ExclamationTriangleIcon class="h-5 w-5 text-yellow-400" />
                </div>
                <div class="ml-3">
                  <h3 class="text-sm font-medium text-yellow-800">Items Expiring Soon</h3>
                  <div class="mt-2 text-sm text-yellow-700">
                    <p>{{ pantryStore.expiringItems.length }} items in your pantry are expiring within 7 days.</p>
                  </div>
                  <div class="mt-4">
                    <div class="-mx-2 -my-1.5 flex">
                      <router-link
                          to="/pantry"
                          class="bg-yellow-50 px-2 py-1.5 rounded-md text-sm font-medium text-yellow-800 hover:bg-yellow-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-yellow-50 focus:ring-yellow-600"
                      >
                        View Pantry
                      </router-link>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Low stock items -->
            <div v-if="pantryStore.lowStockItems.length > 0" class="rounded-md bg-red-50 p-4 mb-4">
              <div class="flex">
                <div class="flex-shrink-0">
                  <ExclamationCircleIcon class="h-5 w-5 text-red-400" />
                </div>
                <div class="ml-3">
                  <h3 class="text-sm font-medium text-red-800">Low Stock Alert</h3>
                  <div class="mt-2 text-sm text-red-700">
                    <p>{{ pantryStore.lowStockItems.length }} items are running low in your pantry.</p>
                  </div>
                  <div class="mt-4">
                    <div class="-mx-2 -my-1.5 flex">
                      <router-link
                          to="/pantry"
                          class="bg-red-50 px-2 py-1.5 rounded-md text-sm font-medium text-red-800 hover:bg-red-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-red-50 focus:ring-red-600"
                      >
                        Restock Items
                      </router-link>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- No alerts -->
            <div v-if="pantryStore.expiringItems.length === 0 && pantryStore.lowStockItems.length === 0" class="text-center py-6">
              <CheckCircleIcon class="mx-auto h-12 w-12 text-green-400" />
              <h3 class="mt-2 text-sm font-medium text-gray-900">All Good!</h3>
              <p class="mt-1 text-sm text-gray-500">No urgent alerts at the moment.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import {
  BookOpenIcon,
  CalendarIcon,
  ShoppingBagIcon,
  ArchiveBoxIcon,
  PlusIcon,
  CalendarDaysIcon,
  MagnifyingGlassIcon,
  ExclamationTriangleIcon,
  ExclamationCircleIcon,
  CheckCircleIcon
} from '@heroicons/vue/24/outline';

import { useRecipesStore } from '@/stores/recipes';
import { useMealPlansStore } from '@/stores/mealPlans';
import { useShoppingListsStore } from '@/stores/shoppingLists';
import { usePantryStore } from '@/stores/pantry';

const recipesStore = useRecipesStore();
const mealPlansStore = useMealPlansStore();
const shoppingListsStore = useShoppingListsStore();
const pantryStore = usePantryStore();

onMounted(async () => {
  // Load all necessary data for dashboard
  await Promise.all([
    recipesStore.fetchRecipes(),
    recipesStore.fetchRecentRecipes(),
    mealPlansStore.fetchMealPlans(),
    shoppingListsStore.fetchShoppingLists(),
    pantryStore.fetchPantryItems(),
    pantryStore.fetchExpiringItems(),
    pantryStore.fetchLowStockItems()
  ]);
});
</script>