import { risks } from '~/db/schema'

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  console.log(body)

  const insertedRisk = (await event.context.db.insert(risks).values(body).returning())[0]

  setResponseStatus(event, 201, 'Quote Created')

  const ratingObject: RERequest = {
    ProductUniqueID: "ModernCar",
    LimitSchemesToSchemeReference: "",
    QuoteInParallel: false,
    EffectiveDate: new Date(),
    RiskReference: insertedRisk.id,
    RiskFields: flattenObject(insertedRisk.risk)
  }

  const cheapestPremium = getRate(ratingObject);

  return cheapestPremium; 
})


async function getRate(request: RERequest) {
  const post = await $fetch<any>(`http://localhost:5000/quote/getquote/new`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: request
  })

  return post.results[0].schemeQuoteResult.premium
}

const flattenObject = (obj: any): any => {
  let resultObj: any = {};

  for (const i in obj) {
      if (typeof obj[i] === 'object' &&
          !Array.isArray(obj[i])) {
          // Recursively invoking the funtion 
          // until the object gets flatten
          const tempObj = flattenObject(obj[i]);
          for (const j in tempObj) {
              resultObj[i + '.' + j] = tempObj[j];
          }
      } else {
          resultObj[i] = obj[i];
      }
  }
  return resultObj;
};


type RERequest = {
  ProductUniqueID: string,
  LimitSchemesToSchemeReference: string,
  QuoteInParallel: boolean,
  EffectiveDate: Date
  RiskReference: string
  RiskFields: any
}