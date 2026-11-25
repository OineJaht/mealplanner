// src/types/env.d.ts
/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_API_BASE_URL: string
    // Add more env variables here if needed
    // readonly VITE_SOME_KEY: string
}
interface ImportMeta {
    readonly env: ImportMetaEnv
}