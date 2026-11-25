<!-- src/views/ShoppingLists.vue - FIXED VERSION with Delete Functionality -->
<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="px-4 py-6 sm:px-0">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Shopping Lists</h1>
            <p class="mt-2 text-sm text-gray-600">
              Organize your grocery shopping
            </p>
          </div>
          <button
              @click="showCreateModal = true"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            New Shopping List
          </button>
        </div>
      </div>

      <!-- Shopping Lists -->
      <div v-if="shoppingListsStore.isLoading" class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <div v-for="i in 6" :key="i" class="animate-pulse">
          <div class="bg-white rounded-lg shadow p-6">
            <div class="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
            <div class="h-3 bg-gray-200 rounded w-1/2 mb-4"></div>
            <div class="h-2 bg-gray-200 rounded w-full"></div>
          </div>
        </div>
      </div>

      <div v-else-if="shoppingListsStore.shoppingLists.length === 0" class="text-center py-12">
        <ShoppingBagIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">No shopping lists yet</h3>
        <p class="mt-1 text-sm text-gray-500">Create your first shopping list to get organized.</p>
        <div class="mt-6">
          <button
              @click="showCreateModal = true"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            Create Your First Shopping List
          </button>
        </div>
      </div>

      <div v-else class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <div
            v-for="list in shoppingListsStore.shoppingLists"
            :key="list.id"
            class="bg-white overflow-hidden shadow rounded-lg hover:shadow-md transition-shadow"
        >
          <div class="p-6">
            <div class="flex items-center justify-between">
              <div class="flex-1 min-w-0">
                <h3 class="text-lg font-medium text-gray-900 truncate">
                  <router-link
                      :to="`/shopping-lists/${list.id}`"
                      class="hover:text-green-600"
                  >
                    {{ list.name }}
                  </router-link>
                </h3>
                <p class="mt-1 text-sm text-gray-500">
                  Created {{ formatDate(list.createdDate) }}
                </p>
              </div>

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
                          :to="`/shopping-lists/${list.id}`"
                          :class="[active ? 'bg-gray-100' : '', 'block px-4 py-2 text-sm text-gray-700']"
                      >
                        View List
                      </router-link>
                    </MenuItem>
                    <MenuItem v-slot="{ active }">
                      <button
                          @click="editShoppingList(list)"
                          :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-gray-700']"
                      >
                        Edit List
                      </button>
                    </MenuItem>
                    <MenuItem v-slot="{ active }">
                      <button
                          @click="deleteShoppingList(list)"
                          :class="[active ? 'bg-gray-100' : '', 'block w-full text-left px-4 py-2 text-sm text-red-600']"
                      >
                        Delete List
                      </button>
                    </MenuItem>
                  </MenuItems>
                </transition>
              </Menu>
            </div>

            <!-- Progress indicator -->
            <div class="mt-4">
              <div class="flex items-center justify-between text-sm text-gray-500">
                <span>Items: {{ getItemCount(list) }}</span>
                <span>Status: Active</span>
              </div>
              <div class="mt-2 w-full bg-gray-200 rounded-full h-2">
                <div class="bg-green-600 h-2 rounded-full" :style="`width: ${getProgressPercentage(list)}%`"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Shopping List Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Create Shopping List</h3>
        <form @submit.prevent="createShoppingList">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">List Name</label>
              <input
                  v-model="newListName"
                  type="text"
                  required
                  class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm"
                  placeholder="e.g., Weekly Groceries"
              />
            </div>
          </div>

          <div class="mt-6 flex justify-end space-x-3">
            <button
                type="button"
                @click="showCreateModal = false"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              Cancel
            </button>
            <button
                type="submit"
                :disabled="!newListName.trim() || shoppingListsStore.isLoading"
                class="px-4 py-2 text-sm font-medium text-white bg-green-600 border border-transparent rounded-md shadow-sm hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ shoppingListsStore.isLoading ? 'Creating...' : 'Create List' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Edit Shopping List Modal -->
    <div v-if="showEditModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Edit Shopping List</h3>
        <form @submit.prevent="updateShoppingList">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">List Name</label>
              <input
                  v-model="editListName"
                  type="text"
                  required
                  class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm"
                  placeholder="e.g., Weekly Groceries"
              />
            </div>
          </div>

          <div class="mt-6 flex justify-end space-x-3">
            <button
                type="button"
                @click="showEditModal = false"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              Cancel
            </button>
            <button
                type="submit"
                :disabled="!editListName.trim() || shoppingListsStore.isLoading"
                class="px-4 py-2 text-sm font-medium text-white bg-green-600 border border-transparent rounded-md shadow-sm hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ shoppingListsStore.isLoading ? 'Updating...' : 'Update List' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <div class="flex items-center">
          <ExclamationTriangleIcon class="h-6 w-6 text-red-600 mr-3" />
          <h3 class="text-lg font-medium text-gray-900">Delete Shopping List</h3>
        </div>
        <div class="mt-2">
          <p class="text-sm text-gray-500">
            Are you sure you want to delete "{{ listToDelete?.name }}"? This action cannot be undone.
          </p>
        </div>

        <div class="mt-6 flex justify-end space-x-3">
          <button
              type="button"
              @click="showDeleteModal = false"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
          >
            Cancel
          </button>
          <button
              @click="confirmDelete"
              :disabled="shoppingListsStore.isLoading"
              class="px-4 py-2 text-sm font-medium text-white bg-red-600 border border-transparent rounded-md shadow-sm hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ shoppingListsStore.isLoading ? 'Deleting...' : 'Delete' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/vue';
import { EllipsisVerticalIcon, PlusIcon, ShoppingBagIcon, ExclamationTriangleIcon } from '@heroicons/vue/24/outline';
import { useShoppingListsStore } from '@/stores/shoppingLists';
import { useToast } from 'vue-toastification';
import type { ShoppingList } from '@/types';

// Store and composables
const shoppingListsStore = useShoppingListsStore();
const toast = useToast();

// Reactive state
const showCreateModal = ref(false);
const showEditModal = ref(false);
const showDeleteModal = ref(false);
const newListName = ref('');
const editListName = ref('');
const listToEdit = ref<ShoppingList | null>(null);
const listToDelete = ref<ShoppingList | null>(null);

// Methods
const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString();
};

const getItemCount = (list: ShoppingList): string => {
  // Since the basic ShoppingList type doesn't have items, return placeholder
  return '0';
};

const getProgressPercentage = (list: ShoppingList): number => {
  // Since the basic ShoppingList type doesn't have items, return 0
  return 0;
};

const createShoppingList = async (): Promise<void> => {
  if (!newListName.value.trim()) {
    toast.error('Please enter a list name');
    return;
  }

  try {
    const created = await shoppingListsStore.createShoppingList({
      name: newListName.value.trim()
    });

    if (created) {
      toast.success('Shopping list created successfully!');
      showCreateModal.value = false;
      newListName.value = '';
    } else {
      toast.error(shoppingListsStore.error || 'Failed to create shopping list');
    }
  } catch (error) {
    console.error('Error creating shopping list:', error);
    toast.error('An unexpected error occurred while creating the shopping list');
  }
};

const editShoppingList = (list: ShoppingList): void => {
  listToEdit.value = list;
  editListName.value = list.name;
  showEditModal.value = true;
};

const updateShoppingList = async (): Promise<void> => {
  if (!listToEdit.value || !editListName.value.trim()) {
    toast.error('Please enter a valid list name');
    return;
  }

  try {
    const updated = await shoppingListsStore.updateShoppingList(
        listToEdit.value.id,
        editListName.value.trim()
    );

    if (updated) {
      toast.success('Shopping list updated successfully!');
      showEditModal.value = false;
      listToEdit.value = null;
      editListName.value = '';
    } else {
      toast.error(shoppingListsStore.error || 'Failed to update shopping list');
    }
  } catch (error) {
    console.error('Error updating shopping list:', error);
    toast.error('An unexpected error occurred while updating the shopping list');
  }
};

const deleteShoppingList = (list: ShoppingList): void => {
  listToDelete.value = list;
  showDeleteModal.value = true;
};

const confirmDelete = async (): Promise<void> => {
  if (!listToDelete.value) return;

  try {
    const deleted = await shoppingListsStore.deleteShoppingList(listToDelete.value.id);

    if (deleted) {
      toast.success('Shopping list deleted successfully!');
      showDeleteModal.value = false;
      listToDelete.value = null;
    } else {
      toast.error(shoppingListsStore.error || 'Failed to delete shopping list');
    }
  } catch (error) {
    console.error('Error deleting shopping list:', error);
    toast.error('An unexpected error occurred while deleting the shopping list');
  }
};

// Lifecycle
onMounted(async () => {
  try {
    await shoppingListsStore.fetchShoppingLists();
  } catch (error) {
    console.error('Error loading shopping lists:', error);
    toast.error('Failed to load shopping lists');
  }
});
</script>

<style scoped>
/* Custom styles can go here if needed */
</style>