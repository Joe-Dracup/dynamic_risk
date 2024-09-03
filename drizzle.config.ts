import process from 'node:process'
import { defineConfig } from 'drizzle-kit'

const connectionString = process.env.NUXT_DATABASE_URL

if (!connectionString)
  throw new Error(
    'Database connection string not provided. Ensure that NUXT_DATABASE_URL is set in .env or as an environment variable.'
  )

export default defineConfig({
  schema: './db/schema.ts',
  out: './db/migrations',
  dialect: 'postgresql',
  dbCredentials: {
    url: connectionString,
  },
})
