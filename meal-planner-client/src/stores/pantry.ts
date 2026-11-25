import { defineStore } from 'pinia';
import { ref } from 'vue';
import { ApiService } from '@/services/api';
import type { UserPantryItem, UserPantryItemCreateDto } from '@/types';

export const usePantryStore = defineStore('pantry', () => {
    const pantryItems = ref<UserPantryItem[]>([]);
    const expiringItems = ref<UserPantryItem[]>([]);
    const lowStockItems = ref<UserPantryItem[]>([]);
    const isLoading = ref(false);
    const error = ref<string>('');

    async function fetchPantryItems(): Promise<void> {
        try {
            isLoading.value = true;
            const response = await ApiService.getUserPantryItems();
            if (response.success && response.data) {
                pantryItems.value = response.data;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch pantry items';
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchExpiringItems(daysThreshold: number = 7): Promise<void> {
        try {
            const response = await ApiService.getExpiringItems(daysThreshold);
            if (response.success && response.data) {
                expiringItems.value = response.data;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch expiring items';
        }
    }

    async function fetchLowStockItems(threshold: number = 5): Promise<void> {
        try {
            const response = await ApiService.getLowStockItems(threshold);
            if (response.success && response.data) {
                lowStockItems.value = response.data;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch low stock items';
        }
    }

    async function addPantryItem(item: UserPantryItemCreateDto): Promise<UserPantryItem | null> {
        try {
            const response = await ApiService.addPantryItem(item);
            if (response.success && response.data) {
                pantryItems.value.push(response.data);
                return response.data;
            }
            return null;
        } catch (err: any) {
            error.value = err.message || 'Failed to add pantry item';
            return null;
        }
    }

    async function updatePantryItem(id: number, item: UserPantryItem): Promise<boolean> {
        try {
            const response = await ApiService.updatePantryItem(id, item);
            if (response.success && response.data) {
                const index = pantryItems.value.findIndex(i => i.id === id);
                if (index !== -1) {
                    pantryItems.value[index] = response.data;
                }
                return true;
            }
            return false;
        } catch (err: any) {
            error.value = err.message || 'Failed to update pantry item';
            return false;
        }
    }

    async function deletePantryItem(id: number): Promise<boolean> {
        try {
            const response = await ApiService.deletePantryItem(id);
            if (response.success) {
                pantryItems.value = pantryItems.value.filter(i => i.id !== id);
                return true;
            }
            return false;
        } catch (err: any) {
            error.value = err.message || 'Failed to delete pantry item';
            return false;
        }
    }

    return {
        pantryItems,
        expiringItems,
        lowStockItems,
        isLoading,
        error,
        fetchPantryItems,
        fetchExpiringItems,
        fetchLowStockItems,
        addPantryItem,
        updatePantryItem,
        deletePantryItem
    };
});