<script setup lang="ts">
import type { FormKitSchemaNode } from "@formkit/core";
import { ref } from "vue";

const risk = ref<any>();
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
    attrs: {
      parseType: "definedListDetail",
    },
  },
  {
    $formkit: "text",
    name: "firstname",
    label: "First Name",
    help: "what is your firstname",
    validation: "required",
    attrs: {
      parseType: "string",
    },
  },
  {
    $formkit: "text",
    name: "lastname",
    label: "Last Name",
    help: "what is your lastname",
    validation: "required",
    attrs: {
      parseType: "string",
    },
  },
  {
    $formkit: "text",
    name: "colour",
    label: "Fave Colour",
    help: "what is your colour",
    validation: "required",
    attrs: {
      parseType: "string",
    },
  },
  {
    $formkit: "text",
    name: "dansq",
    label: "Dans Question",
    help: "what is your dan",
    validation: "required",
    attrs: {
      parseType: "string",
    },
  },
];

async function save(savedRisk: any) {
  // Transform savedRisk to the desired structure
  const transformedRisk = Object.keys(savedRisk).reduce((acc, key) => {
    // Find the schema node that matches the key
    const schemaNode = schema.find(
      (node) => node.$formkit === "text" && node.name === key
    );

    // If the schema node is found, use its parseType
    if (schemaNode && schemaNode.attrs?.parseType) {
      acc[key] = {
        Value: savedRisk[key],
        parseType: schemaNode.attrs.parseType,
      };
    } else {
      // Default case if parseType is not found
      acc[key] = {
        Value: savedRisk[key],
        parseType: "string",
      };
    }
    return acc;
  }, {} as any);

  const post = await useFetch(`/api/quote/create`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: {
      risk: transformedRisk,
    },
  });

  risk.value = savedRisk;
  cheapestPremium.value = post.data.value;
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
