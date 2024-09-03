import { getDB, type DynamicRiskDB } from '~/db/db'

let db: DynamicRiskDB

declare module 'h3' {
  interface H3EventContext {
    db: DynamicRiskDB
  }
}

export default eventHandler((event) => {
  if (!db) {
    const config = useRuntimeConfig(event)
    db = getDB(config.database.url).db
  }

  event.context.db = db
})