<!-- src/views/Recipes.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Recipes</h1>
            <p class="mt-2 text-sm text-gray-600">
              Manage your recipe collection
            </p>
          </div>
          <div class="flex space-x-3">
            <router-link
                to="/recipes/create"
                class="btn-primary"
            >
              <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
              New Recipe
            </router-link>
          </div>
        </div>
      </div>

      <!-- Search and Filters -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="px-4 py-5 sm:p-6">
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <!-- Search -->
            <div>
              <label for="search" class="block text-sm font-medium text-gray-700 mb-1">
                Search Recipes
              </label>
              <div class="relative">
                <input
                    id="search"
                    v-model="searchTerm"
                    type="text"
                    class="form-input pl-10"
                    placeholder="Search recipes..."
                />
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
                </div>
              </div>
            </div>

            <!-- Category Filter -->
            <div>
              <label for="category" class="block text-sm font-medium text-gray-700 mb-1">
                Category
              </label>
              <select
                  id="category"
                  v-model="selectedCategory"
                  class="form-input"
              >
                <option value="">All Categories</option>
                <option v-for="category in ingredientsStore.categories" :key="category.id" :value="category.name">
                  {{ category.name }}
                </option>
              </select>
            </div>

            <!-- Sort -->
            <div>
              <label for="sort" class="block text-sm font-medium text-gray-700 mb-1">
                Sort By
              </label>
              <select
                  id="sort"
                  v-model="sortBy"
                  class="form-input"
              >
                <option value="name">Name</option>
                <option value="created">Date Created</option>
                <option value="prepTime">Prep Time</option>
                <option value="favorite">Favorites First</option>
              </select>
            </div>
          </div>

          <!-- Filter tags -->
          <div class="mt-4 flex flex-wrap items-center gap-2">
            <span class="text-sm font-medium text-gray-700">Filters:</span>
            <button
                @click="showFavoritesOnly = !showFavoritesOnly"
                :class="[
                'inline-flex items-center px-3 py-1 rounded-full text-sm font-medium transition-colors',
                showFavoritesOnly
                  ? 'bg-red-100 text-red-800 border border-red-200'
                  : 'bg-gray-100 text-gray-700 border border-gray-200 hover:bg-gray-200'
              ]"
            >
              <HeartIcon :class="[
                'h-4 w-4 mr-1',
                showFavoritesOnly ? 'text-red-600' : 'text-gray-400'
              ]" />
              Favorites Only
            </button>
            <button
                v-if="searchTerm || selectedCategory || showFavoritesOnly"
                @click="clearFilters"
                class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-gray-100 text-gray-700 border border-gray-200 hover:bg-gray-200"
            >
              <XMarkIcon class="h-4 w-4 mr-1" />
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      <!-- Recipe Grid -->
      <div v-if="recipesStore.isLoading" class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <div v-for="i in 6" :key="i" class="animate-pulse">
          <div class="bg-white rounded-lg shadow">
            <div class="h-48 bg-gray-200 rounded-t-lg"></div>
            <div class="p-4">
              <div class="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
              <div class="h-3 bg-gray-200 rounded w-1/2 mb-4"></div>
              <div class="flex justify-between items-center">
                <div class="h-3 bg-gray-200 rounded w-1/4"></div>
                <div class="h-3 bg-gray-200 rounded w-1/4"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-else-if="filteredRecipes.length === 0" class="text-center py-12">
        <BookOpenIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">
          {{ recipesStore.recipes.length === 0 ? 'No recipes yet' : 'No recipes match your filters' }}
        </h3>
        <p class="mt-1 text-sm text-gray-500">
          {{ recipesStore.recipes.length === 0
            ? 'Get started by creating your first recipe.'
            : 'Try adjusting your search terms or filters.'
          }}
        </p>
        <div class="mt-6">
          <router-link
              v-if="recipesStore.recipes.length === 0"
              to="/recipes/create"
              class="btn-primary"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            Create Your First Recipe
          </router-link>
          <button
              v-else
              @click="clearFilters"
              class="btn-secondary"
          >
            Clear Filters
          </button>
        </div>
      </div>

      <div v-else class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <div
            v-for="recipe in filteredRecipes"
            :key="recipe.id"
            class="bg-white overflow-hidden shadow rounded-lg hover:shadow-md transition-shadow"
        >
          <!-- Recipe Image -->
          <div class="h-48 bg-gray-200 relative">
            <img
                v-if="recipe.imageUrl"
                :src="recipe.imageUrl"
                :alt="recipe.name"
                class="w-full h-full object-cover"
            />
            <div v-else class="w-full h-full flex items-center justify-center">
              <BookOpenIcon class="h-12 w-12 text-gray-400" />
            </div>
            <!-- Favorite badge -->
            <div
                v-if="recipe.isFavorite"
                class="absolute top-2 right-2 p-1 bg-white rounded-full shadow-md"
            >
              <HeartIcon class="h-5 w-5 text-red-500" />
            </div>
          </div>

          <!-- Recipe Content -->
          <div class="p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1 min-w-0">
                <h3 class="text-lg font-medium text-gray-900 truncate">
                  <router-link
                      :to="`/recipes/${recipe.id}`"
                      class="hover:text-green-600"
                  >
                    {{ recipe.name }}
                  </router-link>
                </h3>
                <p class="mt-1 text-sm text-gray-600 line-clamp-2">
                  {{ recipe.description }}
                </p>
              </div>

              <!-- Actions Dropdown -->
              <Menu as="div" class="relative ml-3">
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
                          :to="`/recipes/${recipe.id}`"
                          :class="[active ? 'bg-gray-100' : '', 'block px-4 py-2 text-sm text-gray-700']"
                      >
                        View Recipe
                      </router-link>
                    </MenuItem>
                    <MenuItem v-slot="{ active }">
                      <router-link
                          :to="`/recipes/${recipe.id}/edit`"
                          :class="[active ? 'bg-gray-100' : '', 'block px-4 py-2 text-sm text-gray-700']"
                      >
                        Edit Recipe
                      </router-link>
                    </MenuItem>
                    <MenuItem v-slot="{ active }">
                      <button
                          @click="toggleFavorite(recipe)"
                          :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                      >
                        {{ recipe.isFavorite ? 'Remove from Favorites' : 'Add to Favorites' }}
                      </button>
                    </MenuItem>
                    <MenuItem v-slot="{ active }">
                      <button
                          @click="confirmDelete(recipe)"
                          :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-red-600']"
                      >
                        Delete Recipe
                      </button>
                    </MenuItem>
                  </MenuItems>
                </transition>
              </Menu>
            </div>

            <!-- Recipe Meta -->
            <div class="mt-4 flex items-center justify-between text-sm text-gray-500">
              <div class="flex items-center space-x-4">
                <span class="flex items-center">
                  <ClockIcon class="h-4 w-4 mr-1" />
                  {{ recipe.prepTimeMinutes }}min
                </span>
                <span class="flex items-center">
                  <UserGroupIcon class="h-4 w-4 mr-1" />
                  {{ recipe.defaultServings }}
                </span>
              </div>
              <span class="text-xs">
                {{ formatDate(recipe.createdAt) }}
              </span>
            </div>

            <!-- Categories -->
            <div v-if="recipe.categories.length > 0" class="mt-3 flex flex-wrap gap-1">
              <span
                  v-for="category in recipe.categories.slice(0, 3)"
                  :key="category.id"
                  class="inline-flex items-center px-2 py-1 rounded text-xs font-medium bg-green-100 text-green-800"
              >
                {{ category.name }}
              </span>
              <span
                  v-if="recipe.categories.length > 3"
                  class="inline-flex items-center px-2 py-1 rounded text-xs font-medium bg-gray-100 text-gray-600"
              >
                +{{ recipe.categories.length - 3 }} more
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <TransitionRoot as="template" :show="showDeleteModal">
      <Dialog as="div" class="relative z-10" @close="showDeleteModal = false">
        <TransitionChild
            as="template"
            enter="ease-out duration-300"
            enter-from="opacity-0"
            enter-to="opacity-100"
            leave="ease-in duration-200"
            leave-from="opacity-100"
            leave-to="opacity-0"
        >
          <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" />
        </TransitionChild>

        <div class="fixed inset-0 z-10 overflow-y-auto">
          <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
            <TransitionChild
                as="template"
                enter="ease-out duration-300"
                enter-from="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
                enter-to="opacity-100 translate-y-0 sm:scale-100"
                leave="ease-in duration-200"
                leave-from="opacity-100 translate-y-0 sm:scale-100"
                leave-to="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
            >
              <DialogPanel class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg">
                <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
                  <div class="sm:flex sm:items-start">
                    <div class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-red-100 sm:mx-0 sm:h-10 sm:w-10">
                      <ExclamationTriangleIcon class="h-6 w-6 text-red-600" aria-hidden="true" />
                    </div>
                    <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
                      <DialogTitle as="h3" class="text-base font-semibold leading-6 text-gray-900">
                        Delete Recipe
                      </DialogTitle>
                      <div class="mt-2">
                        <p class="text-sm text-gray-500">
                          Are you sure you want to delete "{{ recipeToDelete?.name }}"? This action cannot be undone.
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="bg-gray-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
                  <button
                      type="button"
                      @click="deleteRecipe"
                      :disabled="isDeleting"
                      class="inline-flex w-full justify-center rounded-md bg-red-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-red-500 sm:ml-3 sm:w-auto disabled:opacity-50"
                  >
                    {{ isDeleting ? 'Deleting...' : 'Delete' }}
                  </button>
                  <button
                      type="button"
                      @click="showDeleteModal = false"
                      class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50 sm:mt-0 sm:w-auto"
                  >
                    Cancel
                  </button>
                </div>
              </DialogPanel>
            </TransitionChild>
          </div>
        </div>
      </Dialog>
    </TransitionRoot>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { format } from 'date-fns';
import {
  PlusIcon,
  MagnifyingGlassIcon,
  BookOpenIcon,
  ClockIcon,
  UserGroupIcon,
  HeartIcon,
  XMarkIcon,
  EllipsisVerticalIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline';
import {
  Menu,
  MenuButton,
  MenuItem,
  MenuItems,
  Dialog,
  DialogPanel,
  DialogTitle,
  TransitionChild,
  TransitionRoot
} from '@headlessui/vue';

import { useRecipesStore } from '@/stores/recipes';
import { useIngredientsStore } from '@/stores/ingredients';
import type {Recipe, RecipeUpdateDto} from '@/types';

const toast = useToast();
const recipesStore = useRecipesStore();
const ingredientsStore = useIngredientsStore();

// Search and filter state
const searchTerm = ref('');
const selectedCategory = ref('');
const sortBy = ref('name');
const showFavoritesOnly = ref(false);

// Modal state
const showDeleteModal = ref(false);
const recipeToDelete = ref<Recipe | null>(null);
const isDeleting = ref(false);

// Computed properties
const filteredRecipes = computed(() => {
  let filtered = [...recipesStore.recipes];

  // Apply search filter
  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase();
    filtered = filtered.filter(recipe =>
        recipe.name.toLowerCase().includes(term) ||
        recipe.description?.toLowerCase().includes(term)
    );
  }

  // Apply category filter
  if (selectedCategory.value) {
    filtered = filtered.filter(recipe =>
        recipe.categories.some(cat => cat.name === selectedCategory.value)
    );
  }

  // Apply favorites filter
  if (showFavoritesOnly.value) {
    filtered = filtered.filter(recipe => recipe.isFavorite);
  }

  // Apply sorting
  filtered.sort((a, b) => {
    switch (sortBy.value) {
      case 'name':
        return a.name.localeCompare(b.name);
      case 'created':
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      case 'prepTime':
        return a.prepTimeMinutes - b.prepTimeMinutes;
      case 'favorite':
        if (a.isFavorite === b.isFavorite) {
          return a.name.localeCompare(b.name);
        }
        return a.isFavorite ? -1 : 1;
      default:
        return 0;
    }
  });

  return filtered;
});

// Methods
const clearFilters = () => {
  searchTerm.value = '';
  selectedCategory.value = '';
  showFavoritesOnly.value = false;
  sortBy.value = 'name';
};

const formatDate = (dateString: string) => {
  return format(new Date(dateString), 'MMM d, yyyy');
};

const toggleFavorite = async (recipe: Recipe) => {
  // For toggling favorite, we only need to update the isFavorite field
  // We'll create a minimal update DTO with required fields
  const updatedRecipe: RecipeUpdateDto = {
    id: recipe.id,
    name: recipe.name,
    description: recipe.description || '',
    instructions: '', // We don't have instructions in the Recipe list view, so empty string
    defaultServings: recipe.defaultServings,
    prepTimeMinutes: recipe.prepTimeMinutes,
    isFavorite: !recipe.isFavorite,
    imageUrl: recipe.imageUrl,
    categoryIds: recipe.categories.map(c => c.id),
    ingredients: [] // We don't have ingredients in the Recipe list view, so empty array
  };

  const result = await recipesStore.updateRecipe(recipe.id, updatedRecipe);

  if (result) {
    toast.success(
        updatedRecipe.isFavorite
            ? `Added "${recipe.name}" to favorites`
            : `Removed "${recipe.name}" from favorites`
    );
  } else {
    toast.error('Failed to update recipe');
  }
};

const confirmDelete = (recipe: Recipe) => {
  recipeToDelete.value = recipe;
  showDeleteModal.value = true;
};

const deleteRecipe = async () => {
  if (!recipeToDelete.value) return;

  isDeleting.value = true;
  const success = await recipesStore.deleteRecipe(recipeToDelete.value.id);

  if (success) {
    toast.success(`Deleted "${recipeToDelete.value.name}"`);
    showDeleteModal.value = false;
    recipeToDelete.value = null;
  } else {
    toast.error('Failed to delete recipe');
  }

  isDeleting.value = false;
};

// Lifecycle
onMounted(async () => {
  await Promise.all([
    recipesStore.fetchRecipes(),
    ingredientsStore.fetchCategories()
  ]);
});
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>