---
title: Tag
parent: Features
nav_order: 6
---

## Tag
Tag - similar to a component, but does not contain any data, serves to label an entity
- Optimized storage, doesn't store massive amounts of data, doesn't slow down component searches, allows you to create multiple tags
- Represented as a user structure without data with a marker interface `ITag`

#### Example:
```c#
public struct Unit : ITag { }
```

___

{: .important }
Requires registration in the world between creation and initialization

```c#
W.Create(WorldConfig.Default());
//...
W.RegisterTagType<Unit>();
//...
W.Initialize();
```

___

#### Creation:
```c#
// Adding a tag to an entity (overload methods from 1-5 tags) (Returns true if a tag was missing and was added)
bool added = entity.SetTag<Unit>();
entity.SetTag<Unit, Player>();
```

___

#### Basic operations:
```c#
// Get the count of tags on an entity
int tagsCount = entity.TagsCount();

// Check for the presence of ALL specified tags (overload methods from 1-3 tags)
entity.HasAllOfTags<Unit>();
entity.HasAllOfTags<Unit, Player>();

// Check for the presence of at least one specified tag (overload methods from 2-3 tags)
entity.HasAnyOfTags<Unit, Player>();

// Remove tag from entity (Returns true if tag was present and was removed)
// Can be safely used even if the tag did not exist
bool deleted = entity.DeleteTag<Unit>();
entity.DeleteTag<Unit, Player>();

// If a tag is not present on an entity, it is added, if it is present, it is removed (overload methods from 1-3 tags)
// (Returns the current state, true if the tag was added, false if the tag was removed)
bool state = entity.ToggleTag<Unit>();
entity.ToggleTag<Unit, Player>();

// Depending on the passed value, either the tag is set (true) or removed (false) (overload methods from 1-3 tags)
entity.ApplyTag<Unit>(true);
entity.ApplyTag<Unit, Player>(false, true);
```