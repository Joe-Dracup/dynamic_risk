import { risks } from '~/db/schema'

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  console.log(body)

  const insertedRisk = await event.context.db.insert(risks).values(body)

  setResponseStatus(event, 201, 'Quote Created')
  
  return body; 
})
