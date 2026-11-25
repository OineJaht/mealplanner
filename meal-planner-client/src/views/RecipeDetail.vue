<!-- src/views/RecipeDetail.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <nav class="flex" aria-label="Breadcrumb">
          <ol class="flex items-center space-x-4">
            <li>
              <router-link to="/recipes" class="text-gray-400 hover:text-gray-500">
                <ChevronLeftIcon class="flex-shrink-0 h-5 w-5" />
                <span class="ml-4 text-sm font-medium text-gray-500 hover:text-gray-700">
                  Recipes
                </span>
              </router-link>
            </li>
          </ol>
        </nav>
      </div>

      <!-- Loading State -->
      <div v-if="recipesStore.isLoading" class="animate-pulse">
        <div class="bg-white shadow rounded-lg p-6 mb-6">
          <div class="h-8 bg-gray-200 rounded w-1/2 mb-4"></div>
          <div class="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
          <div class="h-4 bg-gray-200 rounded w-1/2"></div>
        </div>
      </div>

      <!-- Recipe Content -->
      <div v-else-if="recipesStore.currentRecipe" class="space-y-6">
        <!-- Recipe Header -->
        <div class="bg-white shadow rounded-lg overflow-hidden">
          <div class="h-64 bg-gray-200 relative">
            <img
                v-if="recipesStore.currentRecipe.imageUrl"
                :src="recipesStore.currentRecipe.imageUrl"
                :alt="recipesStore.currentRecipe.name"
                class="w-full h-full object-cover"
            />
            <div v-else class="w-full h-full flex items-center justify-center">
              <BookOpenIcon class="h-16 w-16 text-gray-400" />
            </div>

            <!-- Favorite Badge -->
            <div
                v-if="recipesStore.currentRecipe.isFavorite"
                class="absolute top-4 right-4 p-2 bg-white rounded-full shadow-md"
            >
              <HeartIcon class="h-6 w-6 text-red-500" />
            </div>
          </div>

          <div class="p-6">
            <div class="flex items-start justify-between">
              <div class="flex-1 min-w-0">
                <h1 class="text-3xl font-bold text-gray-900">
                  {{ recipesStore.currentRecipe.name }}
                </h1>
                <p v-if="recipesStore.currentRecipe.description" class="mt-2 text-gray-600">
                  {{ recipesStore.currentRecipe.description }}
                </p>

                <div class="mt-4 flex items-center space-x-6 text-sm text-gray-500">
                  <span class="flex items-center">
                    <ClockIcon class="h-4 w-4 mr-1" />
                    {{ recipesStore.currentRecipe.prepTimeMinutes }} minutes
                  </span>
                  <span class="flex items-center">
                    <UserGroupIcon class="h-4 w-4 mr-1" />
                    {{ recipesStore.currentRecipe.defaultServings }} servings
                  </span>
                  <span class="text-xs">
                    Created {{ formatDate(recipesStore.currentRecipe.createdAt) }}
                  </span>
                </div>

                <!-- Categories -->
                <div v-if="recipesStore.currentRecipe.categories.length > 0" class="mt-4 flex flex-wrap gap-2">
                  <span
                      v-for="category in recipesStore.currentRecipe.categories"
                      :key="category.id"
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800"
                  >
                    {{ category.name }}
                  </span>
                </div>
              </div>

              <div class="flex space-x-3 ml-6">
                <button
                    @click="toggleFavorite"
                    :class="[
                    'btn-secondary',
                    recipesStore.currentRecipe.isFavorite ? 'text-red-600' : 'text-gray-400'
                  ]"
                >
                  <HeartIcon class="-ml-1 mr-2 h-4 w-4" />
                  {{ recipesStore.currentRecipe.isFavorite ? 'Favorited' : 'Add to Favorites' }}
                </button>
                <router-link
                    :to="`/recipes/${recipesStore.currentRecipe.id}/edit`"
                    class="btn-secondary"
                >
                  <PencilIcon class="-ml-1 mr-2 h-4 w-4" />
                  Edit
                </router-link>
                <button
                    @click="addToMealPlan"
                    class="btn-primary"
                >
                  <CalendarIcon class="-ml-1 mr-2 h-4 w-4" />
                  Add to Meal Plan
                </button>
              </div>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- Ingredients -->
          <div class="lg:col-span-1">
            <div class="bg-white shadow rounded-lg">
              <div class="px-6 py-4 border-b border-gray-200">
                <h3 class="text-lg font-medium text-gray-900">Ingredients</h3>
              </div>
              <div class="px-6 py-4">
                <ul v-if="recipesStore.currentRecipe.ingredients.length > 0" class="space-y-3">
                  <li
                      v-for="ingredient in recipesStore.currentRecipe.ingredients"
                      :key="ingredient.id"
                      class="flex items-center justify-between"
                  >
                    <div class="flex-1">
                      <span class="text-sm font-medium text-gray-900">
                        {{ ingredient.ingredientName }}
                      </span>
                    </div>
                    <div class="text-sm text-gray-500">
                      {{ ingredient.amount }} {{ ingredient.unit }}
                    </div>
                  </li>
                </ul>
                <p v-else class="text-sm text-gray-500">No ingredients listed</p>
              </div>
            </div>

            <!-- Nutrition (if available) -->
            <div v-if="recipesStore.currentRecipe.nutrition" class="bg-white shadow rounded-lg mt-6">
              <div class="px-6 py-4 border-b border-gray-200">
                <h3 class="text-lg font-medium text-gray-900">Nutrition</h3>
                <p class="text-sm text-gray-500">Per serving</p>
              </div>
              <div class="px-6 py-4">
                <div class="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <span class="text-gray-500">Calories:</span>
                    <span class="ml-2 font-medium">{{ Math.round(recipesStore.currentRecipe.nutrition.caloriesPerServing) }}</span>
                  </div>
                  <div>
                    <span class="text-gray-500">Protein:</span>
                    <span class="ml-2 font-medium">{{ Math.round(recipesStore.currentRecipe.nutrition.totalProteinG / recipesStore.currentRecipe.defaultServings) }}g</span>
                  </div>
                  <div>
                    <span class="text-gray-500">Carbs:</span>
                    <span class="ml-2 font-medium">{{ Math.round(recipesStore.currentRecipe.nutrition.totalCarbsG / recipesStore.currentRecipe.defaultServings) }}g</span>
                  </div>
                  <div>
                    <span class="text-gray-500">Fat:</span>
                    <span class="ml-2 font-medium">{{ Math.round(recipesStore.currentRecipe.nutrition.totalFatG / recipesStore.currentRecipe.defaultServings) }}g</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Instructions -->
          <div class="lg:col-span-2">
            <div class="bg-white shadow rounded-lg">
              <div class="px-6 py-4 border-b border-gray-200">
                <h3 class="text-lg font-medium text-gray-900">Instructions</h3>
              </div>
              <div class="px-6 py-4">
                <div v-if="recipesStore.currentRecipe.instructions" class="prose max-w-none">
                  <div v-html="formatInstructions(recipesStore.currentRecipe.instructions)"></div>
                </div>
                <p v-else class="text-sm text-gray-500">No instructions provided</p>
              </div>
            </div>

            <!-- Cooking Steps (if available) -->
            <div v-if="recipesStore.currentRecipe.cookingSteps.length > 0" class="bg-white shadow rounded-lg mt-6">
              <div class="px-6 py-4 border-b border-gray-200">
                <h3 class="text-lg font-medium text-gray-900">Cooking Steps</h3>
              </div>
              <div class="px-6 py-4">
                <ol class="space-y-4">
                  <li
                      v-for="step in recipesStore.currentRecipe.cookingSteps"
                      :key="step.id"
                      class="flex"
                  >
                    <div class="flex-shrink-0 w-8 h-8 bg-green-100 rounded-full flex items-center justify-center mr-4">
                      <span class="text-sm font-medium text-green-600">{{ step.stepNumber }}</span>
                    </div>
                    <div class="flex-1">
                      <p class="text-sm text-gray-900">{{ step.instruction }}</p>
                      <p v-if="step.timingMinutes > 0" class="text-xs text-gray-500 mt-1">
                        {{ step.timingMinutes }} minutes
                      </p>
                    </div>
                  </li>
                </ol>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Not Found -->
      <div v-else class="text-center py-12">
        <ExclamationTriangleIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">Recipe not found</h3>
        <p class="mt-1 text-sm text-gray-500">The recipe you're looking for doesn't exist.</p>
        <div class="mt-6">
          <router-link to="/recipes" class="btn-primary">
            Back to Recipes
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useToast } from 'vue-toastification';
import { format } from 'date-fns';
import {
  ChevronLeftIcon,
  BookOpenIcon,
  HeartIcon,
  ClockIcon,
  UserGroupIcon,
  PencilIcon,
  CalendarIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline';

import { useRecipesStore } from '@/stores/recipes';
import type { RecipeUpdateDto } from '@/types';

const route = useRoute();
const toast = useToast();
const recipesStore = useRecipesStore();

// Methods
const formatDate = (dateString: string) => {
  return format(new Date(dateString), 'MMM d, yyyy');
};

const formatInstructions = (instructions: string) => {
  // Simple formatting: convert line breaks to paragraphs
  return instructions.split('\n').map(line => `<p>${line}</p>`).join('');
};

const toggleFavorite = async () => {
  if (!recipesStore.currentRecipe) return;

  const updatedRecipe: RecipeUpdateDto = {
    id: recipesStore.currentRecipe.id,
    name: recipesStore.currentRecipe.name,
    description: recipesStore.currentRecipe.description || '',
    instructions: recipesStore.currentRecipe.instructions || '',
    defaultServings: recipesStore.currentRecipe.defaultServings,
    prepTimeMinutes: recipesStore.currentRecipe.prepTimeMinutes,
    isFavorite: !recipesStore.currentRecipe.isFavorite,
    imageUrl: recipesStore.currentRecipe.imageUrl,
    categoryIds: recipesStore.currentRecipe.categories.map(c => c.id),
    ingredients: recipesStore.currentRecipe.ingredients.map(ing => ({
      id: ing.id,
      ingredientId: ing.ingredientId,
      amount: ing.amount,
      unit: ing.unit || ''
    }))
  };

  const result = await recipesStore.updateRecipe(recipesStore.currentRecipe.id, updatedRecipe);

  if (result) {
    toast.success(
        updatedRecipe.isFavorite
            ? `Added "${recipesStore.currentRecipe.name}" to favorites`
            : `Removed "${recipesStore.currentRecipe.name}" from favorites`
    );
  } else {
    toast.error('Failed to update recipe');
  }
};

const addToMealPlan = () => {
  // Would navigate to meal plan creation with recipe pre-selected
  toast.info('Add to meal plan functionality coming soon');
};

// Lifecycle
onMounted(async () => {
  const id = parseInt(route.params.id as string);
  await recipesStore.fetchRecipe(id);
});
</script>