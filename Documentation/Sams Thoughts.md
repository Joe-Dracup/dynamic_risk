# Starting at the Data

- My first consideration would be that in order to store a properly dynamic risk; the database technology we use should be able to reflect this.
- I'd recommend Mongo, or Cosmos, or DynamoDB for this purpose - so that it doesn't matter what the Risk document looks like; there's no schema violations or columns to manage.
- I'd start with Cosmos, since it's the one I've had the most experience with.
- First problem: Data Types - Document DBs only have a limited number of types. CosmosDB types listed below:

| **Data Type**       | **Description**                                                                                                |
| ------------------- | -------------------------------------------------------------------------------------------------------------- |
| **String**          | Text data enclosed in quotes (e.g., `"Hello World"`).                                                          |
| **Number**          | Numerical values, including integers and floating-point numbers (e.g., `42`).                                  |
| **Boolean**         | Represents `true` or `false`.                                                                                  |
| **Null**            | Represents an absence of a value.                                                                              |
| **Array**           | A collection of values, which can be of any data type (e.g., `[1, "text", true]`).                             |
| **Object**          | A collection of key-value pairs, where keys are strings and values can be any type (e.g., `{"key": "value"}`). |
| **Undefined**       | Represents an undefined value (rarely used directly in data models).                                           |
| **Binary (Base64)** | Binary data encoded as a Base64 string.                                                                        |
| **Geospatial Data** | Represents geographic objects like `Point`, `Polygon`, etc., for spatial queries.                              |

- In order to reduce maintenance cost in dealing with parsing / comparing / using these values; we'd probably want to create an object that contained metadata in order to parse values back to a strongly-typed variable - my suggestion is:

```
# RiskEntry object (Represents a single Risk item)
{
  UniqueName: "CoverType",
  ParseType: "DefinedListDetail",
  Value: 1928,
  IsCore: false
}
```

- Side Note: above "DefinedListDetail" is a non-primitive type; since we quite frequently store DLD values against risk answers. However, with a successive DB call, this can be used to parse an object called `CoverType` that is strongly typed as a `DefinedListDetail`, into our `Risk` objects.

- How I imagine this working:
  - `Risk` is no longer a strongly-typed object in memory; but a collection of `RiskEntry` items. Each of which is capable of self-identifying and parsing at scope-edge for later, easily managed manipulation or reference. `Risk === RiskEntry[]`
  - This way `CarRisk == RiskEntry[16]` or `PetRisk == RiskEntry[50]` etc, etc.
  - `IsCore` can be used to mark individual `RiskEntry` variables as part of a core `IRisk` interface - so we can easily mark ones that _MUST_ be on every type of Risk. If we wanted to, when parsing, we could have computed properties e.g. `Risk.Title => this.RiskEntries.First(x => x.UniqueName == "Title")` to give us compile-time ease of use, but the `IsCore = false` values would be retrieved with a generic resolver `Risk.Get<T>("UniqueName")`

### Summary

- This is kind of akin to reflection; except instead of storing _oodles_ of metadata for each property, the property's representation on the database can be used to tell you everything you'll need to know about it - you design the code that consumes these objects as schema-agnostic (other than the `IsCore` which can be strong-typed for ease), and at that point, we should be able to do everything the existing `Risk`s do.
