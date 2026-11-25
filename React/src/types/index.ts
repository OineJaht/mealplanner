// API Response wrapper
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
}

// Auth types
export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  username: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface TokenDto {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

// Ingredient types (based on your DTOs)
export interface Ingredient {
  id: number;
  name: string;
  category: string;
}

export interface IngredientDetail extends Ingredient {
  nutrition?: IngredientNutrition;
}

export interface IngredientNutrition {
  id: number;
  ingredientId: number;
  caloriesPer100g: number;
  proteinG: number;
  carbsG: number;
  fatG: number;
  fiberG: number;
  sugarG: number;
  sodiumMg: number;
}

export interface IngredientCreateDto {
  name: string;
  category: string;
  nutrition?: IngredientNutritionCreateDto;
}

export interface IngredientUpdateDto {
  id: number;
  name: string;
  category: string;
}

export interface IngredientNutritionCreateDto {
  caloriesPer100g: number;
  proteinG: number;
  carbsG: number;
  fatG: number;
  fiberG: number;
  sugarG: number;
  sodiumMg: number;
}