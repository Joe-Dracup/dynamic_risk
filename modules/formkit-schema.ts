import { addComponent, defineNuxtModule } from '@nuxt/kit'


export default defineNuxtModule({
  setup() {
    addComponent({
      name: 'FormKitSchema',
      export: 'FormKitSchema',
      filePath: '@formkit/vue',
    })
  },
})