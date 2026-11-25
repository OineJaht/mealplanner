<!-- src/views/MealPlanDetail.vue - COMPLETE FIX -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <div class="flex items-center justify-between">
          <div>
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
            <h1 v-if="mealPlansStore.currentMealPlan" class="mt-2 text-3xl font-bold text-gray-900">
              {{ mealPlansStore.currentMealPlan.name }}
            </h1>
            <p v-if="mealPlansStore.currentMealPlan" class="mt-2 text-sm text-gray-600">
              {{ formatDate(mealPlansStore.currentMealPlan.startDate) }} -
              {{ formatDate(mealPlansStore.currentMealPlan.endDate) }}
            </p>
          </div>
          <div class="flex space-x-3">
            <button
                @click="generateShoppingList"
                :disabled="!mealPlansStore.currentMealPlan || !mealPlansStore.currentMealPlan.mealPlanItems || mealPlansStore.currentMealPlan.mealPlanItems.length === 0"
                class="btn-secondary"
            >
              <ShoppingBagIcon class="-ml-1 mr-2 h-4 w-4" />
              Generate Shopping List
            </button>
            <button
                @click="showAddMealModal = true"
                class="btn-primary"
            >
              <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
              Add Meal
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="mealPlansStore.isLoading" class="animate-pulse">
        <div class="bg-white shadow rounded-lg p-6">
          <div class="h-4 bg-gray-200 rounded w-1/4 mb-4"></div>
          <div class="space-y-3">
            <div class="h-3 bg-gray-200 rounded"></div>
            <div class="h-3 bg-gray-200 rounded w-5/6"></div>
          </div>
        </div>
      </div>

      <!-- Meal Plan Content -->
      <div v-else-if="mealPlansStore.currentMealPlan" class="space-y-6">
        <!-- Debug Info (Remove this later) -->
        <div class="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
          <h3 class="text-sm font-medium text-yellow-800">Debug Info:</h3>
          <p class="text-sm text-yellow-700">
            Current meal plan items: {{ mealPlansStore.currentMealPlan.mealPlanItems?.length || 0 }}
          </p>
          <p class="text-sm text-yellow-700">
            Grouped meals: {{ groupedMeals.length }} groups
          </p>
          <details class="mt-2">
            <summary class="text-sm font-medium text-yellow-800 cursor-pointer">Raw meal plan items</summary>
            <pre class="text-xs text-yellow-700 mt-2 whitespace-pre-wrap">{{ JSON.stringify(mealPlansStore.currentMealPlan.mealPlanItems, null, 2) }}</pre>
          </details>
        </div>

        <!-- Meal Plan Info -->
        <div class="bg-white shadow rounded-lg p-6">
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <div>
              <h3 class="text-sm font-medium text-gray-500">Duration</h3>
              <p class="mt-1 text-lg font-semibold text-gray-900">
                {{ getDurationDays(mealPlansStore.currentMealPlan.startDate, mealPlansStore.currentMealPlan.endDate) }} days
              </p>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-500">Total Meals</h3>
              <p class="mt-1 text-lg font-semibold text-gray-900">
                {{ mealPlansStore.currentMealPlan.mealPlanItems?.length || 0 }}
              </p>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-500">Status</h3>
              <span :class="[
                'inline-flex items-center px-2.5 py-0.5 rounded-full text-sm font-medium mt-1',
                getMealPlanStatus(mealPlansStore.currentMealPlan).class
              ]">
                {{ getMealPlanStatus(mealPlansStore.currentMealPlan).text }}
              </span>
            </div>
          </div>
        </div>

        <!-- Calendar View -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg font-medium text-gray-900">Meal Calendar</h3>
          </div>
          <div class="p-6">
            <!-- Show loading while no meals but we have data -->
            <div v-if="!mealPlansStore.currentMealPlan.mealPlanItems || mealPlansStore.currentMealPlan.mealPlanItems.length === 0" class="text-center py-12">
              <CalendarIcon class="mx-auto h-12 w-12 text-gray-400" />
              <h3 class="mt-2 text-sm font-medium text-gray-900">No meals planned yet</h3>
              <p class="mt-1 text-sm text-gray-500">Start adding meals to your plan.</p>
              <div class="mt-6">
                <button
                    @click="showAddMealModal = true"
                    class="btn-primary"
                >
                  <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
                  Add Your First Meal
                </button>
              </div>
            </div>

            <!-- Meals exist but groupedMeals is empty - debug case -->
            <div v-else-if="mealPlansStore.currentMealPlan.mealPlanItems.length > 0 && groupedMeals.length === 0" class="text-center py-12 bg-red-50 border border-red-200 rounded-lg">
              <ExclamationTriangleIcon class="mx-auto h-12 w-12 text-red-400" />
              <h3 class="mt-2 text-sm font-medium text-red-900">Data Loading Issue</h3>
              <p class="mt-1 text-sm text-red-700">
                We found {{ mealPlansStore.currentMealPlan.mealPlanItems.length }} meals but they're not displaying properly.
              </p>
              <button @click="debugLogData" class="mt-4 btn-secondary">Debug Log Data</button>
            </div>

            <!-- Normal meal display -->
            <div v-else class="space-y-6">
              <div v-for="day in groupedMeals" :key="day.date" class="border border-gray-200 rounded-lg">
                <div class="bg-gray-50 px-4 py-3 border-b border-gray-200">
                  <h4 class="text-lg font-medium text-gray-900">
                    {{ formatDateNice(day.date) }}
                  </h4>
                  <p class="text-sm text-gray-500">{{ day.meals.length }} meals planned</p>
                </div>

                <div class="p-4">
                  <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                    <div
                        v-for="meal in day.meals"
                        :key="meal.id"
                        class="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors"
                    >
                      <div class="flex items-start justify-between">
                        <div class="flex-1 min-w-0">
                          <h5 class="text-base font-medium text-gray-900 truncate">
                            {{ meal.recipeName || 'Unknown Recipe' }}
                          </h5>
                          <p class="text-sm text-gray-500 capitalize">{{ meal.mealType || 'Unknown Type' }}</p>
                          <p class="text-sm text-gray-500">{{ meal.servings || 0 }} servings</p>
                          <p class="text-xs text-gray-400">ID: {{ meal.id }}</p>
                        </div>

                        <Menu as="div" class="relative ml-3">
                          <MenuButton class="flex items-center p-1 text-gray-400 hover:text-gray-600">
                            <EllipsisVerticalIcon class="h-4 w-4" />
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
                                    :to="`/recipes/${meal.recipeId}`"
                                    :class="[active ? 'bg-gray-100' : '', 'block px-4 py-2 text-sm text-gray-700']"
                                >
                                  View Recipe
                                </router-link>
                              </MenuItem>
                              <MenuItem v-slot="{ active }">
                                <button
                                    @click="editMeal(meal)"
                                    :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                                >
                                  Edit Meal
                                </button>
                              </MenuItem>
                              <MenuItem v-slot="{ active }">
                                <button
                                    @click="deleteMeal(meal)"
                                    :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-red-600']"
                                >
                                  Remove Meal
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
      </div>

      <!-- Not Found -->
      <div v-else class="text-center py-12">
        <ExclamationTriangleIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">Meal plan not found</h3>
        <p class="mt-1 text-sm text-gray-500">The meal plan you're looking for doesn't exist.</p>
        <div class="mt-6">
          <router-link to="/meal-plans" class="btn-primary">
            Back to Meal Plans
          </router-link>
        </div>
      </div>
    </div>

    <!-- Add Meal Modal -->
    <div v-if="showAddMealModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Add Meal to Plan</h3>
        <form @submit.prevent="addMeal">
          <div class="space-y-4">
            <div>
              <label class="form-label">Recipe</label>
              <select
                  v-model="mealForm.recipeId"
                  required
                  class="form-input"
              >
                <option value="">Select a recipe...</option>
                <option v-for="recipe in availableRecipes" :key="recipe.id" :value="recipe.id">
                  {{ recipe.name }}
                </option>
              </select>
            </div>

            <div>
              <label class="form-label">Date</label>
              <input
                  v-model="mealForm.plannedDate"
                  type="date"
                  required
                  :min="mealPlansStore.currentMealPlan?.startDate?.split('T')[0]"
                  :max="mealPlansStore.currentMealPlan?.endDate?.split('T')[0]"
                  class="form-input"
              />
            </div>

            <div>
              <label class="form-label">Meal Type</label>
              <select
                  v-model="mealForm.mealType"
                  required
                  class="form-input"
              >
                <option value="">Select meal type...</option>
                <option value="breakfast">Breakfast</option>
                <option value="lunch">Lunch</option>
                <option value="dinner">Dinner</option>
                <option value="snack">Snack</option>
              </select>
            </div>

            <div>
              <label class="form-label">Servings</label>
              <input
                  v-model.number="mealForm.servings"
                  type="number"
                  min="1"
                  max="100"
                  required
                  class="form-input"
              />
            </div>
          </div>

          <div class="mt-6 flex justify-end space-x-3">
            <button
                type="button"
                @click="closeModal"
                class="btn-secondary"
            >
              Cancel
            </button>
            <button
                type="submit"
                :disabled="!isAddMealFormValid"
                class="btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Add Meal
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useToast } from 'vue-toastification';
import { format, differenceInDays } from 'date-fns';
import {
  PlusIcon,
  CalendarIcon,
  ShoppingBagIcon,
  ChevronLeftIcon,
  EllipsisVerticalIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline';
import {
  Menu,
  MenuButton,
  MenuItem,
  MenuItems
} from '@headlessui/vue';

import { useMealPlansStore } from '@/stores/mealPlans';
import { useRecipesStore } from '@/stores/recipes';
import { useShoppingListsStore } from '@/stores/shoppingLists';
import type { MealPlan, MealPlanItem, MealPlanItemCreateDto, Recipe } from '@/types';

const route = useRoute();
const router = useRouter();
const toast = useToast();
const mealPlansStore = useMealPlansStore();
const recipesStore = useRecipesStore();
const shoppingListsStore = useShoppingListsStore();

// Modal state
const showAddMealModal = ref(false);

// Available recipes
const availableRecipes = ref<Recipe[]>([]);

// Form state
const mealForm = reactive<MealPlanItemCreateDto>({
  mealPlanId: 0,
  recipeId: 0,
  plannedDate: '',
  mealType: '',
  servings: 1
});

// Form validation
const isAddMealFormValid = computed(() => {
  return (
      mealForm.recipeId > 0 &&
      mealForm.plannedDate.trim() !== '' &&
      mealForm.mealType.trim() !== '' &&
      mealForm.servings > 0
  );
});

// Computed properties
const groupedMeals = computed(() => {
  console.log('COMPUTED: groupedMeals called');

  if (!mealPlansStore.currentMealPlan) {
    console.log('COMPUTED: No current meal plan');
    return [];
  }

  const items = mealPlansStore.currentMealPlan.mealPlanItems;
  console.log('COMPUTED: Meal plan items:', items);

  if (!items || items.length === 0) {
    console.log('COMPUTED: No meal plan items');
    return [];
  }

  // Group meals by date
  const grouped = items.reduce((acc, meal) => {
    if (!meal || !meal.plannedDate) {
      console.log('COMPUTED: Skipping invalid meal:', meal);
      return acc;
    }

    // Extract date part from ISO string or use as-is
    let dateKey: string;
    if (meal.plannedDate.includes('T')) {
      dateKey = meal.plannedDate.split('T')[0];
    } else if (meal.plannedDate.includes(' ')) {
      dateKey = meal.plannedDate.split(' ')[0];
    } else {
      dateKey = meal.plannedDate;
    }

    console.log('COMPUTED: Processing meal for date:', dateKey, meal);

    if (!acc[dateKey]) {
      acc[dateKey] = [];
    }
    acc[dateKey].push(meal);
    return acc;
  }, {} as Record<string, MealPlanItem[]>);

  console.log('COMPUTED: Grouped meals by date:', grouped);

  // Convert to array and sort
  const result = Object.entries(grouped)
      .map(([date, meals]) => ({
        date,
        meals: meals.sort((a, b) => {
          const order = { breakfast: 1, lunch: 2, dinner: 3, snack: 4 };
          const aOrder = order[a.mealType?.toLowerCase() as keyof typeof order] || 5;
          const bOrder = order[b.mealType?.toLowerCase() as keyof typeof order] || 5;
          return aOrder - bOrder;
        })
      }))
      .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

  console.log('COMPUTED: Final grouped result:', result);
  return result;
});

// Methods
const formatDate = (dateString: string) => {
  return format(new Date(dateString), 'MMM d, yyyy');
};

const formatDateNice = (dateString: string) => {
  return format(new Date(dateString), 'EEEE, MMM d, yyyy');
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

const debugLogData = () => {
  console.log('=== DEBUG DATA DUMP ===');
  console.log('Current meal plan:', mealPlansStore.currentMealPlan);
  console.log('Meal plan items:', mealPlansStore.currentMealPlan?.mealPlanItems);
  console.log('Grouped meals:', groupedMeals.value);
  console.log('=======================');
};

const closeModal = () => {
  showAddMealModal.value = false;
  mealForm.recipeId = 0;
  mealForm.plannedDate = '';
  mealForm.mealType = '';
  mealForm.servings = 1;
};

const addMeal = async () => {
  if (!mealPlansStore.currentMealPlan) {
    toast.error('No meal plan selected');
    return;
  }

  if (!isAddMealFormValid.value) {
    toast.error('Please fill in all required fields');
    return;
  }

  try {
    mealForm.mealPlanId = mealPlansStore.currentMealPlan.id;

    // Ensure date is in proper format
    let plannedDate = mealForm.plannedDate;
    if (plannedDate && !plannedDate.includes('T')) {
      plannedDate = new Date(plannedDate + 'T12:00:00').toISOString();
    }

    const mealData = {
      ...mealForm,
      plannedDate
    };

    console.log('Adding meal:', mealData);

    const success = await mealPlansStore.addMealPlanItem(
        mealPlansStore.currentMealPlan.id,
        mealData
    );

    if (success) {
      toast.success('Meal added to plan successfully');
      closeModal();
    } else {
      toast.error(mealPlansStore.error || 'Failed to add meal to plan');
    }
  } catch (error) {
    console.error('Error adding meal:', error);
    toast.error('An unexpected error occurred while adding the meal');
  }
};

const editMeal = (meal: MealPlanItem) => {
  toast.info('Edit meal functionality coming soon!');
};

const deleteMeal = async (meal: MealPlanItem) => {
  if (!mealPlansStore.currentMealPlan) return;

  if (confirm(`Are you sure you want to remove "${meal.recipeName}" from your meal plan?`)) {
    try {
      const success = await mealPlansStore.deleteMealPlanItem(
          mealPlansStore.currentMealPlan.id,
          meal.id
      );

      if (success) {
        toast.success('Meal removed from plan');
      } else {
        toast.error(mealPlansStore.error || 'Failed to remove meal from plan');
      }
    } catch (error) {
      console.error('Error deleting meal:', error);
      toast.error('An unexpected error occurred while removing the meal');
    }
  }
};

const generateShoppingList = async () => {
  if (!mealPlansStore.currentMealPlan) {
    toast.error('No meal plan selected');
    return;
  }

  if (!mealPlansStore.currentMealPlan.mealPlanItems || mealPlansStore.currentMealPlan.mealPlanItems.length === 0) {
    toast.error('Add some meals to your plan first');
    return;
  }

  try {
    const result = await shoppingListsStore.createFromMealPlan(
        mealPlansStore.currentMealPlan.id,
        `Shopping List - ${mealPlansStore.currentMealPlan.name}`
    );

    if (result) {
      toast.success('Shopping list generated successfully');
      router.push(`/shopping-lists/${result.id}`);
    } else {
      toast.error(shoppingListsStore.error || 'Failed to generate shopping list');
    }
  } catch (error) {
    console.error('Error generating shopping list:', error);
    toast.error('An unexpected error occurred while generating the shopping list');
  }
};

// Lifecycle
onMounted(async () => {
  const id = parseInt(route.params.id as string);

  try {
    await Promise.all([
      mealPlansStore.fetchMealPlan(id),
      recipesStore.fetchRecipes()
    ]);

    availableRecipes.value = recipesStore.recipes;

    // Set default date
    if (mealPlansStore.currentMealPlan) {
      const today = new Date().toISOString().split('T')[0];
      const startDate = mealPlansStore.currentMealPlan.startDate.split('T')[0];
      const endDate = mealPlansStore.currentMealPlan.endDate.split('T')[0];

      if (today >= startDate && today <= endDate) {
        mealForm.plannedDate = today;
      } else {
        mealForm.plannedDate = startDate;
      }
    }
  } catch (error) {
    console.error('Error loading meal plan data:', error);
    toast.error('Failed to load meal plan data');
  }
});
</script>