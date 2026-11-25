<!-- src/views/MealPlanForm.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-3xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <nav class="flex" aria-label="Breadcrumb">
          <ol class="flex items-center space-x-4">
            <li>
              <router-link to="/meal-plans" class="text-gray-400 hover:text-gray-500">
                <ChevronLeftIcon class="flex-shrink-0 h-5 w-5" />
                <span class="ml-4 text-sm font-medium text-gray-500 hover:text-gray-700">
                  Meal Plans
                </span>
              </router-link>
            </li>
          </ol>
        </nav>
        <h1 class="mt-2 text-3xl font-bold text-gray-900">Create Meal Plan</h1>
        <p class="mt-2 text-sm text-gray-600">
          Plan your meals for the week ahead
        </p>
      </div>

      <!-- Form -->
      <div class="bg-white shadow rounded-lg">
        <form @submit.prevent="saveMealPlan">
          <div class="px-4 py-5 sm:p-6">
            <div class="space-y-6">
              <div>
                <label for="name" class="form-label">
                  Plan Name *
                </label>
                <input
                    id="name"
                    v-model="form.name"
                    type="text"
                    required
                    class="form-input"
                    placeholder="e.g., Week of January 15th"
                />
                <p class="mt-2 text-sm text-gray-500">
                  Give your meal plan a descriptive name
                </p>
              </div>

              <div class="grid grid-cols-1 gap-6 sm:grid-cols-2">
                <div>
                  <label for="startDate" class="form-label">
                    Start Date *
                  </label>
                  <input
                      id="startDate"
                      v-model="form.startDate"
                      type="date"
                      required
                      class="form-input"
                      @change="updateEndDate"
                  />
                </div>

                <div>
                  <label for="endDate" class="form-label">
                    End Date *
                  </label>
                  <input
                      id="endDate"
                      v-model="form.endDate"
                      type="date"
                      required
                      class="form-input"
                      :min="form.startDate"
                  />
                </div>
              </div>

              <div v-if="form.startDate && form.endDate" class="rounded-md bg-blue-50 p-4">
                <div class="flex">
                  <div class="flex-shrink-0">
                    <InformationCircleIcon class="h-5 w-5 text-blue-400" />
                  </div>
                  <div class="ml-3">
                    <h3 class="text-sm font-medium text-blue-800">
                      Plan Duration: {{ planDuration }} days
                    </h3>
                    <div class="mt-2 text-sm text-blue-700">
                      <p>You can add meals to your plan after creating it.</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
            <button
                type="submit"
                :disabled="mealPlansStore.isLoading"
                class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-green-600 text-base font-medium text-white hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50"
            >
              {{ mealPlansStore.isLoading ? 'Creating...' : 'Create Meal Plan' }}
            </button>
            <router-link
                to="/meal-plans"
                class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
            >
              Cancel
            </router-link>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toastification';
import { differenceInDays, addDays, format } from 'date-fns';
import { ChevronLeftIcon, InformationCircleIcon } from '@heroicons/vue/24/outline';

import { useMealPlansStore } from '@/stores/mealPlans';
import type { MealPlanCreateDto } from '@/types';

const router = useRouter();
const toast = useToast();
const mealPlansStore = useMealPlansStore();

// Form state
const form = reactive<MealPlanCreateDto>({
  name: '',
  startDate: format(new Date(), 'yyyy-MM-dd'),
  endDate: format(addDays(new Date(), 6), 'yyyy-MM-dd') // Default to 7 days
});

// Computed properties
const planDuration = computed(() => {
  if (form.startDate && form.endDate) {
    return differenceInDays(new Date(form.endDate), new Date(form.startDate)) + 1;
  }
  return 0;
});

// Methods
const updateEndDate = () => {
  if (form.startDate) {
    // Default to 7 days from start date
    const start = new Date(form.startDate);
    const defaultEnd = addDays(start, 6);
    form.endDate = format(defaultEnd, 'yyyy-MM-dd');
  }
};

const saveMealPlan = async () => {
  const result = await mealPlansStore.createMealPlan(form);

  if (result) {
    toast.success('Meal plan created successfully');
    router.push(`/meal-plans/${result.id}`);
  } else {
    toast.error('Failed to create meal plan');
  }
};
</script>