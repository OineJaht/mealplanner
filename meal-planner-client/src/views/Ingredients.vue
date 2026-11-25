<!-- src/views/Ingredients.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Ingredients</h1>
            <p class="mt-2 text-sm text-gray-600">
              Manage your ingredient database
            </p>
          </div>
          <button
              @click="showCreateModal = true"
              class="btn-primary"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            Add Ingredient
          </button>
        </div>
      </div>

      <!-- Search and Filters -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="px-4 py-5 sm:p-6">
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <!-- Search -->
            <div>
              <label for="search" class="block text-sm font-medium text-gray-700 mb-1">
                Search Ingredients
              </label>
              <div class="relative">
                <input
                    id="search"
                    v-model="searchTerm"
                    type="text"
                    class="form-input pl-10"
                    placeholder="Search ingredients..."
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
          </div>
        </div>
      </div>

      <!-- Ingredients Grid -->
      <div v-if="ingredientsStore.isLoading" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <div v-for="i in 6" :key="i" class="animate-pulse">
          <div class="bg-white p-4 rounded-lg shadow">
            <div class="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
            <div class="h-3 bg-gray-200 rounded w-1/2"></div>
          </div>
        </div>
      </div>

      <div v-else-if="filteredIngredients.length === 0" class="text-center py-12">
        <BeakerIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">
          {{ ingredientsStore.ingredients.length === 0 ? 'No ingredients yet' : 'No ingredients match your filters' }}
        </h3>
        <p class="mt-1 text-sm text-gray-500">
          {{ ingredientsStore.ingredients.length === 0
            ? 'Get started by adding your first ingredient.'
            : 'Try adjusting your search terms or filters.'
          }}
        </p>
        <div class="mt-6">
          <button
              v-if="ingredientsStore.ingredients.length === 0"
              @click="showCreateModal = true"
              class="btn-primary"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            Add Your First Ingredient
          </button>
          <button
              v-else
              @click="clearFilters"
              class="btn-secondary"
          >
            Clear Filters
          </button>
        </div>
      </div>

      <div v-else class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <div
            v-for="ingredient in filteredIngredients"
            :key="ingredient.id"
            class="bg-white overflow-hidden shadow rounded-lg hover:shadow-md transition-shadow p-4"
        >
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-lg font-medium text-gray-900 truncate">
                {{ ingredient.name }}
              </h3>
              <p class="mt-1 text-sm text-gray-500">
                {{ ingredient.category || 'No category' }}
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
                    <button
                        @click="viewIngredientDetails(ingredient)"
                        :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                    >
                      View Details
                    </button>
                  </MenuItem>
                  <MenuItem v-slot="{ active }">
                    <button
                        @click="editIngredient(ingredient)"
                        :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                    >
                      Edit Ingredient
                    </button>
                  </MenuItem>
                </MenuItems>
              </transition>
            </Menu>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Ingredient Modal (simplified for now) -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Add New Ingredient</h3>
        <form @submit.prevent="createIngredient">
          <div class="space-y-4">
            <div>
              <label class="form-label">Name</label>
              <input
                  v-model="newIngredient.name"
                  type="text"
                  required
                  class="form-input"
                  placeholder="Ingredient name"
              />
            </div>
            <div>
              <label class="form-label">Category</label>
              <input
                  v-model="newIngredient.category"
                  type="text"
                  class="form-input"
                  placeholder="Category (optional)"
              />
            </div>
          </div>
          <div class="mt-6 flex justify-end space-x-3">
            <button
                type="button"
                @click="showCreateModal = false"
                class="btn-secondary"
            >
              Cancel
            </button>
            <button
                type="submit"
                :disabled="!newIngredient.name.trim()"
                class="btn-primary"
            >
              Add Ingredient
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, reactive } from 'vue';
import { useToast } from 'vue-toastification';
import {
  PlusIcon,
  MagnifyingGlassIcon,
  BeakerIcon,
  EllipsisVerticalIcon
} from '@heroicons/vue/24/outline';
import {
  Menu,
  MenuButton,
  MenuItem,
  MenuItems
} from '@headlessui/vue';

import { useIngredientsStore } from '@/stores/ingredients';
import type { Ingredient, IngredientCreateDto } from '@/types';

const toast = useToast();
const ingredientsStore = useIngredientsStore();

// Search and filter state
const searchTerm = ref('');
const selectedCategory = ref('');
const showCreateModal = ref(false);

// Form state
const newIngredient = reactive<IngredientCreateDto>({
  name: '',
  category: '',
  nutrition: {
    caloriesPer100g: 0,
    proteinG: 0,
    carbsG: 0,
    fatG: 0,
    fiberG: 0,
    sugarG: 0,
    sodiumMg: 0
  }
});

// Computed properties
const filteredIngredients = computed(() => {
  let filtered = [...ingredientsStore.ingredients];

  // Apply search filter
  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase();
    filtered = filtered.filter(ingredient =>
        ingredient.name.toLowerCase().includes(term) ||
        ingredient.category?.toLowerCase().includes(term)
    );
  }

  // Apply category filter
  if (selectedCategory.value) {
    filtered = filtered.filter(ingredient =>
        ingredient.category === selectedCategory.value
    );
  }

  return filtered.sort((a, b) => a.name.localeCompare(b.name));
});

// Methods
const clearFilters = () => {
  searchTerm.value = '';
  selectedCategory.value = '';
};

const createIngredient = async () => {
  const result = await ingredientsStore.createIngredient(newIngredient);
  if (result) {
    toast.success(`Added "${result.name}" to ingredients`);
    showCreateModal.value = false;
    newIngredient.name = '';
    newIngredient.category = '';
  } else {
    toast.error('Failed to create ingredient');
  }
};

const viewIngredientDetails = (ingredient: Ingredient) => {
  // Could navigate to ingredient detail page or show modal
  console.log('View ingredient:', ingredient.name);
};

const editIngredient = (ingredient: Ingredient) => {
  // Could navigate to edit page or show modal
  console.log('Edit ingredient:', ingredient.name);
};

// Lifecycle
onMounted(async () => {
  await Promise.all([
    ingredientsStore.fetchIngredients(),
    ingredientsStore.fetchCategories()
  ]);
});
</script>