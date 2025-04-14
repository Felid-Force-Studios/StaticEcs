---
title: Unity интеграция
parent: RU
nav_order: 5
---

### ⚙️ **[Unity editor module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️


# Unity интеграция

Пример:
```csharp
using System;
using UnityEngine;
using FFS.Libraries.StaticEcs;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public struct WT : IWorldType { }
public struct SystemsType : ISystemsType { }

public abstract class W : World<WT> { }
public abstract class MySystems : W.Systems<SystemsType> { }

public struct Position : IComponent {
    public Transform Value;
}

public struct Direction : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

[Serializable]
public struct SceneData {
    public GameObject EntityPrefab;
}

public struct CreateRandomEntities : IInitSystem {
    public void Init() {
        for (var i = 0; i < 100; i++) {
            var go = Object.Instantiate(W.Context<SceneData>.Get().EntityPrefab);
            go.transform.position = new Vector3(Random.Range(0, 50), 0, Random.Range(0, 50));
            W.Entity.New(
                new Position { Value = gameObject.transform },
                new Direction { Value = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) },
                new Velocity { Value = 2f });
        }
    }
}

public struct UpdatePositions : IUpdateSystem {
    public void Update() {
        W.QueryComponents.For((W.Entity entity, ref Position position, ref Velocity velocity, ref Direction direction) => {
            position.Value.position += direction.Value * (Time.deltaTime * velocity.Value);
        });
    }
}

public class Main : MonoBehaviour {
    public SceneData sceneData;
    
    void Start() {
        W.Create(WorldConfig.Default());
                
        W.RegisterComponentType<Position>();
        W.RegisterComponentType<Direction>();
        W.RegisterComponentType<Velocity>();
        
        W.Initialize();
        
        W.Context<SceneData>.Set(sceneData);
        
        MySystems.Create();
        
        MySystems.AddCallOnce(new CreateRandomEntities());
        MySystems.AddUpdate(new UpdatePositions());
        
        MySystems.Initialize();
    }

    void Update() {
        MySystems.Update();
    }

    private void OnDestroy() {
        MySystems.Destroy();
        W.Destroy();
    }
}
```