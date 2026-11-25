import { defineStore } from 'pinia';
import { ref } from 'vue';
import { ApiService } from '@/services/api';
import type { Ingredient, IngredientDetail, IngredientCreateDto, Category } from '@/types';

export const useIngredientsStore = defineStore('ingredients', () => {
    const ingredients = ref<Ingredient[]>([]);
    const categories = ref<Category[]>([]);
    const isLoading = ref(false);
    const error = ref<string>('');

    async function fetchIngredients(): Promise<void> {
        try {
            isLoading.value = true;
            const response = await ApiService.getIngredients();
            if (response.success && response.data) {
                ingredients.value = response.data;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch ingredients';
        } finally {
            isLoading.value = false;
        }
    }

    async function searchIngredients(term: string): Promise<Ingredient[]> {
        try {
            const response = await ApiService.searchIngredients(term);
            if (response.success && response.data) {
                return response.data;
            }
            return [];
        } catch (err: any) {
            error.value = err.message || 'Failed to search ingredients';
            return [];
        }
    }

    async function fetchCategories(): Promise<void> {
        try {
            const response = await ApiService.getCategories();
            if (response.success && response.data) {
                categories.value = response.data;
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch categories';
        }
    }

    async function createIngredient(ingredient: IngredientCreateDto): Promise<Ingredient | null> {
        try {
            const response = await ApiService.createIngredient(ingredient);
            if (response.success && response.data) {
                ingredients.value.push(response.data);
                return response.data;
            }
            return null;
        } catch (err: any) {
            error.value = err.message || 'Failed to create ingredient';
            return null;
        }
    }

    return {
        ingredients,
        categories,
        isLoading,
        error,
        fetchIngredients,
        searchIngredients,
        fetchCategories,
        createIngredient
    };
});