// src/stores/mealPlans.ts - Updated with debug logging
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { ApiService } from '@/services/api';
import type { MealPlan, MealPlanDetail, MealPlanCreateDto, MealPlanItem, MealPlanItemCreateDto } from '@/types';

export const useMealPlansStore = defineStore('mealPlans', () => {
    const mealPlans = ref<MealPlan[]>([]);
    const currentMealPlan = ref<MealPlanDetail | null>(null);
    const isLoading = ref(false);
    const error = ref<string>('');

    async function fetchMealPlans(): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';
            const response = await ApiService.getMealPlans();
            if (response.success && response.data) {
                mealPlans.value = response.data;
            } else {
                error.value = response.message || 'Failed to fetch meal plans';
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch meal plans';
            console.error('Error fetching meal plans:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchMealPlan(id: number): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';

            console.log('STORE DEBUG: Fetching meal plan with ID:', id);

            const response = await ApiService.getMealPlan(id);

            console.log('STORE DEBUG: Meal plan API response:', JSON.stringify(response, null, 2));

            if (response.success && response.data) {
                currentMealPlan.value = response.data;
                console.log('STORE DEBUG: Set current meal plan:', currentMealPlan.value);
            } else {
                error.value = response.message || 'Failed to fetch meal plan';
                console.error('STORE DEBUG: Failed to fetch meal plan:', response.message);
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch meal plan';
            console.error('STORE DEBUG: Error fetching meal plan:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchCurrentMealPlan(): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';
            const response = await ApiService.getCurrentMealPlan();
            if (response.success && response.data) {
                currentMealPlan.value = response.data;
            } else {
                error.value = response.message || 'Failed to fetch current meal plan';
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch current meal plan';
            console.error('Error fetching current meal plan:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function createMealPlan(mealPlan: MealPlanCreateDto): Promise<MealPlan | null> {
        try {
            isLoading.value = true;
            error.value = '';
            const response = await ApiService.createMealPlan(mealPlan);
            if (response.success && response.data) {
                mealPlans.value.push(response.data);
                return response.data;
            } else {
                error.value = response.message || 'Failed to create meal plan';
                return null;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to create meal plan';
            console.error('Error creating meal plan:', err);
            return null;
        } finally {
            isLoading.value = false;
        }
    }

    async function addMealPlanItem(mealPlanId: number, item: MealPlanItemCreateDto): Promise<boolean> {
        try {
            error.value = '';

            console.log('STORE DEBUG: Adding meal plan item');
            console.log('STORE DEBUG: Meal plan ID:', mealPlanId);
            console.log('STORE DEBUG: Item data:', JSON.stringify(item, null, 2));

            const response = await ApiService.addMealPlanItem(mealPlanId, item);

            console.log('STORE DEBUG: Add item API response:', JSON.stringify(response, null, 2));

            if (response.success) {
                // Instead of just refreshing, let's manually update the store first
                if (currentMealPlan.value?.id === mealPlanId && response.data) {
                    console.log('STORE DEBUG: Adding item to current meal plan store');
                    currentMealPlan.value.mealPlanItems.push(response.data);
                    console.log('STORE DEBUG: Updated meal plan items count:', currentMealPlan.value.mealPlanItems.length);
                }

                // Then refresh to get the latest data from server
                console.log('STORE DEBUG: Refreshing meal plan data from server');
                await fetchMealPlan(mealPlanId);

                return true;
            } else {
                error.value = response.message || 'Failed to add meal plan item';
                console.error('STORE DEBUG: Failed to add meal plan item:', response.message);
                return false;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to add meal plan item';
            console.error('STORE DEBUG: Error adding meal plan item:', err);

            // Log full error details
            if (err.response) {
                console.error('STORE DEBUG: Error response status:', err.response.status);
                console.error('STORE DEBUG: Error response data:', err.response.data);
            }

            return false;
        }
    }

    async function deleteMealPlanItem(mealPlanId: number, itemId: number): Promise<boolean> {
        try {
            error.value = '';

            console.log('STORE DEBUG: Deleting meal plan item');
            console.log('STORE DEBUG: Meal plan ID:', mealPlanId);
            console.log('STORE DEBUG: Item ID:', itemId);

            const response = await ApiService.deleteMealPlanItem(mealPlanId, itemId);

            console.log('STORE DEBUG: Delete item API response:', JSON.stringify(response, null, 2));

            if (response.success) {
                // Remove from local store immediately
                if (currentMealPlan.value?.id === mealPlanId) {
                    const index = currentMealPlan.value.mealPlanItems.findIndex(item => item.id === itemId);
                    if (index !== -1) {
                        currentMealPlan.value.mealPlanItems.splice(index, 1);
                        console.log('STORE DEBUG: Removed item from local store, new count:', currentMealPlan.value.mealPlanItems.length);
                    }
                }

                // Refresh to ensure consistency
                await fetchMealPlan(mealPlanId);
                return true;
            } else {
                error.value = response.message || 'Failed to delete meal plan item';
                console.error('STORE DEBUG: Failed to delete meal plan item:', response.message);
                return false;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to delete meal plan item';
            console.error('STORE DEBUG: Error deleting meal plan item:', err);
            return false;
        }
    }

    return {
        mealPlans,
        currentMealPlan,
        isLoading,
        error,
        fetchMealPlans,
        fetchMealPlan,
        fetchCurrentMealPlan,
        createMealPlan,
        addMealPlanItem,
        deleteMealPlanItem
    };
});