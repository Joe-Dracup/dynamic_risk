import { migrate } from 'drizzle-orm/node-postgres/migrator'
import { getDB } from '../../db/db'

export default defineNitroPlugin(async () => {
  const config = useRuntimeConfig()
  const connectionString = config.database.url

  if (!connectionString)
    throw new Error(
      'Database connection string not provided. Ensure that NUXT_DATABASE_URL is set in .env or as an environment variable.'
    )

  const { db, connection } = getDB(connectionString)
  await migrate(db, { migrationsFolder: config.database.migrationsFolder })

  await connection.end()
})
