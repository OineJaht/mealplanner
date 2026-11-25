<!-- src/views/ShoppingListDetail.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <div class="flex items-center justify-between">
          <div>
            <nav class="flex" aria-label="Breadcrumb">
              <ol class="flex items-center space-x-4">
                <li>
                  <router-link to="/shopping-lists" class="text-gray-400 hover:text-gray-500">
                    <ChevronLeftIcon class="flex-shrink-0 h-5 w-5" />
                    <span class="ml-4 text-sm font-medium text-gray-500 hover:text-gray-700">
                      Shopping Lists
                    </span>
                  </router-link>
                </li>
              </ol>
            </nav>
            <h1 v-if="shoppingListsStore.currentShoppingList" class="mt-2 text-3xl font-bold text-gray-900">
              {{ shoppingListsStore.currentShoppingList.name }}
            </h1>
            <p v-if="shoppingListsStore.currentShoppingList" class="mt-2 text-sm text-gray-600">
              Created {{ formatDate(shoppingListsStore.currentShoppingList.createdDate) }}
            </p>
          </div>
          <div class="flex space-x-3">
            <button
                @click="showAddItemModal = true"
                class="btn-secondary"
            >
              <PlusIcon class="-ml-1 mr-2 h-4 w-4" />
              Add Item
            </button>
            <button
                v-if="shoppingListsStore.currentShoppingList && hasCompletedItems"
                @click="removeCompletedItems"
                class="btn-primary"
            >
              <TrashIcon class="-ml-1 mr-2 h-4 w-4" />
              Clear Completed
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="shoppingListsStore.isLoading" class="animate-pulse">
        <div class="bg-white shadow rounded-lg p-6">
          <div class="h-4 bg-gray-200 rounded w-1/4 mb-4"></div>
          <div class="space-y-3">
            <div class="h-3 bg-gray-200 rounded"></div>
            <div class="h-3 bg-gray-200 rounded w-5/6"></div>
          </div>
        </div>
      </div>

      <!-- Shopping List Content -->
      <div v-else-if="shoppingListsStore.currentShoppingList" class="space-y-6">
        <!-- Progress Overview -->
        <div class="bg-white shadow rounded-lg p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900">Shopping Progress</h3>
            <span class="text-sm text-gray-500">
              {{ shoppingListsStore.currentShoppingList.purchasedItems || 0 }} of {{ shoppingListsStore.currentShoppingList.totalItems || 0 }} items
            </span>
          </div>

          <div class="w-full bg-gray-200 rounded-full h-3">
            <div
                class="bg-green-600 h-3 rounded-full transition-all duration-300"
                :style="{ width: `${progressPercentage}%` }"
            ></div>
          </div>

          <div class="mt-2 flex justify-between items-center text-sm text-gray-500">
            <span>{{ progressPercentage }}% complete</span>
            <span v-if="remainingItems > 0">{{ remainingItems }} items remaining</span>
            <span v-else class="text-green-600 font-medium">All done! 🎉</span>
          </div>
        </div>

        <!-- Shopping List Items -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg font-medium text-gray-900">Items</h3>
          </div>

          <div class="divide-y divide-gray-200">
            <div v-if="shoppingListsStore.currentShoppingList.items.length === 0" class="px-6 py-12 text-center">
              <ShoppingBagIcon class="mx-auto h-12 w-12 text-gray-400" />
              <h3 class="mt-2 text-sm font-medium text-gray-900">No items yet</h3>
              <p class="mt-1 text-sm text-gray-500">Add items to start building your shopping list.</p>
              <div class="mt-6">
                <button
                    @click="showAddItemModal = true"
                    class="btn-primary"
                >
                  <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
                  Add Your First Item
                </button>
              </div>
            </div>

            <div
                v-for="item in sortedItems"
                :key="item.id"
                :class="[
                'px-6 py-4 flex items-center space-x-4 hover:bg-gray-50',
                item.purchased ? 'opacity-60' : ''
              ]"
            >
              <button
                  @click="toggleItem(item)"
                  :class="[
                  'flex-shrink-0 h-5 w-5 rounded border-2 flex items-center justify-center transition-colors',
                  item.purchased
                    ? 'bg-green-600 border-green-600 text-white'
                    : 'border-gray-300 hover:border-green-500'
                ]"
              >
                <CheckIcon v-if="item.purchased" class="h-3 w-3" />
              </button>

              <div class="flex-1 min-w-0">
                <p :class="[
                  'text-sm font-medium',
                  item.purchased ? 'line-through text-gray-500' : 'text-gray-900'
                ]">
                  {{ item.ingredientName }}
                </p>
                <p class="text-sm text-gray-500">
                  Amount: {{ item.amount }}
                </p>
              </div>

              <Menu as="div" class="relative">
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
                      <button
                          @click="toggleItem(item)"
                          :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                      >
                        {{ item.purchased ? 'Mark as Needed' : 'Mark as Purchased' }}
                      </button>
                    </MenuItem>
                    <MenuItem v-slot="{ active }">
                      <button
                          @click="editItem(item)"
                          :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                      >
                        Edit Item
                      </button>
                    </MenuItem>
                    <MenuItem v-slot="{ active }">
                      <button
                          @click="deleteItem(item)"
                          :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-red-600']"
                      >
                        Remove Item
                      </button>
                    </MenuItem>
                  </MenuItems>
                </transition>
              </Menu>
            </div>
          </div>
        </div>
      </div>

      <!-- Not Found -->
      <div v-else class="text-center py-12">
        <ExclamationTriangleIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">Shopping list not found</h3>
        <p class="mt-1 text-sm text-gray-500">The shopping list you're looking for doesn't exist.</p>
        <div class="mt-6">
          <router-link to="/shopping-lists" class="btn-primary">
            Back to Shopping Lists
          </router-link>
        </div>
      </div>
    </div>

    <!-- Add Item Modal -->
    <div v-if="showAddItemModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Add Item to Shopping List</h3>
        <form @submit.prevent="addItem">
          <div class="space-y-4">
            <div>
              <label class="form-label">Ingredient</label>
              <select
                  v-model="itemForm.ingredientId"
                  required
                  class="form-input"
              >
                <option value="">Select ingredient...</option>
                <option v-for="ingredient in availableIngredients" :key="ingredient.id" :value="ingredient.id">
                  {{ ingredient.name }}
                </option>
              </select>
            </div>

            <div>
              <label class="form-label">Amount</label>
              <input
                  v-model.number="itemForm.amount"
                  type="number"
                  step="0.1"
                  min="0"
                  required
                  class="form-input"
                  placeholder="0"
              />
            </div>
          </div>

          <div class="mt-6 flex justify-end space-x-3">
            <button
                type="button"
                @click="showAddItemModal = false"
                class="btn-secondary"
            >
              Cancel
            </button>
            <button
                type="submit"
                class="btn-primary"
            >
              Add Item
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
import { format } from 'date-fns';
import {
  PlusIcon,
  ShoppingBagIcon,
  ChevronLeftIcon,
  CheckIcon,
  TrashIcon,
  EllipsisVerticalIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline';
import {
  Menu,
  MenuButton,
  MenuItem,
  MenuItems
} from '@headlessui/vue';

import { useShoppingListsStore } from '@/stores/shoppingLists';
import { useIngredientsStore } from '@/stores/ingredients';
import type { ShoppingListItem, ShoppingListItemCreateDto, Ingredient } from '@/types';

const route = useRoute();
const router = useRouter();
const toast = useToast();
const shoppingListsStore = useShoppingListsStore();
const ingredientsStore = useIngredientsStore();

// Modal state
const showAddItemModal = ref(false);

// Available ingredients
const availableIngredients = ref<Ingredient[]>([]);

// Form state
const itemForm = reactive<ShoppingListItemCreateDto>({
  shoppingListId: 0,
  ingredientId: 0,
  amount: 0,
  purchased: false
});

// Computed properties
const progressPercentage = computed(() => {
  if (!shoppingListsStore.currentShoppingList || (shoppingListsStore.currentShoppingList.totalItems || 0) === 0) {
    return 0;
  }
  return Math.round(((shoppingListsStore.currentShoppingList.purchasedItems || 0) / (shoppingListsStore.currentShoppingList.totalItems || 1)) * 100);
});

const remainingItems = computed(() => {
  if (!shoppingListsStore.currentShoppingList) return 0;
  return (shoppingListsStore.currentShoppingList.totalItems || 0) - (shoppingListsStore.currentShoppingList.purchasedItems || 0);
});

const hasCompletedItems = computed(() => {
  return (shoppingListsStore.currentShoppingList?.purchasedItems || 0) > 0;
});

const sortedItems = computed(() => {
  if (!shoppingListsStore.currentShoppingList) return [];

  // Sort items: unpurchased first, then purchased
  return [...shoppingListsStore.currentShoppingList.items].sort((a, b) => {
    if (a.purchased === b.purchased) {
      return (a.ingredientName || '').localeCompare(b.ingredientName || '');
    }
    return a.purchased ? 1 : -1;
  });
});

// Methods
const formatDate = (dateString: string) => {
  return format(new Date(dateString), 'MMM d, yyyy');
};

const toggleItem = async (item: ShoppingListItem) => {
  if (!shoppingListsStore.currentShoppingList) return;

  const success = await shoppingListsStore.toggleItem(
      shoppingListsStore.currentShoppingList.id,
      item.id,
      !item.purchased
  );

  if (!success) {
    toast.error('Failed to update item');
  }
};

const addItem = async () => {
  if (!shoppingListsStore.currentShoppingList) return;

  itemForm.shoppingListId = shoppingListsStore.currentShoppingList.id;

  const success = await shoppingListsStore.addShoppingListItem(
      shoppingListsStore.currentShoppingList.id,
      itemForm
  );

  if (success) {
    toast.success('Item added to shopping list');
    showAddItemModal.value = false;
    // Reset form
    itemForm.ingredientId = 0;
    itemForm.amount = 0;
  } else {
    toast.error('Failed to add item to shopping list');
  }
};

const editItem = (item: ShoppingListItem) => {
  // Would implement edit functionality
  toast.info('Edit item functionality not yet implemented');
};

const deleteItem = async (item: ShoppingListItem) => {
  if (confirm(`Are you sure you want to remove "${item.ingredientName}" from your shopping list?`)) {
    toast.info('Delete item functionality not yet implemented');
  }
};

const removeCompletedItems = async () => {
  if (!shoppingListsStore.currentShoppingList) return;

  if (confirm('Are you sure you want to remove all completed items?')) {
    const success = await shoppingListsStore.removeCompletedItems(shoppingListsStore.currentShoppingList.id);

    if (success) {
      toast.success('Completed items removed');
    } else {
      toast.error('Failed to remove completed items');
    }
  }
};

// Lifecycle
onMounted(async () => {
  const id = parseInt(route.params.id as string);

  await Promise.all([
    shoppingListsStore.fetchShoppingList(id),
    ingredientsStore.fetchIngredients()
  ]);

  availableIngredients.value = ingredientsStore.ingredients;
});
</script>

<style scoped>
.line-through {
  text-decoration: line-through;
}
</style>