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
using FFS.Libraries.StaticEcs;
using FFS.Libraries.StaticEcs.Unity;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[StaticEcsEditorName("World")]
public struct WT : IWorldType { }
public abstract class W : World<WT> { }
public abstract class WEvents : World<WT>.Events { }
public struct SystemsType : ISystemsType { }
public abstract class Systems : W.Systems<SystemsType> { }

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
public class WSceneData {
    public GameObject EntityPrefab;
}

public struct CreateRandomEntities : IInitSystem {
    public void Init() {
        for (var i = 0; i < 100; i++) {
            var go = Object.Instantiate(W.Context<WSceneData>.Get().EntityPrefab);
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
        W.Query.For((W.Entity entity, ref Position position, ref Velocity velocity, ref Direction direction) => {
            position.Value.position += direction.Value * (Time.deltaTime * velocity.Value);
        });
    }
}

public class Startup : MonoBehaviour {
    public WSceneData sceneData;

    private void Start() {
        W.Create(WorldConfig.Default());

        W.RegisterComponentType<Position>();
        W.RegisterComponentType<Direction>();
        W.RegisterComponentType<Velocity>();

        EcsDebug<WT>.AddWorld();
        AutoRegister<WT>.Apply();

        W.Initialize();

        W.Context<WSceneData>.Set(sceneData);

        Systems.Create();
        
        Systems.AddCallOnce(new CreateRandomEntities());
        Systems.AddUpdate(new UpdatePositions());
        
        Systems.Initialize();
        EcsDebug<WT>.AddSystem<SystemsType>();
    }

    private void Update() {
        Systems.Update();
    }

    private void OnDestroy() {
        Systems.Destroy();
        W.Destroy();
    }

}
```