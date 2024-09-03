// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  modules: ['@formkit/nuxt'],
  formkit: {
    autoImport: true,
    configFile: './formkit.config.ts'
  },
  postcss: {
    plugins: {
      tailwindcss: {},
      autoprefixer: {},
    },
  },
  css: ['./assets/css/main.css'],
  runtimeConfig: {
    database: {
      migrationsFolder: './db/migrations',
      url: '',
    },
  },
})
