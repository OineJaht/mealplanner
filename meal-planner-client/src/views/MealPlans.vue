<!-- src/views/MealPlans.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Meal Plans</h1>
            <p class="mt-2 text-sm text-gray-600">
              Plan your meals and track your nutrition
            </p>
          </div>
          <router-link
              to="/meal-plans/create"
              class="btn-primary"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            New Meal Plan
          </router-link>
        </div>
      </div>

      <!-- Current Meal Plan -->
      <div v-if="mealPlansStore.currentMealPlan" class="bg-white shadow rounded-lg mb-6">
        <div class="px-4 py-5 sm:p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg leading-6 font-medium text-gray-900">Current Meal Plan</h3>
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
              Active
            </span>
          </div>

          <div class="border border-gray-200 rounded-lg p-4">
            <div class="flex items-center justify-between">
              <div>
                <h4 class="text-lg font-medium text-gray-900">
                  {{ mealPlansStore.currentMealPlan.name }}
                </h4>
                <p class="text-sm text-gray-500">
                  {{ formatDate(mealPlansStore.currentMealPlan.startDate) }} -
                  {{ formatDate(mealPlansStore.currentMealPlan.endDate) }}
                </p>
                <p class="text-sm text-gray-500 mt-1">
                  {{ mealPlansStore.currentMealPlan.mealPlanItems.length }} meals planned
                </p>
              </div>
              <div class="flex space-x-3">
                <router-link
                    :to="`/meal-plans/${mealPlansStore.currentMealPlan.id}`"
                    class="btn-secondary"
                >
                  View Details
                </router-link>
                <button
                    @click="generateShoppingList(mealPlansStore.currentMealPlan)"
                    class="btn-primary"
                >
                  <ShoppingBagIcon class="-ml-1 mr-2 h-4 w-4" />
                  Generate Shopping List
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- All Meal Plans -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">All Meal Plans</h3>

          <div v-if="mealPlansStore.isLoading" class="space-y-4">
            <div v-for="i in 3" :key="i" class="animate-pulse">
              <div class="flex space-x-4 p-4 border border-gray-200 rounded-lg">
                <div class="flex-1 space-y-2">
                  <div class="h-4 bg-gray-200 rounded w-1/3"></div>
                  <div class="h-3 bg-gray-200 rounded w-1/2"></div>
                </div>
                <div class="h-8 bg-gray-200 rounded w-20"></div>
              </div>
            </div>
          </div>

          <div v-else-if="mealPlansStore.mealPlans.length === 0" class="text-center py-12">
            <CalendarIcon class="mx-auto h-12 w-12 text-gray-400" />
            <h3 class="mt-2 text-sm font-medium text-gray-900">No meal plans yet</h3>
            <p class="mt-1 text-sm text-gray-500">Get started by creating your first meal plan.</p>
            <div class="mt-6">
              <router-link
                  to="/meal-plans/create"
                  class="btn-primary"
              >
                <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
                Create Your First Meal Plan
              </router-link>
            </div>
          </div>

          <div v-else class="space-y-4">
            <div
                v-for="mealPlan in mealPlansStore.mealPlans"
                :key="mealPlan.id"
                class="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors"
            >
              <div class="flex items-center justify-between">
                <div class="flex-1 min-w-0">
                  <h4 class="text-lg font-medium text-gray-900">
                    <router-link
                        :to="`/meal-plans/${mealPlan.id}`"
                        class="hover:text-green-600"
                    >
                      {{ mealPlan.name }}
                    </router-link>
                  </h4>
                  <p class="text-sm text-gray-500">
                    {{ formatDate(mealPlan.startDate) }} - {{ formatDate(mealPlan.endDate) }}
                  </p>
                  <div class="mt-2 flex items-center space-x-4 text-sm text-gray-500">
                    <span class="flex items-center">
                      <CalendarIcon class="h-4 w-4 mr-1" />
                      {{ getDurationDays(mealPlan.startDate, mealPlan.endDate) }} days
                    </span>
                  </div>
                </div>

                <div class="flex items-center space-x-3">
                  <span :class="[
                    'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    getMealPlanStatus(mealPlan).class
                  ]">
                    {{ getMealPlanStatus(mealPlan).text }}
                  </span>

                  <Menu as="div" class="relative">
                    <MenuButton class="flex items-center p-1 text-gray-400 hover:text-gray-600">
                      <EllipsisVerticalIcon class="h-5 w-5" />
                    </MenuButton>
                    <transition
                        enter-active-class="transition ease-out duration-100"
                        enter-from-class="transform opacity-0 scale-95"
                        enter-to-class="transform opacity-100 scale-100"
                        leave-active-class="transition ease-in duration-75"
                        leave-from-class="transform opacity-100 scale-100"
                        leave-to-class="transform opacity-0 scale-95"
                    >
                      <MenuItems class="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-md bg-white py-1 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none">
                        <MenuItem v-slot="{ active }">
                          <router-link
                              :to="`/meal-plans/${mealPlan.id}`"
                              :class="[active ? 'bg-gray-100' : '', 'block px-4 py-2 text-sm text-gray-700']"
                          >
                            View Details
                          </router-link>
                        </MenuItem>
                        <MenuItem v-slot="{ active }">
                          <button
                              @click="generateShoppingList(mealPlan)"
                              :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                          >
                            Generate Shopping List
                          </button>
                        </MenuItem>
                        <MenuItem v-slot="{ active }">
                          <button
                              @click="deleteMealPlan(mealPlan)"
                              :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-red-600']"
                          >
                            Delete Meal Plan
                          </button>
                        </MenuItem>
                      </MenuItems>
                    </transition>
                  </Menu>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toastification';
import { format, differenceInDays } from 'date-fns';
import {
  PlusIcon,
  CalendarIcon,
  ShoppingBagIcon,
  EllipsisVerticalIcon
} from '@heroicons/vue/24/outline';
import {
  Menu,
  MenuButton,
  MenuItem,
  MenuItems
} from '@headlessui/vue';

import { useMealPlansStore } from '@/stores/mealPlans';
import { useShoppingListsStore } from '@/stores/shoppingLists';
import type { MealPlan } from '@/types';

const router = useRouter();
const toast = useToast();
const mealPlansStore = useMealPlansStore();
const shoppingListsStore = useShoppingListsStore();

// Methods
const formatDate = (dateString: string) => {
  return format(new Date(dateString), 'MMM d, yyyy');
};

const getDurationDays = (startDate: string, endDate: string) => {
  return differenceInDays(new Date(endDate), new Date(startDate)) + 1;
};

const getMealPlanStatus = (mealPlan: MealPlan) => {
  const today = new Date();
  const start = new Date(mealPlan.startDate);
  const end = new Date(mealPlan.endDate);

  if (today < start) {
    return { text: 'Upcoming', class: 'bg-blue-100 text-blue-800' };
  } else if (today >= start && today <= end) {
    return { text: 'Active', class: 'bg-green-100 text-green-800' };
  } else {
    return { text: 'Completed', class: 'bg-gray-100 text-gray-800' };
  }
};

const generateShoppingList = async (mealPlan: MealPlan) => {
  const result = await shoppingListsStore.createFromMealPlan(
      mealPlan.id,
      `Shopping List - ${mealPlan.name}`
  );

  if (result) {
    toast.success('Shopping list generated successfully');
    router.push(`/shopping-lists/${result.id}`);
  } else {
    toast.error('Failed to generate shopping list');
  }
};

const deleteMealPlan = async (mealPlan: MealPlan) => {
  if (confirm(`Are you sure you want to delete "${mealPlan.name}"?`)) {
    // This would need to be implemented in the store
    toast.info('Delete functionality not yet implemented');
  }
};

// Lifecycle
onMounted(async () => {
  await Promise.all([
    mealPlansStore.fetchMealPlans(),
    mealPlansStore.fetchCurrentMealPlan()
  ]);
});
</script>