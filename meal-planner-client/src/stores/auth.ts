import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { ApiService, TokenManager } from '@/services/api';
import type { LoginDto, RegisterDto, User, TokenDto } from '@/types';

export const useAuthStore = defineStore('auth', () => {
    const user = ref<User | null>(null);
    const isLoading = ref(false);
    const error = ref<string>('');

    const isAuthenticated = computed(() => !!user.value && !!TokenManager.getAccessToken());

    async function login(credentials: LoginDto): Promise<boolean> {
        try {
            isLoading.value = true;
            error.value = '';

            const response = await ApiService.login(credentials);

            if (response.success && response.data) {
                TokenManager.setTokens(response.data);
                // You might want to fetch user profile here
                return true;
            } else {
                error.value = response.message || 'Login failed';
                return false;
            }
        } catch (err: any) {
            error.value = err.response?.data?.message || 'Login failed';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    async function register(userData: RegisterDto): Promise<boolean> {
        try {
            isLoading.value = true;
            error.value = '';

            const response = await ApiService.register(userData);

            if (response.success && response.data) {
                TokenManager.setTokens(response.data);
                return true;
            } else {
                error.value = response.message || 'Registration failed';
                return false;
            }
        } catch (err: any) {
            error.value = err.response?.data?.message || 'Registration failed';
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    async function logout(): Promise<void> {
        try {
            await ApiService.logout();
        } catch (err) {
            console.error('Logout error:', err);
        } finally {
            TokenManager.clearTokens();
            user.value = null;
        }
    }

    function checkAuth(): boolean {
        const token = TokenManager.getAccessToken();
        if (token && !TokenManager.isTokenExpired()) {
            return true;
        } else {
            TokenManager.clearTokens();
            user.value = null;
            return false;
        }
    }

    return {
        user,
        isLoading,
        error,
        isAuthenticated,
        login,
        register,
        logout,
        checkAuth
    };
});