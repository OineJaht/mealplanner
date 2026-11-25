// src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'dashboard',
            component: () => import('@/views/Dashboard.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/login',
            name: 'login',
            component: () => import('@/views/Login.vue'),
            meta: { requiresGuest: true }
        },
        {
            path: '/register',
            name: 'register',
            component: () => import('@/views/Register.vue'),
            meta: { requiresGuest: true }
        },
        {
            path: '/recipes',
            name: 'recipes',
            component: () => import('@/views/Recipes.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/recipes/create',
            name: 'recipe-create',
            component: () => import('@/views/RecipeForm.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/recipes/:id',
            name: 'recipe-detail',
            component: () => import('@/views/RecipeDetail.vue'),
            meta: { requiresAuth: true },
            props: true
        },
        {
            path: '/recipes/:id/edit',
            name: 'recipe-edit',
            component: () => import('@/views/RecipeForm.vue'),
            meta: { requiresAuth: true },
            props: true
        },
        {
            path: '/meal-plans',
            name: 'meal-plans',
            component: () => import('@/views/MealPlans.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/meal-plans/create',
            name: 'meal-plan-create',
            component: () => import('@/views/MealPlanForm.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/meal-plans/:id',
            name: 'meal-plan-detail',
            component: () => import('@/views/MealPlanDetail.vue'),
            meta: { requiresAuth: true },
            props: true
        },
        {
            path: '/shopping-lists',
            name: 'shopping-lists',
            component: () => import('@/views/ShoppingLists.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/shopping-lists/:id',
            name: 'shopping-list-detail',
            component: () => import('@/views/ShoppingListDetail.vue'),
            meta: { requiresAuth: true },
            props: true
        },
        {
            path: '/pantry',
            name: 'pantry',
            component: () => import('@/views/Pantry.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/recipe-finder',
            name: 'recipe-finder',
            component: () => import('@/views/RecipeFinder.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/ingredients',
            name: 'ingredients',
            component: () => import('@/views/Ingredients.vue'),
            meta: { requiresAuth: true }
        },
        {
            path: '/:pathMatch(.*)*',
            name: 'not-found',
            component: () => import('@/views/NotFound.vue')
        }
    ]
});

// Navigation guards
router.beforeEach((to, from, next) => {
    const authStore = useAuthStore();

    if (to.meta.requiresAuth && !authStore.checkAuth()) {
        next('/login');
    } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
        next('/');
    } else {
        next();
    }
});

export default router;

