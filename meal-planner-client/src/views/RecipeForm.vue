<!-- src/views/RecipeForm.vue - FIXED VERSION -->
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
        <h1 class="mt-2 text-3xl font-bold text-gray-900">
          {{ isEditing ? 'Edit Recipe' : 'Create Recipe' }}
        </h1>
        <p class="mt-2 text-sm text-gray-600">
          {{ isEditing ? 'Update your recipe details' : 'Add a new recipe to your collection' }}
        </p>
      </div>

      <!-- Form -->
      <form @submit.prevent="saveRecipe" class="space-y-6">
        <!-- Basic Information -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Basic Information</h3>

            <div class="grid grid-cols-1 gap-6 sm:grid-cols-2">
              <div class="sm:col-span-2">
                <label for="name" class="form-label">
                  Recipe Name *
                </label>
                <input
                    id="name"
                    v-model="form.name"
                    type="text"
                    required
                    class="form-input"
                    placeholder="Enter recipe name"
                />
              </div>

              <div class="sm:col-span-2">
                <label for="description" class="form-label">
                  Description
                </label>
                <textarea
                    id="description"
                    v-model="form.description"
                    rows="3"
                    class="form-input"
                    placeholder="Brief description of the recipe"
                ></textarea>
              </div>

              <div>
                <label for="servings" class="form-label">
                  Default Servings *
                </label>
                <input
                    id="servings"
                    v-model.number="form.defaultServings"
                    type="number"
                    min="1"
                    max="100"
                    required
                    class="form-input"
                />
              </div>

              <div>
                <label for="prepTime" class="form-label">
                  Prep Time (minutes) *
                </label>
                <input
                    id="prepTime"
                    v-model.number="form.prepTimeMinutes"
                    type="number"
                    min="0"
                    max="1440"
                    required
                    class="form-input"
                />
              </div>

              <div class="sm:col-span-2">
                <label for="imageUrl" class="form-label">
                  Image URL
                </label>
                <input
                    id="imageUrl"
                    v-model="form.imageUrl"
                    type="url"
                    class="form-input"
                    placeholder="https://example.com/image.jpg"
                />
              </div>

              <div class="sm:col-span-2">
                <label class="flex items-center">
                  <input
                      v-model="form.isFavorite"
                      type="checkbox"
                      class="rounded border-gray-300 text-green-600 shadow-sm focus:border-green-300 focus:ring focus:ring-green-200 focus:ring-opacity-50"
                  />
                  <span class="ml-2 text-sm text-gray-700">Mark as favorite</span>
                </label>
              </div>
            </div>
          </div>
        </div>

        <!-- Categories -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Categories</h3>

            <div class="space-y-2">
              <label
                  v-for="category in availableCategories"
                  :key="category.id"
                  class="flex items-center"
              >
                <input
                    v-model="form.categoryIds"
                    :value="category.id"
                    type="checkbox"
                    class="rounded border-gray-300 text-green-600 shadow-sm focus:border-green-300 focus:ring focus:ring-green-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">{{ category.name }}</span>
              </label>
            </div>
          </div>
        </div>

        <!-- Ingredients -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg leading-6 font-medium text-gray-900">Ingredients</h3>
              <button
                  type="button"
                  @click="addIngredient"
                  class="btn-secondary text-sm"
              >
                <PlusIcon class="-ml-1 mr-1 h-4 w-4" />
                Add Ingredient
              </button>
            </div>

            <div v-if="form.ingredients.length === 0" class="text-center py-8">
              <BeakerIcon class="mx-auto h-8 w-8 text-gray-400" />
              <p class="mt-2 text-sm text-gray-500">No ingredients added yet</p>
            </div>

            <div v-else class="space-y-4">
              <div
                  v-for="(ingredient, index) in form.ingredients"
                  :key="index"
                  class="flex items-center space-x-4 p-4 border border-gray-200 rounded-lg"
              >
                <div class="flex-1">
                  <select
                      v-model="ingredient.ingredientId"
                      required
                      class="form-input"
                  >
                    <option value="">Select ingredient...</option>
                    <option v-for="ing in availableIngredients" :key="ing.id" :value="ing.id">
                      {{ ing.name }}
                    </option>
                  </select>
                </div>
                <div class="w-24">
                  <input
                      v-model.number="ingredient.amount"
                      type="number"
                      step="0.1"
                      min="0"
                      placeholder="Amount"
                      required
                      class="form-input"
                  />
                </div>
                <div class="w-20">
                  <input
                      v-model="ingredient.unit"
                      type="text"
                      placeholder="Unit"
                      required
                      class="form-input"
                  />
                </div>
                <button
                    type="button"
                    @click="removeIngredient(index)"
                    class="text-red-400 hover:text-red-600"
                >
                  <TrashIcon class="h-5 w-5" />
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Instructions -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Instructions</h3>

            <div>
              <label for="instructions" class="form-label">
                Cooking Instructions *
              </label>
              <textarea
                  id="instructions"
                  v-model="form.instructions"
                  rows="8"
                  required
                  class="form-input"
                  placeholder="Enter detailed cooking instructions, one step per line..."
              ></textarea>
              <p class="mt-2 text-sm text-gray-500">
                Write clear, step-by-step instructions. Each new line will be treated as a separate step.
              </p>
            </div>
          </div>
        </div>

        <!-- Form Actions -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
            <button
                type="submit"
                :disabled="recipesStore.isLoading || !isFormValid"
                class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-green-600 text-base font-medium text-white hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50"
            >
              {{ recipesStore.isLoading ? 'Saving...' : (isEditing ? 'Update Recipe' : 'Create Recipe') }}
            </button>
            <router-link
                :to="isEditing ? `/recipes/${route.params.id}` : '/recipes'"
                class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
            >
              Cancel
            </router-link>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed, onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useToast } from 'vue-toastification';
import {
  ChevronLeftIcon,
  PlusIcon,
  TrashIcon,
  BeakerIcon
} from '@heroicons/vue/24/outline';

import { useRecipesStore } from '@/stores/recipes';
import { useIngredientsStore } from '@/stores/ingredients';
import type { RecipeCreateDto, RecipeUpdateDto, RecipeIngredientCreateDto, RecipeIngredientUpdateDto, Category, Ingredient, RecipeFormDto } from '@/types';

const route = useRoute();
const router = useRouter();
const toast = useToast();
const recipesStore = useRecipesStore();
const ingredientsStore = useIngredientsStore();

// Available data
const availableCategories = ref<Category[]>([]);
const availableIngredients = ref<Ingredient[]>([]);

// Computed properties
const isEditing = computed(() => !!route.params.id);

// Form validation
const isFormValid = computed(() => {
  return (
      form.name.trim() !== '' &&
      form.instructions.trim() !== '' &&
      form.defaultServings > 0 &&
      form.prepTimeMinutes >= 0 &&
      form.ingredients.length > 0 &&
      form.ingredients.every(ing => ing.ingredientId > 0 && ing.amount > 0 && ing.unit.trim() !== '')
  );
});

// Form state - Initialize with proper defaults
const form = reactive<RecipeFormDto & { imageUrl?: string }>({
  name: '',
  description: '',
  instructions: '',
  defaultServings: 4,
  prepTimeMinutes: 30,
  isFavorite: false,
  categoryIds: [],
  ingredients: [] as RecipeIngredientCreateDto[],
  imageUrl: ''
});

// Methods
const addIngredient = () => {
  form.ingredients.push({
    ingredientId: 0,
    amount: 0,
    unit: ''
  });
};

const removeIngredient = (index: number) => {
  form.ingredients.splice(index, 1);
};

const loadRecipeData = async () => {
  if (isEditing.value) {
    const id = parseInt(route.params.id as string);
    await recipesStore.fetchRecipe(id);

    if (recipesStore.currentRecipe) {
      const recipe = recipesStore.currentRecipe;
      form.name = recipe.name;
      form.description = recipe.description || '';
      form.instructions = recipe.instructions || '';
      form.defaultServings = recipe.defaultServings;
      form.prepTimeMinutes = recipe.prepTimeMinutes;
      form.isFavorite = recipe.isFavorite;
      form.imageUrl = recipe.imageUrl || '';
      form.categoryIds = recipe.categories.map(c => c.id);

      // Properly map ingredients for editing
      form.ingredients = recipe.ingredients.map(ing => ({
        ingredientId: ing.ingredientId,
        amount: ing.amount,
        unit: ing.unit || ''
      }));
    }
  }
};

const saveRecipe = async () => {
  if (!isFormValid.value) {
    toast.error('Please fill in all required fields correctly');
    return;
  }

  try {
    // Validate ingredients
    if (!form.ingredients || form.ingredients.length === 0) {
      toast.error('Please add at least one ingredient');
      return;
    }

    // Check for invalid ingredients
    const invalidIngredients = form.ingredients.some(ing =>
        ing.ingredientId === 0 || ing.amount <= 0 || ing.unit.trim() === ''
    );

    if (invalidIngredients) {
      toast.error('Please complete all ingredient information');
      return;
    }

    // Debug: Log the form data before sending
    console.log('Form data before sending:', JSON.stringify(form, null, 2));

    if (isEditing.value) {
      // Update existing recipe
      const id = parseInt(route.params.id as string);
      const updateData: RecipeUpdateDto = {
        ...form,
        id,
        // Ensure imageUrl is never null/undefined
        imageUrl: form.imageUrl?.trim() || '',
        ingredients: form.ingredients.map((ing, index) => ({
          id: recipesStore.currentRecipe?.ingredients[index]?.id || 0,
          ingredientId: ing.ingredientId,
          amount: ing.amount,
          unit: ing.unit
        }))
      };

      console.log('Update data:', JSON.stringify(updateData, null, 2));

      const result = await recipesStore.updateRecipe(id, updateData);
      if (result) {
        toast.success('Recipe updated successfully');
        router.push(`/recipes/${id}`);
      } else {
        toast.error(recipesStore.error || 'Failed to update recipe');
      }
    } else {
      // Create new recipe
      const result = await recipesStore.createRecipe(form);
      if (result) {
        toast.success('Recipe created successfully');
        router.push(`/recipes/${result.id}`);
      } else {
        toast.error(recipesStore.error || 'Failed to create recipe');
      }
    }
  } catch (error) {
    console.error('Error saving recipe:', error);
    toast.error('An unexpected error occurred while saving the recipe');
  }
};


// Lifecycle
onMounted(async () => {
  try {
    await Promise.all([
      ingredientsStore.fetchIngredients(),
      ingredientsStore.fetchCategories(),
      loadRecipeData()
    ]);

    availableIngredients.value = ingredientsStore.ingredients;
    availableCategories.value = ingredientsStore.categories;

    // Add one ingredient by default for new recipes
    if (!isEditing.value && (!form.ingredients || form.ingredients.length === 0)) {
      addIngredient();
    }
  } catch (error) {
    console.error('Error loading recipe form data:', error);
    toast.error('Failed to load form data');
  }
});
</script>