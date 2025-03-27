---
title: Tag
parent: Main types
nav_order: 6
---

## Tag
Tag - similar to a component, but does not contain any data, serves to label an entity
- Optimized storage, doesn't store massive amounts of data, doesn't slow down component searches, allows you to create multiple tags
- Gives the option to build search queries based on tags only
- Represented as a user structure without data with a marker interface `ITag`

#### Example:
```c#
public struct Unit : ITag { }
```

___

{: .important }
Requires registration in the world between creation and initialization

```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterTagType<Unit>();
//...
MyEcs.Initialize();
```

___

#### Creation:
```c#
// Adding a tag to an entity (in DEBUG mode there will be an error if it already exists on the entity) (overload methods from 1-5 tags)
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

// Remove tag from entity (in DEBUG mode there will be an error if tag is not present, 
// in release cannot be used if there is no guarantee that tag is present) (overload methods from 1-5 tags)
entity.DeleteTag<Unit>();
entity.DeleteTag<Unit, Player>();

// Remove a tag from an entity if it has one (overload methods from 1-5 tags)
bool deleted = entity.TryDeleteTag<Unit>();  // deleted = true if the tag has been deleted, false if the tag was not there originally
bool deleted = entity.TryDeleteTag<Unit, Player>();  // deleted = true if ALL tags have been deleted, false if at least 1 tag was not originally there.
```