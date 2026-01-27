---
title: Unity интеграция
parent: RU
nav_order: 5
---

### ⚙️ **[Unity editor module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️

# Unity интеграция

Пример интеграции StaticEcs с Unity:

```csharp
using System;
using FFS.Libraries.StaticEcs;
using FFS.Libraries.StaticEcs.Unity;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

// Определяем тип мира с именем для редактора
[StaticEcsEditorName("World")]
public struct WT : IWorldType { }
public abstract class W : World<WT> { }

// Определяем системы
public struct GameSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }

// Компоненты
public struct Position : IComponent {
    public Transform Value;
}

public struct Direction : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

// Данные сцены — передаются из MonoBehaviour через ресурс
[Serializable]
public class SceneData {
    public GameObject EntityPrefab;
}

// Система создания сущностей
public struct CreateRandomEntities : ISystem {
    public void Init() {
        ref var sceneData = ref W.GetResource<SceneData>();
        for (var i = 0; i < 100; i++) {
            var go = Object.Instantiate(sceneData.EntityPrefab);
            go.transform.position = new Vector3(Random.Range(0, 50), 0, Random.Range(0, 50));
            W.NewEntity<Default>().Set(
                new Position { Value = go.transform },
                new Direction { Value = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) },
                new Velocity { Value = 2f }
            );
        }
    }
}

// Система обновления позиций
public struct UpdatePositions : ISystem {
    public void Update() {
        W.Query().For(
            static (ref Position position, in Velocity velocity, in Direction direction) => {
                position.Value.position += direction.Value * (Time.deltaTime * velocity.Value);
            }
        );
    }
}

// MonoBehaviour точка входа
public class Startup : MonoBehaviour {
    public SceneData sceneData;

    private void Start() {
        // Создаём мир
        W.Create(WorldConfig.Default());

        // Регистрируем все типы и подключаем отладку (Unity модуль)
        W.Types().RegisterAll();
        UnityEventTypes.Register<WT>(); // Регистрирует все события и компоненты Unity
        EcsDebug<WT>.AddWorld<GameSystems>();

        // Инициализируем мир
        W.Initialize();

        // Передаём данные сцены через ресурс
        W.SetResource(sceneData);

        // Создаём и настраиваем системы
        GameSys.Create();
        GameSys.Add(new CreateRandomEntities(), order: -10)
            .Add(new UpdatePositions(), order: 0);
        GameSys.Initialize();

        // Подключаем отладку систем
        EcsDebug<WT>.AddSystem<GameSystems>();
    }

    private void Update() {
        GameSys.Update();
        // Продвижение трекинга изменений (изменения видны в следующем кадре)
        W.Tick();
    }

    private void OnDestroy() {
        GameSys.Destroy();
        W.Destroy();
    }
}
```
