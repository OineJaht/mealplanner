<!-- src/views/NotFound.vue -->
<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50">
    <div class="max-w-md w-full text-center">
      <div class="mb-8">
        <ExclamationTriangleIcon class="mx-auto h-24 w-24 text-gray-400" />
      </div>
      <h1 class="text-6xl font-bold text-gray-900 mb-4">404</h1>
      <h2 class="text-2xl font-semibold text-gray-700 mb-4">Page Not Found</h2>
      <p class="text-gray-500 mb-8">
        The page you're looking for doesn't exist or has been moved.
      </p>
      <div class="space-y-3">
        <router-link
            to="/"
            class="btn-primary w-full"
        >
          Go Home
        </router-link>
        <button
            @click="$router.go(-1)"
            class="btn-secondary w-full"
        >
          Go Back
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline';
</script>