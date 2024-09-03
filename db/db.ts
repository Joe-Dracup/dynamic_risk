import postgres from 'postgres'
import * as schema from './schema'
import { drizzle } from 'drizzle-orm/postgres-js'

export function getDB(connectionString: string) {
  console.log(connectionString)
  const queryClient = postgres(connectionString)

  return {
    db: drizzle(queryClient, { schema }),
    connection: queryClient,
  }
}

export type DynamicRiskDB = ReturnType<typeof getDB>['db']
