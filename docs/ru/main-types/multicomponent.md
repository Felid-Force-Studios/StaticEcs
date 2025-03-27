---
title: Мульти-компонент
parent: Основные типы
nav_order: 5
---

## MultiComponent
Мультикомпоненты - позволяют наделять сущность множеством одинаковых свойств (компонентов)
- Представляют собой оптимизированные компоненты-списки, содержащие N значений
- Все элементы, всех мультикомпонентов с одним типом для всех сущностей в мире хранятся в едином хранилище, это обеспечивает:
  - оптимизированное потребление памяти
  - позволяет хранить до 32768 значений в одном компоненте
  - быстрый и удобный доступ к данным
  - быстрое создание, добавление и расширение
  - не требуется создавать ссылочные типы массивов или списков внутри компонента и отслеживать их очистку или возврат в пул и тд.
- Является реализацией [Component](#component), все базовые правила и способы работы аналогичны
- Представлен в виде:
  - стандартной структуры `Multi<T>`, где T - тип элементов
  - или в виде пользовательской структуры с интерфейсом `IMultiComponent<T>`
- На базе мультикомпонентов реализованы [Отношения](#отношения) сущностей, например компонент `Children` является мультикомпонентом

___

{: .importantru }
Требуется регистрация в мире между созданием и инициализацией


#### Пример:  
Есть два способа опеределить мультикомпонент: 

Первый - Использовать дефолтный `Multi<T>`

```c#
// Определим тип значения мультикомпонента 
public struct Item {
    public string Value;
}

MyEcs.Create(EcsConfig.Default());
//...
// где defaultComponentCapacity - минимальная вместительность мультикомпонента, со степенью двойки, в диапазоне от 4 до 32768  (4, 8, 16, 32 ...)
MyEcs.World.RegisterMultiComponentType<Multi<Item>, Item>(defaultComponentCapacity: 4); 
//...
MyEcs.Initialize();
```

Второй - Использовать кастомную реализация `IMultiComponent<T>`

```c#
// Определим тип значения мультикомпонента 
public struct Item {
    public string Value;
}

// Определим тип компонента 
public struct Inventory : IMultiComponent<Item> {
    // Определим значения мультикомпонента
    public Multi<Item> Items;
    
    // Добавляем любые пользовательские данные если нужно, как и в обычный компонент
    public int SomeUserData;
    
    // Реализуем метод интерфейса IMultiComponent<Item>, для доступа и автоматического менеджемента значений
    // необходимо указать доступ к значениям через access.For(ref Values)
    public void Access<A>(A access) where A : struct, AccessMulti<Item> => access.For(ref Items);
}

MyEcs.Create(EcsConfig.Default());
//...
// вместо Multi<Item> указываем кастомный компонент Inventory
// где defaultComponentCapacity - минимальная вместительность мультикомпонента, со степенью двойки, в диапазоне от 4 до 32768  (4, 8, 16, 32 ...)
MyEcs.World.RegisterMultiComponentType<Inventory, Item>(defaultComponentCapacity: 4); 
//...
MyEcs.Initialize();
```
___

#### Основные операции:
```c#
MyEcs.World.RegisterMultiComponentType<Multi<Item>, Item>(defaultComponentCapacity: 4); 

// При добавлениие мультикомпонента по умолчанию вместимость будет defaultComponentCapacity, по мере добавления элементов будет расширяться 
// Добавление мультикомпонента как и обычного компонента
// в случае дефолтной реализации
ref var items = ref entity.Add<Multi<Item>>();
// в случае кастомной реализации
ref var inventory = ref entity.Add<Inventory>();
ref var items = ref inventory.Items;

// Доступны все остальные бызовые методы работы с компонентами
entity.TryAdd<Multi<Item>>();
entity.HasAllOf<Multi<Item>>();
// ...

// При удалении мультикомпонента - список элементов будет автоматически очищен
entity.Delete<Multi<Item>>();
entity.TryDelete<Multi<Item>>();

// При копировании и перемещении мультикомпонента - будут автоматически скопированы все элементы
entity.CopyComponentsTo<Multi<Item>>(entity2);
entity.Clone();

entity.MoveComponentsTo<Multi<Item>>(entity2);
entity.MoveTo(entity2);

// Мультикомпонент ведет себя как динамический массив (список)
// Доступны следующие операции:
// Информационные:
ushort capacity = items.Capacity;                                      // Текущая вместимость
ushort count = items.Count;                                            // Количество элементов
bool empty = items.IsEmpty();                                          // True если нет элементов
bool notEmpty = items.IsNotEmpty();                                    // True если есть элементы
bool full = items.IsFull();                                            // True если текущая емкость заполнена

// Доступ:
ref Item element = ref items[1];                                       // Индексатор
ref Item first = ref items.First();                                    // Сслыка на первый элемент
ref Item last = ref items.Last();                                      // Ссылка на последний элемент
foreach (ref var item in items) {                                      // Цикл foreach по ссылкам элементов
    //..
}

for (ushort i = 0; i < items.Count; i++) {                             // Цикл for по элементам
    ref var item = ref items[i];
}

// Добавление и расширение:
items.Add(new Item());                                                 // Добавить элемент
items.Add(new Item("a"), new Item("b"), new Item("c"), new Item("d")); // Добавить элементы (1 - 4)
items.Add(new[] { new Item("f"), new Item("g") });                     // Добавить элементы из массива
items.Add(new[] { new Item("f"), new Item("g") }, 1, 1);               // Добавить элементы из массива c указанием старта и количества
items.Add(ref entity2.RefMut<Multi<Item>>());                          // Добавить элементы из другого компонента
items.Add(ref entity2.RefMut<Multi<Item>>(), 1, 1);                    // Добавить элементы из другого компонента c указанием старта и количества
items.InsertAt(idx: 1, new Item("e"));                                 // Вставить элемент в указанный индекс, остальные элементы будут сдвинуты
items.EnsureSize(10);                                                  // Обеспечить вместимость еще на N элементов если требуется
items.Resize(16);                                                      // Расширить вместимость до N если требуется

// Удаление и очистка
items.DeleteFirst();                                                   // Удалить первый элемент и сдвинуть последующие (если важен порядок эелментов)
items.DeleteFirstSwap();                                               // Удалить первый элемент и заменить последним (если НЕ важен порядок эелментов) (Быстрее)
items.DeleteLast();                                                    // Удалить последний элемент и сбросить значение в default
items.DeleteLastFast();                                                // Удалить последний элемент и БЕЗ сброса значения в default (если НЕ важен сброс значения) (Быстрее)
items.DeleteAt(idx: 1);                                                // Удалить элемент по индексу и сдвинуть последующие (если важен порядок эелментов)
items.DeleteAtSwap(idx: 1);                                            // Удалить элемент по индексу и заменить последним (если НЕ важен порядок эелментов) (Быстрее)
items.Clear();                                                         // Очистить элементы и сбросить в default
items.ResetCount();                                                    // Сброс количества без очистки

// Поиск
short idx = items.IndexOf(new Item("a"));                              // Получить индекс элемента или -1
bool contains = items.Contains(new Item("a"));                         // Проверить наличие элемента с дефолтным IEqualityComparer
bool contains = items.Contains(new Item("a"), comparer);               // Проверить наличие элемента с кастомным IEqualityComparer

// Дополнительно
var array = new Item[items.Capacity];                                  
items.CopyTo(array);                                                   // Копировать элементы в указанный массив
items.Sort();                                                          // Отсортировать элементы c дефолтным Comparer
items.Sort(comparer);                                                  // Отсортировать элементы с переданым Comparer
```