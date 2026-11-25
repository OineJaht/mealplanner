<!-- src/views/Pantry.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Pantry</h1>
            <p class="mt-2 text-sm text-gray-600">
              Manage your ingredient inventory
            </p>
          </div>
          <button
              @click="showAddModal = true"
              class="btn-primary"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            Add Item
          </button>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="px-4 py-5 sm:p-6">
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-4">
            <div>
              <label for="search" class="block text-sm font-medium text-gray-700 mb-1">
                Search Items
              </label>
              <div class="relative">
                <input
                    id="search"
                    v-model="searchTerm"
                    type="text"
                    class="form-input pl-10"
                    placeholder="Search pantry items..."
                />
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
                </div>
              </div>
            </div>

            <div>
              <label for="location" class="block text-sm font-medium text-gray-700 mb-1">
                Location
              </label>
              <select
                  id="location"
                  v-model="selectedLocation"
                  class="form-input"
              >
                <option value="">All Locations</option>
                <option v-for="location in uniqueLocations" :key="location" :value="location">
                  {{ location }}
                </option>
              </select>
            </div>

            <div>
              <label for="status" class="block text-sm font-medium text-gray-700 mb-1">
                Status
              </label>
              <select
                  id="status"
                  v-model="selectedStatus"
                  class="form-input"
              >
                <option value="">All Items</option>
                <option value="expiring">Expiring Soon</option>
                <option value="low-stock">Low Stock</option>
                <option value="fresh">Fresh</option>
              </select>
            </div>

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
                <option value="expiry">Expiry Date</option>
                <option value="amount">Amount</option>
                <option value="location">Location</option>
              </select>
            </div>
          </div>

          <!-- Clear filters button -->
          <div class="mt-4 flex justify-end">
            <button
                v-if="searchTerm || selectedLocation || selectedStatus || sortBy !== 'name'"
                @click="clearFilters"
                class="text-sm text-gray-500 hover:text-gray-700 underline"
            >
              Clear all filters
            </button>
          </div>
        </div>
      </div>

      <!-- Quick Stats -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-3 mb-6">
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <ArchiveBoxIcon class="h-6 w-6 text-gray-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Total Items</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ pantryStore.pantryItems.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <ExclamationTriangleIcon class="h-6 w-6 text-yellow-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Expiring Soon</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ pantryStore.expiringItems.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <ExclamationCircleIcon class="h-6 w-6 text-red-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Low Stock</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ pantryStore.lowStockItems.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pantry Items -->
      <div v-if="pantryStore.isLoading" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <div v-for="i in 6" :key="i" class="animate-pulse">
          <div class="bg-white p-4 rounded-lg shadow">
            <div class="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
            <div class="h-3 bg-gray-200 rounded w-1/2 mb-2"></div>
            <div class="h-3 bg-gray-200 rounded w-1/3"></div>
          </div>
        </div>
      </div>

      <div v-else-if="filteredItems.length === 0" class="text-center py-12">
        <ArchiveBoxIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">
          {{ pantryStore.pantryItems.length === 0 ? 'No pantry items yet' : 'No items match your filters' }}
        </h3>
        <p class="mt-1 text-sm text-gray-500">
          {{ pantryStore.pantryItems.length === 0
            ? 'Get started by adding items to your pantry.'
            : 'Try adjusting your search terms or filters.'
          }}
        </p>
        <div class="mt-6">
          <button
              v-if="pantryStore.pantryItems.length === 0"
              @click="showAddModal = true"
              class="btn-primary"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            Add Your First Item
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
            v-for="item in filteredItems"
            :key="item.id"
            class="bg-white overflow-hidden shadow rounded-lg hover:shadow-md transition-shadow"
        >
          <div class="p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1 min-w-0">
                <h3 class="text-lg font-medium text-gray-900 truncate">
                  {{ item.ingredientName }}
                </h3>
                <div class="mt-1 flex items-center space-x-4 text-sm text-gray-500">
                  <span>{{ item.amountInStock }} {{ item.unit }}</span>
                  <span v-if="item.location">📍 {{ item.location }}</span>
                </div>
                <div class="mt-2">
                  <span :class="[
                    'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    getExpiryStatus(item.expiryDate).class
                  ]">
                    {{ getExpiryStatus(item.expiryDate).text }}
                  </span>
                </div>
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
                        Delete Item
                      </button>
                    </MenuItem>
                  </MenuItems>
                </transition>
              </Menu>
            </div>

            <div class="mt-3 text-xs text-gray-500">
              Expires: {{ formatDate(item.expiryDate) }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add/Edit Item Modal -->
    <div v-if="showAddModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-screen overflow-y-auto">
        <h3 class="text-lg font-medium text-gray-900 mb-4">
          {{ editingItem ? 'Edit Pantry Item' : 'Add Pantry Item' }}
        </h3>
        <form @submit.prevent="saveItem">
          <div class="space-y-4">
            <div>
              <label class="form-label">Ingredient</label>
              <input
                  v-if="editingItem"
                  :value="editingItem.ingredientName"
                  type="text"
                  disabled
                  class="form-input bg-gray-50"
              />
              <select
                  v-else
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

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="form-label">Amount</label>
                <input
                    v-model.number="itemForm.amountInStock"
                    type="number"
                    step="0.1"
                    min="0"
                    required
                    class="form-input"
                    placeholder="0"
                />
              </div>
              <div>
                <label class="form-label">Unit</label>
                <input
                    v-model="itemForm.unit"
                    type="text"
                    required
                    class="form-input"
                    placeholder="e.g., kg, pcs, ml"
                />
              </div>
            </div>

            <div>
              <label class="form-label">Location</label>
              <input
                  v-model="itemForm.location"
                  type="text"
                  class="form-input"
                  placeholder="e.g., Fridge, Pantry, Freezer"
              />
            </div>

            <div>
              <label class="form-label">Expiry Date</label>
              <input
                  v-model="itemForm.expiryDate"
                  type="date"
                  required
                  class="form-input"
              />
            </div>
          </div>

          <div class="mt-6 flex justify-end space-x-3">
            <button
                type="button"
                @click="cancelEdit"
                class="btn-secondary"
            >
              Cancel
            </button>
            <button
                type="submit"
                class="btn-primary"
            >
              {{ editingItem ? 'Update' : 'Add' }} Item
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
import { format, differenceInDays } from 'date-fns';
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArchiveBoxIcon,
  ExclamationTriangleIcon,
  ExclamationCircleIcon,
  EllipsisVerticalIcon
} from '@heroicons/vue/24/outline';
import {
  Menu,
  MenuButton,
  MenuItem,
  MenuItems
} from '@headlessui/vue';

import { usePantryStore } from '@/stores/pantry';
import { useIngredientsStore } from '@/stores/ingredients';
import type { UserPantryItem, UserPantryItemCreateDto, Ingredient } from '@/types';

const toast = useToast();
const pantryStore = usePantryStore();
const ingredientsStore = useIngredientsStore();

// Search and filter state
const searchTerm = ref('');
const selectedLocation = ref('');
const selectedStatus = ref('');
const sortBy = ref('name');

// Modal state
const showAddModal = ref(false);
const editingItem = ref<UserPantryItem | null>(null);

// Form state
const itemForm = reactive<UserPantryItemCreateDto>({
  ingredientId: 0,
  amountInStock: 0,
  unit: '',
  location: '',
  expiryDate: ''
});

// Available ingredients for selection
const availableIngredients = ref<Ingredient[]>([]);

// Computed properties
const uniqueLocations = computed(() => {
  const locations = pantryStore.pantryItems
      .map(item => item.location)
      .filter(location => location && location.trim() !== '');
  return [...new Set(locations)].sort();
});

const filteredItems = computed(() => {
  let filtered = [...pantryStore.pantryItems];

  // Apply search filter
  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase();
    filtered = filtered.filter(item =>
        item.ingredientName?.toLowerCase().includes(term) ||
        item.location?.toLowerCase().includes(term)
    );
  }

  // Apply location filter
  if (selectedLocation.value) {
    filtered = filtered.filter(item => item.location === selectedLocation.value);
  }

  // Apply status filter
  if (selectedStatus.value) {
    const today = new Date();
    filtered = filtered.filter(item => {
      const daysUntilExpiry = differenceInDays(new Date(item.expiryDate), today);
      switch (selectedStatus.value) {
        case 'expiring':
          return daysUntilExpiry <= 7 && daysUntilExpiry >= 0;
        case 'low-stock':
          return item.amountInStock <= 5;
        case 'fresh':
          return daysUntilExpiry > 7;
        default:
          return true;
      }
    });
  }

  // Apply sorting
  filtered.sort((a, b) => {
    switch (sortBy.value) {
      case 'name':
        return (a.ingredientName || '').localeCompare(b.ingredientName || '');
      case 'expiry':
        return new Date(a.expiryDate).getTime() - new Date(b.expiryDate).getTime();
      case 'amount':
        return b.amountInStock - a.amountInStock;
      case 'location':
        return (a.location || '').localeCompare(b.location || '');
      default:
        return 0;
    }
  });

  return filtered;
});

// Methods
const clearFilters = () => {
  searchTerm.value = '';
  selectedLocation.value = '';
  selectedStatus.value = '';
  sortBy.value = 'name';
};

const getExpiryStatus = (expiryDate: string) => {
  const today = new Date();
  const expiry = new Date(expiryDate);
  const daysUntilExpiry = differenceInDays(expiry, today);

  if (daysUntilExpiry < 0) {
    return { text: 'Expired', class: 'bg-red-100 text-red-800' };
  } else if (daysUntilExpiry <= 3) {
    return { text: 'Expires Soon', class: 'bg-red-100 text-red-800' };
  } else if (daysUntilExpiry <= 7) {
    return { text: 'Expiring', class: 'bg-yellow-100 text-yellow-800' };
  } else {
    return { text: 'Fresh', class: 'bg-green-100 text-green-800' };
  }
};

const formatDate = (dateString: string) => {
  return format(new Date(dateString), 'MMM d, yyyy');
};

const resetForm = () => {
  itemForm.ingredientId = 0;
  itemForm.amountInStock = 0;
  itemForm.unit = '';
  itemForm.location = '';
  itemForm.expiryDate = '';
};

const editItem = (item: UserPantryItem) => {
  editingItem.value = item;
  itemForm.ingredientId = item.ingredientId;
  itemForm.amountInStock = item.amountInStock;
  itemForm.unit = item.unit || '';
  itemForm.location = item.location || '';
  itemForm.expiryDate = format(new Date(item.expiryDate), 'yyyy-MM-dd');
  showAddModal.value = true;
};

const cancelEdit = () => {
  showAddModal.value = false;
  editingItem.value = null;
  resetForm();
};

const saveItem = async () => {
  if (editingItem.value) {
    // Update existing item
    const updatedItem: UserPantryItem = {
      ...editingItem.value,
      amountInStock: itemForm.amountInStock,
      unit: itemForm.unit,
      location: itemForm.location,
      expiryDate: itemForm.expiryDate
    };

    const success = await pantryStore.updatePantryItem(editingItem.value.id, updatedItem);
    if (success) {
      toast.success('Pantry item updated successfully');
      cancelEdit();
    } else {
      toast.error('Failed to update pantry item');
    }
  } else {
    // Create new item
    const result = await pantryStore.addPantryItem(itemForm);
    if (result) {
      toast.success('Item added to pantry');
      cancelEdit();
    } else {
      toast.error('Failed to add item to pantry');
    }
  }
};

const deleteItem = async (item: UserPantryItem) => {
  if (confirm(`Are you sure you want to delete "${item.ingredientName}" from your pantry?`)) {
    const success = await pantryStore.deletePantryItem(item.id);
    if (success) {
      toast.success('Item removed from pantry');
    } else {
      toast.error('Failed to remove item from pantry');
    }
  }
};

// Lifecycle
onMounted(async () => {
  await Promise.all([
    pantryStore.fetchPantryItems(),
    pantryStore.fetchExpiringItems(),
    pantryStore.fetchLowStockItems(),
    ingredientsStore.fetchIngredients()
  ]);

  availableIngredients.value = ingredientsStore.ingredients;
});
</script>