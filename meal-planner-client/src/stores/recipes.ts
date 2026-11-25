// src/stores/recipes.ts - Updated with debug logging
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { ApiService } from '@/services/api';
import type { Recipe, RecipeDetail, RecipeCreateDto, RecipeUpdateDto, RecipeMatch, FindRecipesRequest } from '@/types';

export const useRecipesStore = defineStore('recipes', () => {
    const recipes = ref<Recipe[]>([]);
    const currentRecipe = ref<RecipeDetail | null>(null);
    const favoriteRecipes = ref<Recipe[]>([]);
    const recentRecipes = ref<Recipe[]>([]);
    const recipeMatches = ref<RecipeMatch[]>([]);
    const isLoading = ref(false);
    const error = ref<string>('');

    async function fetchRecipes(): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';
            const response = await ApiService.getRecipes();
            if (response.success && response.data) {
                recipes.value = response.data;
            } else {
                error.value = response.message || 'Failed to fetch recipes';
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch recipes';
            console.error('Error fetching recipes:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchRecipe(id: number): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';
            const response = await ApiService.getRecipe(id);
            if (response.success && response.data) {
                currentRecipe.value = response.data;
            } else {
                error.value = response.message || 'Failed to fetch recipe';
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch recipe';
            console.error('Error fetching recipe:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchFavoriteRecipes(): Promise<void> {
        try {
            error.value = '';
            const response = await ApiService.getFavoriteRecipes();
            if (response.success && response.data) {
                favoriteRecipes.value = response.data;
            } else {
                error.value = response.message || 'Failed to fetch favorite recipes';
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch favorite recipes';
            console.error('Error fetching favorite recipes:', err);
        }
    }

    async function fetchRecentRecipes(): Promise<void> {
        try {
            error.value = '';
            const response = await ApiService.getRecentRecipes();
            if (response.success && response.data) {
                recentRecipes.value = response.data;
            } else {
                error.value = response.message || 'Failed to fetch recent recipes';
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to fetch recent recipes';
            console.error('Error fetching recent recipes:', err);
        }
    }

    async function createRecipe(recipe: RecipeCreateDto): Promise<Recipe | null> {
        try {
            isLoading.value = true;
            error.value = '';

            console.log('Store: Creating recipe with data:', JSON.stringify(recipe, null, 2));

            const response = await ApiService.createRecipe(recipe);

            console.log('Store: API response:', JSON.stringify(response, null, 2));

            if (response.success && response.data) {
                recipes.value.push(response.data);
                return response.data;
            } else {
                error.value = response.message || 'Failed to create recipe';
                console.error('Store: Recipe creation failed:', response.message);
                return null;
            }
        } catch (err: any) {
            error.value = err.response?.data?.message || err.message || 'Failed to create recipe';
            console.error('Store: Error creating recipe:', err);

            // Log the full error response for debugging
            if (err.response) {
                console.error('Store: Error response status:', err.response.status);
                console.error('Store: Error response data:', err.response.data);
                console.error('Store: Error response headers:', err.response.headers);
            }

            return null;
        } finally {
            isLoading.value = false;
        }
    }

    async function updateRecipe(id: number, recipe: RecipeUpdateDto): Promise<Recipe | null> {
        try {
            isLoading.value = true;
            error.value = '';

            console.log('Store: Updating recipe with data:', JSON.stringify(recipe, null, 2));

            const response = await ApiService.updateRecipe(id, recipe);

            console.log('Store: Update API response:', JSON.stringify(response, null, 2));

            if (response.success && response.data) {
                const index = recipes.value.findIndex(r => r.id === id);
                if (index !== -1) {
                    recipes.value[index] = response.data;
                }
                return response.data;
            } else {
                error.value = response.message || 'Failed to update recipe';
                console.error('Store: Recipe update failed:', response.message);
                return null;
            }
        } catch (err: any) {
            error.value = err.response?.data?.message || err.message || 'Failed to update recipe';
            console.error('Store: Error updating recipe:', err);

            // Log the full error response for debugging
            if (err.response) {
                console.error('Store: Update error response status:', err.response.status);
                console.error('Store: Update error response data:', err.response.data);
            }

            return null;
        } finally {
            isLoading.value = false;
        }
    }

    async function deleteRecipe(id: number): Promise<boolean> {
        try {
            isLoading.value = true;
            error.value = '';
            const response = await ApiService.deleteRecipe(id);
            if (response.success) {
                recipes.value = recipes.value.filter(r => r.id !== id);
                return true;
            } else {
                error.value = response.message || 'Failed to delete recipe';
                return false;
            }
        } catch (err: any) {
            error.value = err.response?.data?.message || err.message || 'Failed to delete recipe';
            console.error('Store: Error deleting recipe:', err);
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    async function findRecipes(request: FindRecipesRequest): Promise<void> {
        try {
            isLoading.value = true;
            error.value = '';
            const response = await ApiService.findRecipes(request);
            if (response.success && response.data) {
                recipeMatches.value = response.data;
            } else {
                error.value = response.message || 'Failed to find recipes';
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to find recipes';
            console.error('Error finding recipes:', err);
        } finally {
            isLoading.value = false;
        }
    }

    return {
        recipes,
        currentRecipe,
        favoriteRecipes,
        recentRecipes,
        recipeMatches,
        isLoading,
        error,
        fetchRecipes,
        fetchRecipe,
        fetchFavoriteRecipes,
        fetchRecentRecipes,
        createRecipe,
        updateRecipe,
        deleteRecipe,
        findRecipes
    };
});