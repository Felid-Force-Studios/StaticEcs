---
title: Unity 集成
parent: ZH
nav_order: 5
---

### ⚙️ **[Unity editor module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️

# Unity 集成

StaticEcs 与 Unity 集成示例：

```csharp
using System;
using FFS.Libraries.StaticEcs;
using FFS.Libraries.StaticEcs.Unity;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

// 定义世界类型并设置编辑器名称
[StaticEcsEditorName("World")]
public struct WT : IWorldType { }
public abstract class W : World<WT> { }

// 定义系统
public struct GameSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }

// 组件
public struct Position : IComponent {
    public Transform Value;
}

public struct Direction : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

// 场景数据 — 通过资源从 MonoBehaviour 传递
[Serializable]
public class SceneData {
    public GameObject EntityPrefab;
}

// 实体创建系统
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

// 位置更新系统
public struct UpdatePositions : ISystem {
    public void Update() {
        W.Query().For(
            static (ref Position position, in Velocity velocity, in Direction direction) => {
                position.Value.position += direction.Value * (Time.deltaTime * velocity.Value);
            }
        );
    }
}

// MonoBehaviour 入口点
public class Startup : MonoBehaviour {
    public SceneData sceneData;

    private void Start() {
        // 创建世界
        W.Create(WorldConfig.Default());

        // 注册所有类型并连接调试（Unity 模块）
        W.Types().RegisterAll();
        UnityEventTypes.Register<WT>(); // 注册所有 Unity 事件和组件
        EcsDebug<WT>.AddWorld<GameSystems>();

        // 初始化世界
        W.Initialize();

        // 通过资源传递场景数据
        W.SetResource(sceneData);

        // 创建和配置系统
        GameSys.Create();
        GameSys.Add(new CreateRandomEntities(), order: -10)
            .Add(new UpdatePositions(), order: 0);
        GameSys.Initialize();

        // 连接系统调试
        EcsDebug<WT>.AddSystem<GameSystems>();
    }

    private void Update() {
        GameSys.Update();
        // 推进变更追踪（变更在下一帧可见）
        W.Tick();
    }

    private void OnDestroy() {
        GameSys.Destroy();
        W.Destroy();
    }
}
```
