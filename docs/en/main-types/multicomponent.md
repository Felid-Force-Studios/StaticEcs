---
title: Multi component
parent: Main types
nav_order: 5
---

## MultiComponent
Multi-components - allow to give an entity many identical properties (components)
- Represent optimized component-lists containing N values
- All elements, of all multicomponents with the same type for all entities in the world are stored in a single repository, this provides:
    - optimized memory usage
    - allows storing up to 32768 values in one component
    - quick and easy access to data
    - quick creation, addition and extension
    - no need to create reference types of arrays or lists inside the component and keep track of them being cleared or returned to the pool, etc.
- Is the implementation [Component](component.md), all the basic rules and methods of work are similar
- Presented as:
    - standard structure `Multi<T>`, where T is the element type
    - or as a custom structure with the `IMultiComponent<T>` interface
- Based on multicomponents, [Relations](../additional-features/relations.md) of entities are implemented, for example, the `Children` component is a multicomponent

___

{: .important }
Requires registration in the world between creation and initialization


#### Example:  
There are two ways to define a multicomponent:  
First - Use the default `Multi<T>`

```c#
// Define the type of the multicomponent value
public struct Item {
    public string Value;
}

MyEcs.Create(EcsConfig.Default());
//...
// where defaultComponentCapacity is the minimum capacity of the multicomponent, with a power of two, in the range of 4 to 32768 (4, 8, 16, 32 ...)
MyEcs.World.RegisterMultiComponentType<Multi<Item>, Item>(defaultComponentCapacity: 4); 
//...
MyEcs.Initialize();
```

Second - Use a custom implementation `IMultiComponent<T>`

```c#
// Define the type of the multicomponent value 
public struct Item {
    public string Value;
}

// Define the type of component
public struct Inventory : IMultiComponent<Item> {
    // Define the values of the multicomponent
    public Multi<Item> Items;
    
    // Add any custom data if needed, just like in a simple component
    public int SomeUserData;
    
    // Implement the IMultiComponent<Item> interface method, for access and automatic value management
    // it is necessary to specify access to values via access.For(ref Values)
    public void Access<A>(A access) where A : struct, AccessMulti<Item> => access.For(ref Items);
}

MyEcs.Create(EcsConfig.Default());
//...
// instead of Multi<Item> specify custom Inventory component
// where defaultComponentCapacity is the minimum capacity of the multicomponent, with a power of two, in the range of 4 to 32768 (4, 8, 16, 32 ...)
MyEcs.World.RegisterMultiComponentType<Inventory, Item>(defaultComponentCapacity: 4); 
//...
MyEcs.Initialize();
```
___

#### Basic operations:
```c#
MyEcs.World.RegisterMultiComponentType<Multi<Item>, Item>(defaultComponentCapacity: 4); 

// When adding a multi-component, the defaultComponentCapacity will be defaultComponentCapacity, as elements are added, it will expand as the elements are added 
// Adding a multicomponent like a simple component
// in case of default implementaion
ref var items = ref entity.Add<Multi<Item>>();
// n case of custom implementaion
ref var inventory = ref entity.Add<Inventory>();
ref var items = ref inventory.Items;

// All other methods for working with components are available
entity.TryAdd<Multi<Item>>();
entity.HasAllOf<Multi<Item>>();
// ...

// When deleting a multicomponent - the list of items will be automatically cleared
entity.Delete<Multi<Item>>();
entity.TryDelete<Multi<Item>>();

// When copying and moving a multicomponent - all elements will be automatically copied
entity.CopyComponentsTo<Multi<Item>>(entity2);
entity.Clone();

entity.MoveComponentsTo<Multi<Item>>(entity2);
entity.MoveTo(entity2);

// A multicomponent behaves like a dynamic array (list)
// The available operations are as follows:
// Info:
ushort capacity = items.Capacity;                                      // Current capacity
ushort count = items.Count;                                            // Number of items
bool empty = items.IsEmpty();                                          // True if there are no elements
bool notEmpty = items.IsNotEmpty();                                    // True if there are elements
bool full = items.IsFull();                                            // True if the current capacity is full

// Access:
ref Item element = ref items[1];                                       // Indexer
ref Item first = ref items.First();                                    // Link to the first element
ref Item last = ref items.Last();                                      // Link to the last element
foreach (ref var item in items) {                                      // Foreach
    //..
}

for (ushort i = 0; i < items.Count; i++) {                             // For loop
    ref var item = ref items[i];
}

// Addition and extension:
items.Add(new Item());                                                 // Add element
items.Add(new Item("a"), new Item("b"), new Item("c"), new Item("d")); // Add elements (1 - 4)
items.Add(new[] { new Item("f"), new Item("g") });                     // Add elements from an array
items.Add(new[] { new Item("f"), new Item("g") }, 1, 1);               // Add elements from an array with the start and number specified
items.Add(ref entity2.RefMut<Multi<Item>>());                          // Add elements from another component
items.Add(ref entity2.RefMut<Multi<Item>>(), 1, 1);                    // Add elements from another component specifying start and quantity
items.InsertAt(idx: 1, new Item("e"));                                 // Insert an element in the specified index, the other elements will be shifted
items.EnsureSize(10);                                                  // Ensure capacity for N more elements if required
items.Resize(16);                                                      // Extend capacity to N if required

// Removal and cleaning
items.DeleteFirst();                                                   // Delete the first element and shift the subsequent elements (if element order is important)
items.DeleteFirstSwap();                                               // Delete the first element and replace with the last element (if element order is NOT important) (Faster)
items.DeleteLast();                                                    // Delete the last element and reset to default
items.DeleteLastFast();                                                // Delete the last element and WITHOUT resetting the value to default (if resetting the value is NOT important) (Faster)
items.DeleteAt(idx: 1);                                                // Delete element by index and shift subsequent elements (if element order is important) (Faster)
items.DeleteAtSwap(idx: 1);                                            // Delete element by index and replace with the last element (if element order is NOT important) (Faster)
items.Clear();                                                         // Clear items and reset to default
items.ResetCount();                                                    // Reset quantity without clearing

// Search
short idx = items.IndexOf(new Item("a"));                              // Get item index or -1
bool contains = items.Contains(new Item("a"));                         // Check if an element exists with default IEqualityComparer
bool contains = items.Contains(new Item("a"), comparer);               // Check for an element with a custom IEqualityComparer

// Extra
var array = new Item[items.Capacity];                                  
items.CopyTo(array);                                                   // Copy elements to the specified array
items.Sort();                                                          // Sort elements with default Comparer
items.Sort(comparer);                                                  // Sort elements with passed Comparer
```