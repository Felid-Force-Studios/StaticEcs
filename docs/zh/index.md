---
title: ZH
has_toc: false
parent: Main page
---

<p align="center">
  <img src="../fulllogo.png" alt="Static ECS" width="100%">
  <br><br>
  <img src="https://img.shields.io/badge/version-2.0.0-blue?style=for-the-badge" alt="Version">
  <a href="https://felid-force-studios.github.io/StaticEcs/zh/"><img src="https://img.shields.io/badge/Docs-文档-blueviolet?style=for-the-badge" alt="文档"></a>
  <a href="https://gist.github.com/blackbone/6d254a684cf580441bf58690ad9485c3"><img src="https://img.shields.io/badge/Benchmarks-基准测试-green?style=for-the-badge" alt="基准测试"></a>
  <a href="https://github.com/Felid-Force-Studios/StaticEcs-Unity"><img src="https://img.shields.io/badge/Unity-模块-orange?style=for-the-badge&logo=unity" alt="Unity 模块"></a>
  <a href="https://github.com/Felid-Force-Studios/StaticEcs-Showcase"><img src="https://img.shields.io/badge/Showcase-示例-yellow?style=for-the-badge" alt="Showcase"></a>
  <br><br>
  <a href="https://felid-force-studios.github.io/StaticEcs/zh/migrationguide.html"><img src="https://img.shields.io/badge/迁移指南-2.0.0-red?style=for-the-badge" alt="迁移指南"></a>
</p>

<p align="center">
  <a href="https://github.com/Felid-Force-Studios/StaticEcs/blob/master/CHANGELOG_2_0_0_ZH.md"><img src="https://img.shields.io/badge/🚀_2.0.0_新版本介绍-实体类型_·_变更追踪_·_Burst_·_块迭代_·_批量操作-ff6600?style=for-the-badge&labelColor=222222" alt="2.0.0 新版本介绍"></a>
</p>

# Static ECS - C# 分层倒排位图 ECS 框架
- 高性能
- 轻量级
- 零内存分配
- 低内存占用
- 核心代码无 Unsafe
- 基于静态类和结构体
- 类型安全
- 零成本抽象
- 强大的查询引擎，支持并行化
- 批量实体操作
- 组件和标签变更追踪
- 按类型和集群分组实体
- 实体关系系统
- 世界快照序列化
- 事件系统
- 无样板代码
- 兼容 Unity，支持 Il2Cpp 和 [Burst](https://github.com/Felid-Force-Studios/StaticEcs-Unity?tab=readme-ov-file#templates)
- 兼容其他 C# 引擎
- 兼容 Native AOT

## 目录
* [联系方式](#联系方式)
* [安装](#安装)
* [概念](#概念)
* [快速开始](#快速开始)
* [功能](features.md)
    * [实体](features/entity.md)
    * [实体全局标识符](features/gid.md)
    * [组件](features/component.md)
    * [标签](features/tag.md)
    * [多组件](features/multicomponent.md)
    * [关系](features/relations.md)
    * [世界](features/world.md)
    * [系统](features/systems.md)
    * [资源](features/resources.md)
    * [查询](features/query.md)
    * [事件](features/events.md)
    * [变更追踪](features/tracking.md)
    * [序列化](features/serialization.md)
    * [编译器指令](features/compilerdirectives.md)
* [性能](performance.md)
* [Unity 集成](unityintegrations.md)
* [常见陷阱](pitfalls.md)
* [AI 代理指南](aiagentguide.md)
* [许可证](#许可证)


# 联系方式
* [felid.force.studios@gmail.com](mailto:felid.force.studios@gmail.com)
* [Telegram](https://t.me/felid_force_studios)

# 支持项目
如果您喜欢 Static ECS 并且它对您的项目有所帮助，您可以支持开发：

<a href="https://www.buymeacoffee.com/felid.force.studios" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" height="60"></a>

# 安装
本库依赖 [StaticPack](https://github.com/Felid-Force-Studios/StaticPack) `1.1.0` 进行二进制序列化，StaticPack 也需要一并安装
* ### 以源代码形式
  从发布页面或从分支下载归档文件。`master` 分支包含稳定测试版本
* ### Unity 安装
  通过 Unity PackageManager 的 git 模块：
  ```
  https://github.com/Felid-Force-Studios/StaticEcs.git
  https://github.com/Felid-Force-Studios/StaticPack.git
  ```
  或添加到 `Packages/manifest.json` 清单文件：
  ```json
  "com.felid-force-studios.static-ecs": "https://github.com/Felid-Force-Studios/StaticEcs.git"
  "com.felid-force-studios.static-pack": "https://github.com/Felid-Force-Studios/StaticPack.git"
  ```
* ### NuGet
  ```
  dotnet add package FFS.StaticEcs
  ```
  用于带断言的调试构建：
  ```
  dotnet add package FFS.StaticEcs.Debug
  ```
  包：[FFS.StaticEcs](https://www.nuget.org/packages/FFS.StaticEcs/) · [FFS.StaticEcs.Debug](https://www.nuget.org/packages/FFS.StaticEcs.Debug/)

# AI Agent 集成
如果您使用 AI 编码助手（Claude Code、Cursor、Copilot 等）与 StaticEcs：
- **llms.txt**：将代理指向 [`https://felid-force-studios.github.io/StaticEcs/llms.txt`](https://felid-force-studios.github.io/StaticEcs/llms.txt) 获取简洁的 AI 可读参考
- **完整上下文**：[`https://felid-force-studios.github.io/StaticEcs/llms-full.txt`](https://felid-force-studios.github.io/StaticEcs/llms-full.txt) 获取完整文档
- **Claude Code**：将 [CLAUDE.md 代码片段](aiagentguide.md)复制到项目的 `CLAUDE.md` 中
- **常见问题**：参见[常见错误指南](pitfalls.md)


# 概念
StaticEcs — 一种基于倒排分层位图模型的新型 ECS 架构。
与依赖原型或稀疏集的传统 ECS 框架不同，该设计引入了倒排索引结构，其中每个组件类型拥有实体位图，而不是实体存储组件掩码。
这些位图的分层聚合提供了对数级空间的实体块索引，实现了 O(1) 块过滤和通过位运算的高效并行迭代。
该方法完全消除了原型迁移和稀疏集间接寻址，提供直接的 SoA 式内存访问，缓存未命中最小化。
该模型每个块的内存查询次数减少多达 64 倍，并随活跃组件集数量线性扩展，非常适合大规模仿真、带流式加载的开放世界、需要状态同步的网络游戏、拥有数千代理的响应式 AI，以及组件组合频繁变化的项目（增益、效果、状态）。

在原型式 ECS（Unity DOTS、Flecs、Bevy、Arch）中，每次添加或移除组件都会触发实体迁移——将所有数据复制到新原型，组件组合数量的增长导致原型爆炸。
在稀疏集 ECS（EnTT、DefaultEcs）中，组件访问需要通过稀疏表进行间接寻址，每次查找至少两次缓存未命中。
StaticEcs 消除了这两个问题：每个实体占据分段数组中的固定槽位，永远不会在内存中移动——Add/Remove 是对存在掩码的 O(1) 位翻转操作，无需数据复制。内存中稳定的实体地址使得通过版本化标识符（EntityGID）实现低成本的实体关系成为可能，包括在流式加载中部分关联实体处于未加载区域的场景——非常适合开放世界中的复杂仿真。组件类型数量不影响存储结构，因为每种类型独立拥有自己的掩码。EntityType × Cluster 二维分区进一步确保缓存局部性：同一集群内同类型的实体占据相邻的内存段，而集群允许加载和卸载整个空间区域而不影响其余数据。

内存按层级组织：块（4096 个实体）→ 段（256）→ 区块（64）。世界查询首先在块级别对启发式掩码进行 AND 运算——单次位运算覆盖多达 4096 个实体，整体跳过空区块——然后在 64 实体区块级别进行精确过滤。批量操作（BatchAdd、BatchRemove、BatchSet）通过一次位运算处理多达 64 个实体。


> - 该实现的核心理念是静态化：所有世界和组件数据都存储在静态泛型类（`World<TWorld>`）中，从而避免昂贵的虚调用和分配，提供便捷的 API 和丰富的语法糖。JIT 编译器会为未使用的组件钩子消除死代码
> - 该框架专注于最大化的易用性、速度和编码舒适度，同时不牺牲性能
> - 支持多世界创建、严格类型化、零成本抽象
> - 二进制序列化系统，支持世界、集群和单实体快照，模式版本控制和压缩
> - 实体关系系统，自动双向钩子，用于层级、分组和关联
> - 响应式变更追踪，用于网络同步、UI 和触发器
> - 多组件——每实体可变长度数据（背包、增益效果），无堆分配
> - 多线程处理，支持并行查询和块级安全保证
> - 低内存占用，SoA 布局（数组结构体）——相同类型的组件存储在连续数组中
> - 基于位图架构，无原型，无稀疏集
> - 该框架为私有项目需求而创建，并以开源形式发布。

# 快速开始
```csharp
using FFS.Libraries.StaticEcs;

// 定义世界类型
public struct WT : IWorldType { }

// 定义类型别名以便于访问
public abstract class W : World<WT> { }

// 定义系统类型
public struct GameSystems : ISystemsType { }

// 定义系统类型别名
public abstract class GameSys : W.Systems<GameSystems> { }

// 定义组件
public struct Position : IComponent { public Vector3 Value; }
public struct Direction : IComponent { public Vector3 Value; }
public struct Velocity : IComponent { public float Value; }

// 定义系统
public struct VelocitySystem : ISystem {
    public void Update() {
        // 通过 foreach 迭代
        foreach (var entity in W.Query<All<Position, Velocity, Direction>>().Entities()) {
            ref var pos = ref entity.Ref<Position>();
            ref readonly var dir = ref entity.Read<Direction>();
            ref readonly var vel = ref entity.Read<Velocity>();
            pos.Value += dir.Value * vel.Value;
        }

        // 或通过委托（更快，零分配）
        W.Query().For(
            static (ref Position pos, in Velocity vel, in Direction dir) => {
                pos.Value += dir.Value * vel.Value;
            }
        );
    }
}

public class Program {
    public static void Main() {
        // 创建世界
        W.Create(WorldConfig.Default());

        // 自动注册当前程序集中的所有组件、标签、事件等
        W.Types().RegisterAll();

        // 初始化世界
        W.Initialize();

        // 创建并配置系统
        GameSys.Create();
        GameSys.Add(new VelocitySystem(), order: 0);
        GameSys.Initialize();

        // 创建带有组件的实体
        var entity = W.NewEntity<Default>().Set(
            new Position { Value = Vector3.Zero },
            new Direction { Value = Vector3.UnitX },
            new Velocity { Value = 1f }
        );

        // 更新所有系统 - 每帧调用
        GameSys.Update();
        // 推进变更追踪（变更在下一帧可见）
        W.Tick();

        // 销毁系统
        GameSys.Destroy();
        // 销毁世界并清理所有数据
        W.Destroy();
    }
}
```

# 许可证
[MIT license](https://github.com/Felid-Force-Studios/StaticEcs/blob/master/LICENSE.md)
