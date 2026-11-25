// src/stores/shoppingLists.ts - COMPLETE with API integration
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { ApiService } from '@/services/api';
import type {ShoppingList, ShoppingListDetail, ShoppingListCreateDto, ShoppingListItemCreateDto} from '@/types';

export const useShoppingListsStore = defineStore('shoppingLists', () => {
    const shoppingLists = ref<ShoppingList[]>([]);
    const currentShoppingList = ref<ShoppingListDetail | null>(null);
    const isLoading = ref(false);
    const error = ref<string>('');

    async function fetchShoppingLists(): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';
            console.log('Fetching shopping lists...');

            const response = await ApiService.getShoppingLists();
            console.log('Shopping lists response:', response);

            if (response.success && response.data) {
                shoppingLists.value = response.data;
                console.log('Shopping lists loaded:', response.data.length);
            } else {
                error.value = response.message || 'Failed to fetch shopping lists';
                console.error('Failed to fetch shopping lists:', response.message);
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch shopping lists';
            console.error('Error fetching shopping lists:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchShoppingList(id: number): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';
            console.log('Fetching shopping list:', id);

            const response = await ApiService.getShoppingList(id);
            console.log('Shopping list response:', response);

            if (response.success && response.data) {
                currentShoppingList.value = response.data;
                console.log('Shopping list loaded:', response.data);
            } else {
                error.value = response.message || 'Failed to fetch shopping list';
                console.error('Failed to fetch shopping list:', response.message);
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch shopping list';
            console.error('Error fetching shopping list:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function createShoppingList(shoppingList: ShoppingListCreateDto): Promise<ShoppingList | null> {
        try {
            isLoading.value = true;
            error.value = '';
            console.log('Creating shopping list:', shoppingList);

            const response = await ApiService.createShoppingList(shoppingList);
            console.log('Create shopping list response:', response);

            if (response.success && response.data) {
                shoppingLists.value.push(response.data);
                console.log('Shopping list created:', response.data);
                return response.data;
            } else {
                error.value = response.message || 'Failed to create shopping list';
                console.error('Failed to create shopping list:', response.message);
                return null;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to create shopping list';
            console.error('Error creating shopping list:', err);
            return null;
        } finally {
            isLoading.value = false;
        }
    }

    async function updateShoppingList(id: number, name: string): Promise<ShoppingList | null> {
        try {
            isLoading.value = true;
            error.value = '';
            console.log('Updating shopping list:', id, name);

            const response = await ApiService.updateShoppingList(id, { name });
            console.log('Update shopping list response:', response);

            if (response.success && response.data) {
                // Update local state
                const index = shoppingLists.value.findIndex(list => list.id === id);
                if (index !== -1) {
                    shoppingLists.value[index] = response.data;
                }

                // Update current shopping list if it's the one being edited
                if (currentShoppingList.value?.id === id) {
                    currentShoppingList.value = { ...currentShoppingList.value, name };
                }

                console.log('Shopping list updated:', response.data);
                return response.data;
            } else {
                error.value = response.message || 'Failed to update shopping list';
                console.error('Failed to update shopping list:', response.message);
                return null;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to update shopping list';
            console.error('Error updating shopping list:', err);
            return null;
        } finally {
            isLoading.value = false;
        }
    }

    async function deleteShoppingList(id: number): Promise<boolean> {
        try {
            isLoading.value = true;
            error.value = '';
            console.log('Deleting shopping list:', id);

            const response = await ApiService.deleteShoppingList(id);
            console.log('Delete shopping list response:', response);

            if (response.success) {
                // Remove from local state
                const index = shoppingLists.value.findIndex(list => list.id === id);
                if (index !== -1) {
                    shoppingLists.value.splice(index, 1);
                }

                // Clear current shopping list if it's the one being deleted
                if (currentShoppingList.value?.id === id) {
                    currentShoppingList.value = null;
                }

                console.log('Shopping list deleted successfully');
                return true;
            } else {
                error.value = response.message || 'Failed to delete shopping list';
                console.error('Failed to delete shopping list:', response.message);
                return false;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to delete shopping list';
            console.error('Error deleting shopping list:', err);
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    async function createFromMealPlan(mealPlanId: number, name: string): Promise<ShoppingListDetail | null> {
        try {
            isLoading.value = true;
            error.value = '';
            console.log('Creating shopping list from meal plan:', mealPlanId, name);

            const response = await ApiService.createShoppingListFromMealPlan(mealPlanId, name);
            console.log('Create from meal plan response:', response);

            if (response.success && response.data) {
                // Add to local lists (just the basic info for the list view)
                const basicList: ShoppingList = {
                    id: response.data.id,
                    name: response.data.name,
                    createdDate: response.data.createdDate
                };
                shoppingLists.value.push(basicList);

                console.log('Shopping list created from meal plan:', response.data);
                return response.data;
            } else {
                error.value = response.message || 'Failed to create shopping list from meal plan';
                console.error('Failed to create shopping list from meal plan:', response.message);
                return null;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to create shopping list from meal plan';
            console.error('Error creating shopping list from meal plan:', err);
            return null;
        } finally {
            isLoading.value = false;
        }
    }

    async function addShoppingListItem(listId: number, item: ShoppingListItemCreateDto): Promise<boolean> {
        try {
            error.value = '';
            console.log('Adding item to shopping list:', listId, item);

            const response = await ApiService.addShoppingListItem(listId, item);
            console.log('Add item response:', response);

            if (response.success) {
                // Refresh the shopping list to get updated data
                if (currentShoppingList.value?.id === listId) {
                    await fetchShoppingList(listId);
                }
                console.log('Item added to shopping list successfully');
                return true;
            } else {
                error.value = response.message || 'Failed to add item to shopping list';
                console.error('Failed to add item to shopping list:', response.message);
                return false;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to add item to shopping list';
            console.error('Error adding item to shopping list:', err);
            return false;
        }
    }

    async function toggleItem(listId: number, itemId: number, purchased: boolean): Promise<boolean> {
        try {
            error.value = '';
            console.log('Toggling item:', listId, itemId, purchased);

            const response = await ApiService.toggleShoppingListItem(listId, itemId, purchased);
            console.log('Toggle item response:', response);

            if (response.success) {
                // Update local state optimistically
                if (currentShoppingList.value?.id === listId) {
                    const item = currentShoppingList.value.items.find(i => i.id === itemId);
                    if (item) {
                        const wasAlreadyPurchased = item.purchased;
                        item.purchased = purchased;

                        // Update counts
                        if (purchased && !wasAlreadyPurchased) {
                            currentShoppingList.value.purchasedItems++;
                        } else if (!purchased && wasAlreadyPurchased) {
                            currentShoppingList.value.purchasedItems--;
                        }
                    }
                }
                console.log('Item toggled successfully');
                return true;
            } else {
                error.value = response.message || 'Failed to toggle item';
                console.error('Failed to toggle item:', response.message);
                return false;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to toggle item';
            console.error('Error toggling item:', err);
            return false;
        }
    }

    async function removeCompletedItems(listId: number): Promise<boolean> {
        try {
            error.value = '';
            console.log('Removing completed items from list:', listId);

            const response = await ApiService.removeCompletedItems(listId);
            console.log('Remove completed items response:', response);

            if (response.success) {
                // Refresh the shopping list to get updated data
                if (currentShoppingList.value?.id === listId) {
                    await fetchShoppingList(listId);
                }
                console.log('Completed items removed successfully, count:', response.data?.removedCount || 0);
                return true;
            } else {
                error.value = response.message || 'Failed to remove completed items';
                console.error('Failed to remove completed items:', response.message);
                return false;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to remove completed items';
            console.error('Error removing completed items:', err);
            return false;
        }
    }

    // Helper function to clear error state
    function clearError(): void {
        error.value = '';
    }

    return {
        // State
        shoppingLists,
        currentShoppingList,
        isLoading,
        error,

        // Actions
        fetchShoppingLists,
        fetchShoppingList,
        createShoppingList,
        updateShoppingList,
        deleteShoppingList,
        createFromMealPlan,
        addShoppingListItem,
        toggleItem,
        removeCompletedItems,
        clearError
    };
});