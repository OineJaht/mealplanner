// src/types/recipe.ts
import type { Category } from './index';

export interface Recipe {
    id: number;
    name: string;
    description: string;
    userId: string;
    defaultServings: number;
    prepTimeMinutes: number;
    createdAt: string;
    isFavorite: boolean;
    categories: Category[];
}

export interface RecipeCreateDto {
    name: string;
    description: string;
    defaultServings: number;
    prepTimeMinutes: number;
}