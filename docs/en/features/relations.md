---
title: Relations
parent: Features
nav_order: 9
---

### Relations
Relationships - are a way of linking some entities to other entities, relationships allow you to add hierarchies between entities
- links can be of several types and can be controlled automatically
- links are based on components or multi-components, and use a global entity identifier

### Types of links:

#### Unidirectional link to one

#### Example:
```csharp
//  A Passenger -> B
    
public struct Passenger : IEntityLinkComponent<Passenger> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Passenger, EntityGID>.RefValue(ref Passenger component) => ref component.Link;
}

W.RegisterToOneRelationType<Passenger>();
```

___


#### Unidirectional link to many

#### Example:
```csharp
//      Passenger -> B
//   /
//  A-- Passenger -> C
//   \
//      Passenger -> D
        
public struct Passengers : IEntityLinksComponent<Passengers> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Passengers, ROMulti<EntityGID>>.RefValue(ref Passengers component) => ref component.Links;
}
                       
W.RegisterToManyRelationType<Passengers>();
```

___


#### Bidirectional one-to-one link

#### Example:
```csharp
//  A <- Parent Child -> B <- Parent Child -> C
    
public struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;
}

public struct Child : IEntityLinkComponent<Child> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Child, EntityGID>.RefValue(ref Child component) => ref component.Link;
}

W.RegisterOneToOneRelationType<Parent, Child>();
```

___


#### Bidirectional one-to-one link (closed pair)

#### Example:
```csharp
//    Married   
//  A -------> B
//    Married   
//  A <------- B
    
public struct MarriedTo : IEntityLinkComponent<MarriedTo> {
    public EntityGID Link;

    ref EntityGID IRefProvider<MarriedTo, EntityGID>.RefValue(ref MarriedTo component) => ref component.Link;
}

W.RegisterOneToOneRelationType<MarriedTo, MarriedTo>()
```

___


#### Bidirectional one-to-many link

#### Example:
```csharp
                       
//     <- Parent Child -> B 
//   /                     
//  A- <- Parent Child -> С 
//   \
//     <- Parent Child -> D
       
public struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;
}

public struct Childs: IEntityLinksComponent<Childs> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Childs, ROMulti<EntityGID>>.RefValue(ref Childs component) => ref component.Links;
}
                       
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4);
```

___


#### Bidirectional many-to-many link

#### Example:
```csharp
                       
//     <- Owners Ownerships -> B 
//   /            
//  A- <- Owners Ownerships -> С 
//   /            
//  D- <- Owners Ownerships -> E 

public struct Ownerships : IEntityLinksComponent<Ownerships> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Ownerships, ROMulti<EntityGID>>.RefValue(ref Ownerships component) => ref component.Links;
}

public struct Owners : IEntityLinksComponent<Owners> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Owners, ROMulti<EntityGID>>.RefValue(ref Owners component) => ref component.Links;
}
                       
W.RegisterManyToManyRelationType<Ownerships, Owners>(16)
```

___


### Let's look at the configuration and example step by step using the example of `One to many` link
- Component definition
> Components can be of two types:  
> `IEntityLinkComponent` - component interface for storing a reference to a single entity  
> `IEntityLinksComponent` - component interface for storing references to several entities

```csharp
// Let's define the Parent - `One` link component of type IEntityLinkComponent
public struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;

    // We implement a technical method to access the value of the relation
    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;

    public override string ToString() => Link.ToString();
}

// Let's define a Child links component - `Many` of type IEntityLinksComponent
public struct Childs: IEntityLinksComponent<Childs> {
    public ROMulti<EntityGID> Links;

    // We implement a technical method to access the value of the relation
    ref ROMulti<EntityGID> IRefProvider<Childs, ROMulti<EntityGID>>.RefValue(ref Childs component) => ref component.Links;

    public override string ToString() => Links.ToString();
}
```

___


- Creation of the world and entities

```csharp

W.Create(WorldConfig.Default());
// ...
// Register component types defaultComponentCapacity sets the minimum size of Childs in a multicomponent
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4);
// ...
W.Initialize();

var father = W.Entity.New(new Name("Father"));
var sonAlex = W.Entity.New(new Name("Son Alex"));
var sonJack = W.Entity.New(new Name("Son Jack"));
var sonKevin = W.Entity.New(new Name("Son Kevin"));
```

___


- Link Registration (Option 1 on the parent's side)
> Set up a link where father refers to children {`sonAlex`, `sonJack`, `sonKevin` }  
> When the component is installed, children will automatically receive a reverse `Parent` component with a reference to father  
> The `SetLinks` method creates or uses an existing component (used for `IEntityLinksComponent` type)  
> takes from 1-5 `EntityGID` values and returns a reference to the component, in case the value is already set to `DEBUG` there will be an error

```csharp
ref Childs childs = ref father.SetLinks<Childs>(sonAlex, sonJack, sonKevin);
```

___

- Link Registration (Option 2 on the children's side)
> We could make a link from the children's side  
> When the parent installs the component, the parent will automatically receive a Childs reverse component with a link to the child  
> The `SetLink` method adds a component and sets the value (used for `IEntityLinkComponent` type)   
> if the component is already present, the component is deleted and a new one is added.  
> it's necessary for automatic link management

```csharp
 ref Parent sonAlexParent = ref sonAlex.SetLink<Parent>(father);
 ref Parent sonJackParent = ref sonJack.SetLink<Parent>(father);
 ref Parent sonKevinParent = ref sonKevin.SetLink<Parent>(father);
```

So it does not matter on which side the link is established, the backlink will be established in any case  
By looking at all the entities we can make sure of this  

```csharp
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
//  Entity ID: 3                   
//  Components:                    
//   - [0] Name ( Son Kevin )      
//   - [1] Parent ( Father )       
//  
//  Entity ID: 2                   
//  Components:                    
//   - [0] Name ( Son Jack )       
//   - [1] Parent ( Father )       
//                                 
//  Entity ID: 1                   
//  Components:                    
//   - [0] Name ( Son Alex )       
//   - [1] Parent ( Father )       
//                                 
//  Entity ID: 0                   
//  Components:
//   - [0] Name ( Father )
//   - [2] Childs ( Son Alex, Son Jack, Son Kevin )
```

___


- Deletion of the link (Option 1 on the children's side )
> The `TryDeleteLink` method deletes the link (and component) if it exists (used for `IEntityLinkComponent` type)

```csharp
sonAlex.TryDeleteLink<Parent>();
sonJack.TryDeleteLink<Parent>();
sonKevin.TryDeleteLink<Parent>();
```

> By default, deleting a link will remove the backlink when the link is deleted   
> This means that `father' will have the reference to all children removed.   
> To override this behavior you need to specify a deletion strategy when registering components   
> `leftDeleteStrategy` - deletion strategy for `Parent` component  

```csharp
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4, leftDeleteStrategy: Default);
```

The following types of strategies are available:  
- `Default`               : Doesn't do anything when deleting  
- `DestroyLinkedEntity`   : Destroys the attached entity  
- `DeleteAnotherLink`     : Removes the link from the attached entity (default behavior)


- Deletion of the link (Option 2 on the parent side )
> The `TryDeleteLinks` method deletes a link if it exists (used for `IEntityLinksComponent` type)  
> accepts from 0-5 `EntityGID` value, if no value is passed deletes all links   
> If there are no links left, the component will also be deleted.  

```csharp
father.TryDeleteLinks<Childs>();
```

> By default, deleting a link will remove the backlink when the link is deleted  
> That means all the children will have their parental reference removed.  
> To override this behavior you need to specify a deletion strategy when registering components  
> `rightDeleteStrategy` - removal strategy for a component `Childs`
```csharp
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4, rightDeleteStrategy: Default);
```

The following types of strategies are available:
- `Default`               : Doesn't do anything when deleting
- `DestroyLinkedEntity`   : Destroys the attached entity
- `DeleteAnotherLink`     : Removes the link from the attached entity (default behavior)

___


- Additionally
> DEBUG validation of cyclic links can be overridden during registration, it is enabled by default  
> To do this, specify `disableRelationsCheckDebug` = `true` in the component registration method
>
> When registering, you can override the copy behavior by specifying CopyStrategy `FFS.Libraries.StaticEcs.CopyStrategy`

> Relationship components are normal components and all standard methods of operation are available with some special features

```csharp
entity.Ref<Parent>();
entity.HasAllOf<Parent>();
//..
```

{: .important }
It is worth cautioning against changing the values of links manually (not through special methods such as `SetLink`, `SetLinks`, `TryDeleteLink`, `TryDeleteLinks`)  
for example, you don't want to do things like this: `entity.Ref<Parent>().Link = someGid;`  
Because it doesn't automatically manage backlinks and other actions and can lead to broken game logic  
at the same time nothing prevents from storing additional data in components besides the connection itself

___


- Filtering methods
> Relationship components can be used in queries like any other components

```csharp
W.QueryEntities.For<All<Parent, Childs>>()
// ..
```
