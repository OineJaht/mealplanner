import axios, { AxiosResponse, AxiosError, AxiosRequestConfig } from 'axios';
import type {
  ApiResponse,
  LoginDto,
  RegisterDto,
  TokenDto,
  Ingredient,
  IngredientDetail,
  IngredientCreateDto,
  IngredientUpdateDto
} from '@/types';

// Create axios instance - matching Vue's configuration exactly
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Token management
export class TokenManager {
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

// Response interceptor for token refresh - matching Vue's approach
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

  // Ingredients
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

  static async createIngredient(ingredient: IngredientCreateDto): Promise<ApiResponse<Ingredient>> {
    const response: AxiosResponse<ApiResponse<Ingredient>> = await api.post('/v1/ingredients', ingredient);
    return response.data;
  }

  static async updateIngredient(id: number, ingredient: IngredientUpdateDto): Promise<ApiResponse<Ingredient>> {
    const response: AxiosResponse<ApiResponse<Ingredient>> = await api.put(`/v1/ingredients/${id}`, ingredient);
    return response.data;
  }

  static async deleteIngredient(id: number): Promise<ApiResponse<object>> {
    const response: AxiosResponse<ApiResponse<object>> = await api.delete(`/v1/ingredients/${id}`);
    return response.data;
  }
}

export default api;