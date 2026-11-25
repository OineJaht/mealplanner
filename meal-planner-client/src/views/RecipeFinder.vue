<!-- src/views/RecipeFinder.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <h1 class="text-3xl font-bold text-gray-900">Recipe Finder</h1>
        <p class="mt-2 text-sm text-gray-600">Find recipes based on ingredients you have in your pantry</p>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Left Column - Ingredient Selection -->
        <div class="lg:col-span-1">
          <div class="bg-white shadow rounded-lg p-6">
            <h2 class="text-lg font-medium text-gray-900 mb-4">Select Ingredients</h2>

            <!-- Search for ingredients -->
            <div class="mb-4">
              <label for="ingredient-search" class="block text-sm font-medium text-gray-700 mb-2">
                Search Ingredients
              </label>
              <div class="relative">
                <input
                    id="ingredient-search"
                    v-model="searchTerm"
                    type="text"
                    class="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    placeholder="Search ingredients..."
                    @input="handleSearch"
                />
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
                </div>
              </div>
            </div>

            <!-- Search results -->
            <div v-if="searchResults.length > 0 && searchTerm" class="mb-4">
              <h3 class="text-sm font-medium text-gray-700 mb-2">Search Results</h3>
              <div class="space-y-2 max-h-48 overflow-y-auto">
                <button
                    v-for="ingredient in searchResults"
                    :key="ingredient.id"
                    @click="toggleIngredient(ingredient)"
                    :class="[
                    'w-full text-left px-3 py-2 rounded-md text-sm transition-colors',
                    selectedIngredients.some(i => i.id === ingredient.id)
                      ? 'bg-green-100 text-green-800 border border-green-200'
                      : 'bg-gray-50 text-gray-700 hover:bg-gray-100 border border-gray-200'
                  ]"
                >
                  <div class="flex items-center justify-between">
                    <span>{{ ingredient.name }}</span>
                    <span v-if="selectedIngredients.some(i => i.id === ingredient.id)" class="text-green-600">
                      <CheckIcon class="h-4 w-4" />
                    </span>
                  </div>
                  <div class="text-xs text-gray-500">{{ ingredient.category }}</div>
                </button>
              </div>
            </div>

            <!-- Quick access to pantry ingredients -->
            <div class="mb-4">
              <div class="flex items-center justify-between mb-2">
                <h3 class="text-sm font-medium text-gray-700">Your Pantry</h3>
                <button
                    @click="selectAllPantryIngredients"
                    class="text-xs text-green-600 hover:text-green-500"
                >
                  Select All
                </button>
              </div>
              <div v-if="pantryIngredients.length === 0" class="text-sm text-gray-500 text-center py-4">
                <ArchiveBoxIcon class="mx-auto h-8 w-8 text-gray-400 mb-2" />
                <p>No pantry ingredients found</p>
                <router-link to="/pantry" class="text-green-600 hover:text-green-500">
                  Add some ingredients to your pantry
                </router-link>
              </div>
              <div v-else class="space-y-2 max-h-64 overflow-y-auto">
                <button
                    v-for="ingredient in pantryIngredients"
                    :key="ingredient.id"
                    @click="toggleIngredient(ingredient)"
                    :class="[
                    'w-full text-left px-3 py-2 rounded-md text-sm transition-colors',
                    selectedIngredients.some(i => i.id === ingredient.id)
                      ? 'bg-green-100 text-green-800 border border-green-200'
                      : 'bg-gray-50 text-gray-700 hover:bg-gray-100 border border-gray-200'
                  ]"
                >
                  <div class="flex items-center justify-between">
                    <span>{{ ingredient.name }}</span>
                    <span v-if="selectedIngredients.some(i => i.id === ingredient.id)" class="text-green-600">
                      <CheckIcon class="h-4 w-4" />
                    </span>
                  </div>
                </button>
              </div>
            </div>

            <!-- Selected ingredients -->
            <div v-if="selectedIngredients.length > 0" class="mb-4">
              <h3 class="text-sm font-medium text-gray-700 mb-2">
                Selected Ingredients ({{ selectedIngredients.length }})
              </h3>
              <div class="flex flex-wrap gap-2">
                <span
                    v-for="ingredient in selectedIngredients"
                    :key="ingredient.id"
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800"
                >
                  {{ ingredient.name }}
                  <button
                      @click="removeIngredient(ingredient)"
                      class="flex-shrink-0 ml-1.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-green-400 hover:bg-green-200 hover:text-green-500 focus:outline-none focus:bg-green-500 focus:text-white"
                  >
                    <XMarkIcon class="h-3 w-3" />
                  </button>
                </span>
              </div>
            </div>

            <!-- Match options -->
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-2">Match Type</label>
              <div class="space-y-2">
                <label class="flex items-center">
                  <input
                      v-model="matchAll"
                      type="radio"
                      :value="false"
                      class="focus:ring-green-500 h-4 w-4 text-green-600 border-gray-300"
                  />
                  <span class="ml-2 text-sm text-gray-700">Any ingredients (more results)</span>
                </label>
                <label class="flex items-center">
                  <input
                      v-model="matchAll"
                      type="radio"
                      :value="true"
                      class="focus:ring-green-500 h-4 w-4 text-green-600 border-gray-300"
                  />
                  <span class="ml-2 text-sm text-gray-700">All ingredients (exact matches)</span>
                </label>
              </div>
            </div>

            <!-- Search button -->
            <button
                @click="findRecipes"
                :disabled="selectedIngredients.length === 0 || recipesStore.isLoading"
                class="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="recipesStore.isLoading" class="flex items-center">
                <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                Searching...
              </span>
              <span v-else>Find Recipes</span>
            </button>
          </div>
        </div>

        <!-- Right Column - Results -->
        <div class="lg:col-span-2">
          <div class="bg-white shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200">
              <h2 class="text-lg font-medium text-gray-900">Recipe Results</h2>
              <p v-if="recipesStore.recipeMatches.length > 0" class="text-sm text-gray-600">
                Found {{ recipesStore.recipeMatches.length }} recipes
              </p>
            </div>

            <div class="p-6">
              <!-- Loading state -->
              <div v-if="recipesStore.isLoading" class="space-y-4">
                <div v-for="i in 3" :key="i" class="animate-pulse">
                  <div class="flex space-x-4">
                    <div class="rounded-md bg-gray-200 h-20 w-20"></div>
                    <div class="flex-1 space-y-2 py-1">
                      <div class="h-4 bg-gray-200 rounded w-3/4"></div>
                      <div class="space-y-2">
                        <div class="h-3 bg-gray-200 rounded"></div>
                        <div class="h-3 bg-gray-200 rounded w-5/6"></div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- No results -->
              <div v-else-if="recipesStore.recipeMatches.length === 0 && hasSearched" class="text-center py-12">
                <MagnifyingGlassIcon class="mx-auto h-12 w-12 text-gray-400" />
                <h3 class="mt-2 text-sm font-medium text-gray-900">No recipes found</h3>
                <p class="mt-1 text-sm text-gray-500">
                  Try selecting different ingredients or change your match type.
                </p>
              </div>

              <!-- Initial state -->
              <div v-else-if="!hasSearched" class="text-center py-12">
                <BookOpenIcon class="mx-auto h-12 w-12 text-gray-400" />
                <h3 class="mt-2 text-sm font-medium text-gray-900">Ready to find recipes?</h3>
                <p class="mt-1 text-sm text-gray-500">
                  Select ingredients from your pantry or search for specific ingredients to get started.
                </p>
              </div>

              <!-- Results -->
              <div v-else class="space-y-6">
                <div
                    v-for="match in recipesStore.recipeMatches"
                    :key="match.recipe?.id"
                    class="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors"
                >
                  <div class="flex items-start space-x-4">
                    <div class="flex-shrink-0">
                      <div class="h-20 w-20 bg-gray-200 rounded-md flex items-center justify-center">
                        <BookOpenIcon class="h-8 w-8 text-gray-400" />
                      </div>
                    </div>

                    <div class="flex-1 min-w-0">
                      <div class="flex items-center justify-between">
                        <h3 class="text-lg font-medium text-gray-900">
                          <router-link
                              :to="`/recipes/${match.recipe?.id}`"
                              class="hover:text-green-600"
                          >
                            {{ match.recipe?.name }}
                          </router-link>
                        </h3>
                        <div class="flex items-center space-x-2">
                          <span :class="[
                            'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                            match.matchPercentage >= 80 ? 'bg-green-100 text-green-800' :
                            match.matchPercentage >= 60 ? 'bg-yellow-100 text-yellow-800' :
                            'bg-red-100 text-red-800'
                          ]">
                            {{ match.matchPercentage }}% match
                          </span>
                        </div>
                      </div>

                      <p class="mt-1 text-sm text-gray-600">{{ match.recipe?.description }}</p>

                      <div class="mt-2 flex items-center space-x-4 text-sm text-gray-500">
                        <span class="flex items-center">
                          <ClockIcon class="h-4 w-4 mr-1" />
                          {{ match.recipe?.prepTimeMinutes }}min
                        </span>
                        <span class="flex items-center">
                          <UserGroupIcon class="h-4 w-4 mr-1" />
                          {{ match.recipe?.defaultServings }} servings
                        </span>
                      </div>

                      <!-- Missing ingredients -->
                      <div v-if="match.missingIngredients.length > 0" class="mt-3">
                        <p class="text-sm font-medium text-gray-700 mb-1">Missing ingredients:</p>
                        <div class="flex flex-wrap gap-1">
                          <span
                              v-for="ingredient in match.missingIngredients"
                              :key="ingredient.id"
                              class="inline-flex items-center px-2 py-1 rounded text-xs font-medium bg-red-100 text-red-800"
                          >
                            {{ ingredient.name }}
                          </span>
                        </div>
                      </div>

                      <div class="mt-4 flex space-x-3">
                        <router-link
                            :to="`/recipes/${match.recipe?.id}`"
                            class="text-sm font-medium text-green-600 hover:text-green-500"
                        >
                          View Recipe
                        </router-link>
                        <button
                            @click="addToMealPlan(match.recipe)"
                            class="text-sm font-medium text-blue-600 hover:text-blue-500"
                        >
                          Add to Meal Plan
                        </button>
                      </div>
                    </div>
                  </div>
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
import { ref, onMounted, computed } from 'vue';
import {
  MagnifyingGlassIcon,
  CheckIcon,
  XMarkIcon,
  ArchiveBoxIcon,
  BookOpenIcon,
  ClockIcon,
  UserGroupIcon
} from '@heroicons/vue/24/outline';

import { useRecipesStore } from '@/stores/recipes';
import { useIngredientsStore } from '@/stores/ingredients';
import { ApiService } from '@/services/api';
import type { Ingredient, FindRecipesRequest, Recipe } from '@/types';

const recipesStore = useRecipesStore();
const ingredientsStore = useIngredientsStore();

const searchTerm = ref('');
const searchResults = ref<Ingredient[]>([]);
const pantryIngredients = ref<Ingredient[]>([]);
const selectedIngredients = ref<Ingredient[]>([]);
const matchAll = ref(false);
const hasSearched = ref(false);

const handleSearch = async () => {
  if (searchTerm.value.length >= 2) {
    searchResults.value = await ingredientsStore.searchIngredients(searchTerm.value);
  } else {
    searchResults.value = [];
  }
};

const toggleIngredient = (ingredient: Ingredient) => {
  const index = selectedIngredients.value.findIndex(i => i.id === ingredient.id);
  if (index > -1) {
    selectedIngredients.value.splice(index, 1);
  } else {
    selectedIngredients.value.push(ingredient);
  }
};

const removeIngredient = (ingredient: Ingredient) => {
  const index = selectedIngredients.value.findIndex(i => i.id === ingredient.id);
  if (index > -1) {
    selectedIngredients.value.splice(index, 1);
  }
};

const selectAllPantryIngredients = () => {
  pantryIngredients.value.forEach(ingredient => {
    if (!selectedIngredients.value.some(i => i.id === ingredient.id)) {
      selectedIngredients.value.push(ingredient);
    }
  });
};

const findRecipes = async () => {
  if (selectedIngredients.value.length === 0) return;

  const request: FindRecipesRequest = {
    ingredientIds: selectedIngredients.value.map(i => i.id),
    matchAll: matchAll.value
  };

  await recipesStore.findRecipes(request);
  hasSearched.value = true;
};

const addToMealPlan = (recipe: Recipe | undefined) => {
  if (recipe) {
    // Navigate to meal plan with recipe pre-selected
    // This would require additional implementation
    console.log('Add to meal plan:', recipe.name);
  }
};

onMounted(async () => {
  // Load pantry ingredients
  try {
    const response = await ApiService.getPantryIngredients();
    if (response.success && response.data) {
      pantryIngredients.value = response.data;
    }
  } catch (error) {
    console.error('Failed to load pantry ingredients:', error);
  }
});
</script>