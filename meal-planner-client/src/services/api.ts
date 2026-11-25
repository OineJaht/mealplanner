// src/services/api.ts
import axios, { AxiosResponse, AxiosError, AxiosRequestConfig } from 'axios';
import type {
    ApiResponse,
    LoginDto,
    RegisterDto,
    TokenDto,
    RefreshTokenDto,
    Recipe,
    RecipeDetail,
    RecipeCreateDto,
    RecipeUpdateDto,
    RecipeMatch,
    FindRecipesRequest,
    AvailabilityRequest,
    RecipeAvailabilityResponse,
    Ingredient,
    IngredientDetail,
    IngredientCreateDto,
    Category,
    CategoryCreateDto,
    MealPlan,
    MealPlanDetail,
    MealPlanCreateDto,
    MealPlanItem,
    MealPlanItemCreateDto,
    ShoppingList,
    ShoppingListDetail,
    ShoppingListCreateDto,
    ShoppingListItem,
    ShoppingListItemCreateDto,
    UserPantryItem,
    UserPantryItemCreateDto,
    RecipeIngredientUpdateDto,
    RecipeIngredient,
    MealPlanItemUpdateDto,
    CategoryUpdateDto,
    IngredientUpdateDto
} from '@/types';

// Create axios instance
const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Token management
class TokenManager {
    private static readonly ACCESS_TOKEN_KEY = 'accessToken';
    private static readonly REFRESH_TOKEN_KEY = 'refreshToken';
    private static readonly TOKEN_EXPIRY_KEY = 'tokenExpiry';

    static getAccessToken(): string | null {
        return localStorage.getItem(this.ACCESS_TOKEN_KEY);
    }

    static setTokens(tokenData: TokenDto): void {
        localStorage.setItem(this.ACCESS_TOKEN_KEY, tokenData.accessToken);
        localStorage.setItem(this.REFRESH_TOKEN_KEY, tokenData.refreshToken);
        localStorage.setItem(this.TOKEN_EXPIRY_KEY, tokenData.expiresAt);
    }

    static getRefreshToken(): string | null {
        return localStorage.getItem(this.REFRESH_TOKEN_KEY);
    }

    static clearTokens(): void {
        localStorage.removeItem(this.ACCESS_TOKEN_KEY);
        localStorage.removeItem(this.REFRESH_TOKEN_KEY);
        localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
    }

    static isTokenExpired(): boolean {
        const expiry = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
        if (!expiry) return true;
        return new Date() >= new Date(expiry);
    }
}

// Request interceptor to add auth header
api.interceptors.request.use(
    (config) => {
        const token = TokenManager.getAccessToken();
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor for token refresh
api.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

        if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
            originalRequest._retry = true;

            const refreshToken = TokenManager.getRefreshToken();
            const accessToken = TokenManager.getAccessToken();

            if (refreshToken && accessToken) {
                try {
                    const response = await axios.post(`${import.meta.env.VITE_API_BASE_URL}/account/refresh-token`, {
                        accessToken,
                        refreshToken
                    });

                    const newTokens = response.data.data;
                    TokenManager.setTokens(newTokens);

                    // Retry original request
                    if (originalRequest.headers) {
                        originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;
                    }
                    return api(originalRequest);
                } catch (refreshError) {
                    TokenManager.clearTokens();
                    window.location.href = '/login';
                    return Promise.reject(refreshError);
                }
            }
        }

        return Promise.reject(error);
    }
);

// API Service Class
export class ApiService {
    // Authentication
    static async login(credentials: LoginDto): Promise<ApiResponse<TokenDto>> {
        const response: AxiosResponse<ApiResponse<TokenDto>> = await api.post('/account/login', credentials);
        return response.data;
    }

    static async register(userData: RegisterDto): Promise<ApiResponse<TokenDto>> {
        const response: AxiosResponse<ApiResponse<TokenDto>> = await api.post('/account/register', userData);
        return response.data;
    }

    static async logout(): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.post('/account/logout');
        return response.data;
    }

    static async refreshToken(tokens: RefreshTokenDto): Promise<ApiResponse<TokenDto>> {
        const response: AxiosResponse<ApiResponse<TokenDto>> = await api.post('/account/refresh-token', tokens);
        return response.data;
    }

    // Recipes - FIXED: Use correct versioned endpoints
    static async getRecipes(): Promise<ApiResponse<Recipe[]>> {
        const response: AxiosResponse<ApiResponse<Recipe[]>> = await api.get('/v1/recipes');
        return response.data;
    }

    static async getRecipe(id: number): Promise<ApiResponse<RecipeDetail>> {
        const response: AxiosResponse<ApiResponse<RecipeDetail>> = await api.get(`/v1/recipes/${id}`);
        return response.data;
    }

    static async getFavoriteRecipes(): Promise<ApiResponse<Recipe[]>> {
        const response: AxiosResponse<ApiResponse<Recipe[]>> = await api.get('/v1/recipes/favorites');
        return response.data;
    }

    static async getRecentRecipes(count: number = 5): Promise<ApiResponse<Recipe[]>> {
        const response: AxiosResponse<ApiResponse<Recipe[]>> = await api.get(`/v1/recipes/recent?count=${count}`);
        return response.data;
    }

    static async createRecipe(recipe: RecipeCreateDto): Promise<ApiResponse<Recipe>> {
        const response: AxiosResponse<ApiResponse<Recipe>> = await api.post('/v1/recipes', recipe);
        return response.data;
    }

    static async updateRecipe(id: number, recipe: RecipeUpdateDto): Promise<ApiResponse<Recipe>> {
        const response: AxiosResponse<ApiResponse<Recipe>> = await api.put(`/v1/recipes/${id}`, recipe);
        return response.data;
    }

    static async deleteRecipe(id: number): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/recipes/${id}`);
        return response.data;
    }

    // Recipe Finder - FIXED: Use correct versioned endpoints
    static async findRecipes(request: FindRecipesRequest): Promise<ApiResponse<RecipeMatch[]>> {
        const response: AxiosResponse<ApiResponse<RecipeMatch[]>> = await api.post('/v1/recipefinder/find', request);
        return response.data;
    }

    static async getRecipeAvailability(id: number, request: AvailabilityRequest): Promise<ApiResponse<RecipeAvailabilityResponse>> {
        const response: AxiosResponse<ApiResponse<RecipeAvailabilityResponse>> = await api.post(`/v1/recipefinder/${id}/availability`, request);
        return response.data;
    }

    static async getPantryIngredients(): Promise<ApiResponse<Ingredient[]>> {
        const response: AxiosResponse<ApiResponse<Ingredient[]>> = await api.get('/v1/recipefinder/pantry-ingredients');
        return response.data;
    }

    // Ingredients - FIXED: Use correct versioned endpoints
    static async getIngredients(): Promise<ApiResponse<Ingredient[]>> {
        const response: AxiosResponse<ApiResponse<Ingredient[]>> = await api.get('/v1/ingredients');
        return response.data;
    }

    static async getIngredient(id: number): Promise<ApiResponse<IngredientDetail>> {
        const response: AxiosResponse<ApiResponse<IngredientDetail>> = await api.get(`/v1/ingredients/${id}`);
        return response.data;
    }

    static async searchIngredients(term: string): Promise<ApiResponse<Ingredient[]>> {
        const response: AxiosResponse<ApiResponse<Ingredient[]>> = await api.get(`/v1/ingredients/search?term=${encodeURIComponent(term)}`);
        return response.data;
    }

    static async getIngredientsByCategory(category: string): Promise<ApiResponse<Ingredient[]>> {
        const response: AxiosResponse<ApiResponse<Ingredient[]>> = await api.get(`/v1/ingredients/category/${encodeURIComponent(category)}`);
        return response.data;
    }

    static async createIngredient(ingredient: IngredientCreateDto): Promise<ApiResponse<Ingredient>> {
        const response: AxiosResponse<ApiResponse<Ingredient>> = await api.post('/v1/ingredients', ingredient);
        return response.data;
    }

    // Categories - FIXED: Use correct versioned endpoints
    static async getCategories(): Promise<ApiResponse<Category[]>> {
        const response: AxiosResponse<ApiResponse<Category[]>> = await api.get('/v1/categories');
        return response.data;
    }

    static async getCategoryNames(): Promise<ApiResponse<string[]>> {
        const response: AxiosResponse<ApiResponse<string[]>> = await api.get('/v1/categories/names');
        return response.data;
    }

    static async createCategory(category: CategoryCreateDto): Promise<ApiResponse<Category>> {
        const response: AxiosResponse<ApiResponse<Category>> = await api.post('/v1/categories', category);
        return response.data;
    }

    // Meal Plans - FIXED: Use correct versioned endpoints
    static async getMealPlans(): Promise<ApiResponse<MealPlan[]>> {
        const response: AxiosResponse<ApiResponse<MealPlan[]>> = await api.get('/v1/mealplans');
        return response.data;
    }

    static async getMealPlan(id: number): Promise<ApiResponse<MealPlanDetail>> {
        const response: AxiosResponse<ApiResponse<MealPlanDetail>> = await api.get(`/v1/mealplans/${id}`);
        return response.data;
    }

    static async getCurrentMealPlan(): Promise<ApiResponse<MealPlanDetail>> {
        const response: AxiosResponse<ApiResponse<MealPlanDetail>> = await api.get('/v1/mealplans/current');
        return response.data;
    }

    static async createMealPlan(mealPlan: MealPlanCreateDto): Promise<ApiResponse<MealPlan>> {
        const response: AxiosResponse<ApiResponse<MealPlan>> = await api.post('/v1/mealplans', mealPlan);
        return response.data;
    }

    static async addMealPlanItem(mealPlanId: number, item: MealPlanItemCreateDto): Promise<ApiResponse<MealPlanItem>> {
        const response: AxiosResponse<ApiResponse<MealPlanItem>> = await api.post(`/v1/mealplans/${mealPlanId}/items`, item);
        return response.data;
    }

    static async deleteMealPlanItem(mealPlanId: number, itemId: number): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/mealplans/${mealPlanId}/items/${itemId}`);
        return response.data;
    }

    // Shopping Lists - FIXED: Use correct versioned endpoints
    static async getShoppingLists(): Promise<ApiResponse<ShoppingList[]>> {
        const response: AxiosResponse<ApiResponse<ShoppingList[]>> = await api.get('/v1/shoppinglists');
        return response.data;
    }

    static async getShoppingList(id: number): Promise<ApiResponse<ShoppingListDetail>> {
        const response: AxiosResponse<ApiResponse<ShoppingListDetail>> = await api.get(`/v1/shoppinglists/${id}`);
        return response.data;
    }

    static async createShoppingList(shoppingList: ShoppingListCreateDto): Promise<ApiResponse<ShoppingList>> {
        const response: AxiosResponse<ApiResponse<ShoppingList>> = await api.post('/v1/shoppinglists', shoppingList);
        return response.data;
    }

    static async createShoppingListFromMealPlan(mealPlanId: number, name: string): Promise<ApiResponse<ShoppingListDetail>> {
        const response: AxiosResponse<ApiResponse<ShoppingListDetail>> = await api.post('/v1/shoppinglists/from-meal-plan', {
            mealPlanId,
            name
        });
        return response.data;
    }

    static async addShoppingListItem(listId: number, item: ShoppingListItemCreateDto): Promise<ApiResponse<ShoppingListItem>> {
        const response: AxiosResponse<ApiResponse<ShoppingListItem>> = await api.post(`/v1/shoppinglists/${listId}/items`, item);
        return response.data;
    }

    static async toggleShoppingListItem(listId: number, itemId: number, purchased: boolean): Promise<ApiResponse<ShoppingListItem>> {
        const response: AxiosResponse<ApiResponse<ShoppingListItem>> = await api.put(`/v1/shoppinglists/${listId}/items/${itemId}/toggle`, { purchased });
        return response.data;
    }

    static async removeCompletedItems(listId: number): Promise<ApiResponse<{ removedCount: number }>> {
        const response: AxiosResponse<ApiResponse<{ removedCount: number }>> = await api.delete(`/v1/shoppinglists/${listId}/completed-items`);
        return response.data;
    }

    // User Pantry - FIXED: Use correct versioned endpoints
    static async getUserPantryItems(): Promise<ApiResponse<UserPantryItem[]>> {
        const response: AxiosResponse<ApiResponse<UserPantryItem[]>> = await api.get('/v1/userpantry');
        return response.data;
    }

    static async getUserPantryItem(id: number): Promise<ApiResponse<UserPantryItem>> {
        const response: AxiosResponse<ApiResponse<UserPantryItem>> = await api.get(`/v1/userpantry/${id}`);
        return response.data;
    }

    static async getExpiringItems(daysThreshold: number = 7): Promise<ApiResponse<UserPantryItem[]>> {
        const response: AxiosResponse<ApiResponse<UserPantryItem[]>> = await api.get(`/v1/userpantry/expiring?daysThreshold=${daysThreshold}`);
        return response.data;
    }

    static async getLowStockItems(threshold: number = 5): Promise<ApiResponse<UserPantryItem[]>> {
        const response: AxiosResponse<ApiResponse<UserPantryItem[]>> = await api.get(`/v1/userpantry/low-stock?threshold=${threshold}`);
        return response.data;
    }

    static async addPantryItem(item: UserPantryItemCreateDto): Promise<ApiResponse<UserPantryItem>> {
        const response: AxiosResponse<ApiResponse<UserPantryItem>> = await api.post('/v1/userpantry', item);
        return response.data;
    }

    static async updatePantryItem(id: number, item: UserPantryItem): Promise<ApiResponse<UserPantryItem>> {
        const response: AxiosResponse<ApiResponse<UserPantryItem>> = await api.put(`/v1/userpantry/${id}`, item);
        return response.data;
    }

    static async deletePantryItem(id: number): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/userpantry/${id}`);
        return response.data;
    }

    static async updateShoppingList(id: number, data: { name: string }): Promise<ApiResponse<ShoppingList>> {
        const response: AxiosResponse<ApiResponse<ShoppingList>> = await api.put(`/v1/shoppinglists/${id}`, data);
        return response.data;
    }

    static async deleteShoppingList(id: number): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/shoppinglists/${id}`);
        return response.data;
    }

// Recipes - Additional methods for better error handling
    static async updateRecipeIngredient(recipeId: number, ingredientId: number, ingredient: RecipeIngredientUpdateDto): Promise<ApiResponse<RecipeIngredient>> {
        const response: AxiosResponse<ApiResponse<RecipeIngredient>> = await api.put(`/v1/recipes/${recipeId}/ingredients/${ingredientId}`, ingredient);
        return response.data;
    }

    static async deleteRecipeIngredient(recipeId: number, ingredientId: number): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/recipes/${recipeId}/ingredients/${ingredientId}`);
        return response.data;
    }

// Meal Plans - Additional methods
    static async updateMealPlanItem(mealPlanId: number, itemId: number, item: MealPlanItemUpdateDto): Promise<ApiResponse<MealPlanItem>> {
        const response: AxiosResponse<ApiResponse<MealPlanItem>> = await api.put(`/v1/mealplans/${mealPlanId}/items/${itemId}`, item);
        return response.data;
    }

// Categories - Additional methods
    static async updateCategory(id: number, category: CategoryUpdateDto): Promise<ApiResponse<Category>> {
        const response: AxiosResponse<ApiResponse<Category>> = await api.put(`/v1/categories/${id}`, category);
        return response.data;
    }

    static async deleteCategory(id: number): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/categories/${id}`);
        return response.data;
    }

// Ingredients - Additional methods
    static async updateIngredient(id: number, ingredient: IngredientUpdateDto): Promise<ApiResponse<Ingredient>> {
        const response: AxiosResponse<ApiResponse<Ingredient>> = await api.put(`/v1/ingredients/${id}`, ingredient);
        return response.data;
    }

    static async deleteIngredient(id: number): Promise<ApiResponse<object>> {
        const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/ingredients/${id}`);
        return response.data;
    }

    // Debug method to test API connectivity
    static async testConnection(): Promise<void> {
        try {
            console.log('Testing API connection...');
            console.log('Base URL:', import.meta.env.VITE_API_BASE_URL);

            // Test a simple endpoint
            const response = await api.get('/v1/categories');
            console.log('API Connection Test - Categories Response:', response.data);
        } catch (error) {
            console.error('API Connection Test Failed:', error);

            if (axios.isAxiosError(error)) {
                console.error('Status:', error.response?.status);
                console.error('Status Text:', error.response?.statusText);
                console.error('Response Data:', error.response?.data);
                console.error('Request URL:', error.config?.url);
                console.error('Full URL:', `${error.config?.baseURL}${error.config?.url}`);
            }
        }
    }
}

// Export token manager for use in auth store
export { TokenManager };

export default api;