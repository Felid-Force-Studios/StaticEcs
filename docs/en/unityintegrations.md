---
title: Unity integration
parent: EN
nav_order: 5
---

### ⚙️ **[Unity editor module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️

# Unity integration

Example of StaticEcs integration with Unity:

```csharp
using System;
using FFS.Libraries.StaticEcs;
using FFS.Libraries.StaticEcs.Unity;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

// Define world type with editor name
[StaticEcsEditorName("World")]
public struct WT : IWorldType { }
public abstract class W : World<WT> { }

// Define systems
public struct GameSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }

// Components
public struct Position : IComponent {
    public Transform Value;
}

public struct Direction : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

// Scene data — passed from MonoBehaviour via resource
[Serializable]
public class SceneData {
    public GameObject EntityPrefab;
}

// Entity creation system
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

// Position update system
public struct UpdatePositions : ISystem {
    public void Update() {
        W.Query().For(
            static (ref Position position, in Velocity velocity, in Direction direction) => {
                position.Value.position += direction.Value * (Time.deltaTime * velocity.Value);
            }
        );
    }
}

// MonoBehaviour entry point
public class Startup : MonoBehaviour {
    public SceneData sceneData;

    private void Start() {
        // Create the world
        W.Create(WorldConfig.Default());

        // Register all types and connect debug (Unity module)
        W.Types().RegisterAll();
        UnityEventTypes.Register<WT>(); // Registers all Unity events and components
        EcsDebug<WT>.AddWorld<GameSystems>();

        // Initialize the world
        W.Initialize();

        // Pass scene data via resource
        W.SetResource(sceneData);

        // Create and configure systems
        GameSys.Create();
        GameSys.Add(new CreateRandomEntities(), order: -10)
            .Add(new UpdatePositions(), order: 0);
        GameSys.Initialize();

        // Connect system debugging
        EcsDebug<WT>.AddSystem<GameSystems>();
    }

    private void Update() {
        GameSys.Update();
        // Advance change tracking (changes become visible next frame)
        W.Tick();
    }

    private void OnDestroy() {
        GameSys.Destroy();
        W.Destroy();
    }
}
```
