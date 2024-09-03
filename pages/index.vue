<script setup lang="ts">
import type { FormKitSchemaNode } from "@formkit/core";

const risk = ref<any>({ title: "test" });
const cheapestPremium = ref<string>();

const schema: FormKitSchemaNode[] = [
  {
    $el: "h1",
    children: "Here is a question set!",
  },
  {
    $formkit: "text",
    name: "title",
    label: "Title",
    help: "what is your title",
    validation: "required",
  },
  {
    $formkit: "text",
    name: "firstname",
    label: "First Name",
    help: "what is your firstname",
    validation: "required",
  },
  {
    $formkit: "text",
    name: "lastname",
    label: "Last Name",
    help: "what is your lastname",
    validation: "required",
  },
  {
    $formkit: "text",
    name: "colour",
    label: "Fave Colour",
    help: "what is your colour",
    validation: "required",
  },
  {
    $formkit: "text",
    name: "dansq",
    label: "Dans Question",
    help: "what is your dan",
    validation: "required",
  },
];

async function save(savedRisk: any) {
  const post = await useFetch(`/api/quote/create`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: {
      risk: savedRisk
    }
  })

  risk.value = savedRisk
  cheapestPremium.value = post.data.value
}

</script>

<template>
  <div class="m-8">
    <FormKit
      type="form"
      submit-label="Save"
      :submit-attrs="{
        id: 'submit',
      }"
      @submit="save"
      :value="risk"
    >
      <FormKitSchema :schema="schema" />
    </FormKit>
    <p>{{ cheapestPremium }}</p>
    <pre wrap>{{ risk }}</pre>
  </div>
</template>
