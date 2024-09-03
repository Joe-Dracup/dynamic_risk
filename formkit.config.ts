import { defineFormKitConfig } from '@formkit/vue'
import { rootClasses } from './formkit.theme.ts'

export default defineFormKitConfig(() => {
  return {
    config: {
      rootClasses,
    },
  }
})
