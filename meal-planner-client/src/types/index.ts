// src/types/index.ts

export interface ApiResponse<T> {
    success: boolean;
    message: string;
    data?: T;
}

// Base DTO
export interface BaseDTO {
    id: number;
}

// Auth Types
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

export interface RefreshTokenDto {
    accessToken: string;
    refreshToken: string;
}

// Category Types - MOVED TO TOP
export interface Category extends BaseDTO {
    name: string;
    description?: string;
}

export interface CategoryCreateDto {
    name: string;
    description?: string;
}

// Recipe Types
export interface Recipe extends BaseDTO {
    name: string;
    description?: string;
    defaultServings: number;
    prepTimeMinutes: number;
    isFavorite: boolean;
    createdAt: string;
    imageUrl?: string;
    categories: Category[];
}

export interface RecipeDetail extends Recipe {
    instructions?: string;
    ingredients: RecipeIngredient[];
    cookingSteps: CookingStep[];
    nutrition?: RecipeNutrition;
}

export interface RecipeIngredient extends BaseDTO {
    ingredientId: number;
    ingredientName?: string;
    amount: number;
    unit?: string;
    isAvailable?: boolean;
}

export interface CookingStep extends BaseDTO {
    stepNumber: number;
    instruction?: string;
    timingMinutes: number;
}

export interface RecipeNutrition extends BaseDTO {
    totalCalories: number;
    totalProteinG: number;
    totalCarbsG: number;
    totalFatG: number;
    totalFiberG: number;
    totalSugarG: number;
    totalSodiumG: number;
    caloriesPerServing: number;
}

export interface RecipeCreateDto {
    name: string;
    description?: string;
    instructions: string;
    defaultServings: number;
    prepTimeMinutes: number;
    isFavorite: boolean;
    categoryIds?: number[];
    ingredients?: RecipeIngredientCreateDto[];
}

export interface RecipeIngredientCreateDto {
    ingredientId: number;
    amount: number;
    unit: string;
}

export interface RecipeIngredientUpdateDto extends RecipeIngredientCreateDto {
    id: number;
}

export interface RecipeUpdateDto extends RecipeCreateDto {
    id: number;
    imageUrl?: string;
    ingredients: RecipeIngredientUpdateDto[];
}

export interface RecipeMatch {
    recipe?: Recipe;
    matchPercentage: number;
    missingIngredients: Ingredient[];
}

// Ingredient Types
export interface Ingredient extends BaseDTO {
    name: string;
    category?: string;
}

export interface IngredientDetail extends Ingredient {
    nutrition?: IngredientNutrition;
}

export interface IngredientNutrition extends BaseDTO {
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

export interface IngredientNutritionCreateDto {
    caloriesPer100g: number;
    proteinG: number;
    carbsG: number;
    fatG: number;
    fiberG: number;
    sugarG: number;
    sodiumMg: number;
}

// Meal Plan Types
export interface MealPlan extends BaseDTO {
    name: string;
    startDate: string;
    endDate: string;
}

export interface MealPlanDetail extends MealPlan {
    mealPlanItems: MealPlanItem[];
}

export interface MealPlanItem extends BaseDTO {
    recipeId: number;
    recipeName?: string;
    plannedDate: string;
    mealType?: string;
    servings: number;
}

export interface MealPlanCreateDto {
    name: string;
    startDate: string;
    endDate: string;
}

export interface MealPlanItemCreateDto {
    mealPlanId: number;
    recipeId: number;
    plannedDate: string;
    mealType: string;
    servings: number;
}

// Meal Plan Item Update DTO
export interface MealPlanItemUpdateDto {
    id: number;
    mealPlanId: number;
    recipeId: number;
    plannedDate: string;
    mealType: string;
    servings: number;
}

// Category Update DTO
export interface CategoryUpdateDto {
    id: number;
    name: string;
    description?: string;
}

// Ingredient Update DTO
export interface IngredientUpdateDto {
    id: number;
    name: string;
    category: string;
    nutrition?: IngredientNutritionUpdateDto;
}

// Ingredient Nutrition Update DTO (if not already defined)
export interface IngredientNutritionUpdateDto {
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



// Shopping List Types
export interface ShoppingList extends BaseDTO {
    name: string;
    createdDate: string;
}

export interface RecipeFormDto extends Omit<RecipeCreateDto, 'ingredients'> {
    ingredients: RecipeIngredientCreateDto[];
}

// Shopping List Update DTO
export interface ShoppingListUpdateDto {
    id: number;
    name: string;
    createdDate: string;
}

// Shopping List Item Update DTO
export interface ShoppingListItemUpdateDto {
    id: number;
    shoppingListId: number;
    ingredientId: number;
    amount: number;
    purchased: boolean;
}

export interface ShoppingListDetail extends ShoppingList {
    items: ShoppingListItem[];
    totalItems: number;
    purchasedItems: number;
}

export interface ShoppingListItem extends BaseDTO {
    ingredientId: number;
    ingredientName?: string;
    amount: number;
    purchased: boolean;
}

export interface ShoppingListCreateDto {
    name: string;
}

export interface ShoppingListItemCreateDto {
    shoppingListId: number;
    ingredientId: number;
    amount: number;
    purchased: boolean;
}

// Pantry Types
export interface UserPantryItem extends BaseDTO {
    ingredientId: number;
    ingredientName?: string;
    amountInStock: number;
    unit?: string;
    location?: string;
    expiryDate: string;
}

export interface UserPantryItemCreateDto {
    ingredientId: number;
    amountInStock: number;
    unit: string;
    location: string;
    expiryDate: string;
}

// Recipe Finder Types
export interface FindRecipesRequest {
    ingredientIds: number[];
    matchAll: boolean;
}

export interface AvailabilityRequest {
    availableIngredientIds: number[];
}

export interface RecipeAvailabilityResponse {
    recipe?: RecipeDetail;
    availableIngredients: number[];
    missingIngredients: number[];
}

// Shopping List Update DTOs
export interface ShoppingListUpdateDto {
    id: number;
    name: string;
    createdDate: string;
}

// Recipe Update DTOs (if not already present)
export interface RecipeIngredientUpdateDto extends RecipeIngredientCreateDto {
    id: number;
}

export interface RecipeUpdateDto extends RecipeCreateDto {
    id: number;
    imageUrl?: string;
    ingredients: RecipeIngredientUpdateDto[];
}

// Meal Plan Update DTOs (if not already present)
export interface MealPlanUpdateDto extends MealPlanCreateDto {
    id: number;
}

export interface MealPlanItemUpdateDto extends MealPlanItemCreateDto {
    id: number;
}

// Category Update DTOs (if not already present)
export interface CategoryUpdateDto extends CategoryCreateDto {
    id: number;
}

// Ingredient Update DTOs (if not already present)
export interface IngredientUpdateDto extends IngredientCreateDto {
    id: number;
}

export interface IngredientNutritionUpdateDto extends IngredientNutritionCreateDto {
    id: number;
    ingredientId: number;
}

// Shopping List Item Update DTOs
export interface ShoppingListItemUpdateDto {
    id: number;
    amount: number;
    purchased: boolean;
}


// Utility Types
export type MealType = 'breakfast' | 'lunch' | 'dinner' | 'snack';

export interface User {
    id: string;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
}