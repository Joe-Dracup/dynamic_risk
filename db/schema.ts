import { createId } from '@paralleldrive/cuid2'
import { json, pgTable, varchar } from 'drizzle-orm/pg-core'

export const risks = pgTable('risk', {
  id: varchar('id', { length: 128 })
    .$defaultFn(() => createId())
    .primaryKey(),
  risk: json('risk'),
})
