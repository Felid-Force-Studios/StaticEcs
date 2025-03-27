---
title: Unity integration
parent: EN
nav_order: 5
---

### ⚙️ **[Unity editor module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️


# Unity integration

Example startup:
```csharp
using System;
using UnityEngine;
using FFS.Libraries.StaticEcs;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public struct MyWorldType : IWorldType { }
public struct MySystemsType : ISystemsType { }

public abstract class MyEcs : Ecs<MyWorldType> { }
public abstract class MyWorld : MyEcs.World { }
public abstract class MySystems : MyEcs.Systems<MySystemsType> { }

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
            var gameObject = Object.Instantiate(MyEcs.Context<SceneData>.Get().EntityPrefab);
            gameObject.transform.position = new Vector3(Random.Range(0, 50), 0, Random.Range(0, 50));
            MyEcs.Entity.New(
                new Position { Value = gameObject.transform },
                new Direction { Value = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)) },
                new Velocity { Value = 2f });
        }
    }
}

public struct UpdatePositions : IUpdateSystem {
    public void Update() {
        MyWorld.QueryComponents.For((MyEcs.Entity entity, ref Position position, ref Velocity velocity, ref Direction direction) => {
            position.Value.position += direction.Value * (Time.deltaTime * velocity.Value);
        });
    }
}

public class Main : MonoBehaviour {
    public SceneData sceneData;
    
    void Start() {
        MyEcs.Create(EcsConfig.Default());
                
        MyWorld.RegisterComponentType<Position>();
        MyWorld.RegisterComponentType<Direction>();
        MyWorld.RegisterComponentType<Velocity>();
        
        MyEcs.Initialize();
        
        MyEcs.Context<SceneData>.Set(sceneData);
        
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
        MyEcs.Destroy();
    }
}
```