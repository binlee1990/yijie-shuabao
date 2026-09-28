# 浏览器 → Godot/C# 迁移记录

| 原实现 | 原生实现 | 迁移状态 |
|---|---|---|
| JS `catalog.js` | `Data/catalog.json` + `Catalog.cs` | 内容包成为运行时权威JSON；96/120/12闭包验证 |
| `model.js` | `State.cs` + `Model.cs` | 唯一人物、十属性、六阶、派生数值、排他岗位 |
| `random.js` | `RandomStream.cs` | 32位运算、地址化随机；黄金向量逐项相等 |
| `recruitment.js` | `Recruitment.cs` | 九随机+选择、保新、重复转片、待选恢复 |
| `battle.js` | `Battle.cs` | 自动回合、三波、状态、Boss蓄力与手动指令 |
| `economy.js` | `Economy.cs` | 胜利奖励、成长、装备、剧情、洞天与离线 |
| `engine.js` | `GameEngine.cs` | 唯一写入口、拟议状态、验证、持久化成功后提交 |
| localStorage | `SaveCodec.cs` | SHA256校验、临时文件+Flush+原子替换、备份与迁移 |
| HTML/CSS | `Game/Main.tscn` + `Main.cs` | 原生Godot Control界面；没有WebView |
| 素材清单 | `Assets/manifest.json` + `AssetStore.cs` | 稳定资源ID、缓存、Texture2D、原生音频 |

## 版本与存档

原生存档外壳 `shuabao-save/2`，主体schema2、rules=`shuabao-godot-0.2.0`。校验使用UTF-8 payload的SHA256，作用是检测损坏，不是反作弊签名。

浏览器 `shuabao-save/1` 先验证原FNV校验，再检查schema1与`shuabao-web-0.1.0`。迁移保留全部权威状态，并将旧Boss宝箱的裸装备候选包装为类型明确的Item claim。未决招募与行动边界战斗均有真实浏览器生成的固定夹具。

旧版种子、抽序、战斗序号和收据保留。测试比较了相同开包的候选、碎片、属性，及迁移中途战斗后的终局HP、行动数与胜负。此证据针对当前规则和夹具，不宣称任意浮点公式在未来平台版本都逐位兼容。核心仍采用double与正向Math.floor(x+0.5)兼容舍入，未改成原设想的全定点profile。

原生保存使用同目录临时文件，写入后`Flush(true)`，已存在主档时`File.Replace(temp, main, backup)`。持久化失败会阻止GameEngine内存状态推进。加载损坏主档时尝试备份，两者都坏则报错而不清空名册。进程持有独占文件句柄，避免两个原生实例写同一档。

## 不新增虚假内容

本次迁移保持已有的开发内容边界：六类职责技能、生成式敌人、24个模板剧情节点、条件式个人历练、被动生产建筑。没有声称补完96套独有技能、召唤单位、输入预约型产线、12种独立Boss或完整商业剧情。这些属于后续内容开发，不影响当前核心循环和终章可达性。

## 运行时边界

Core不引用Godot程序集，可在.NET控制台验证。Godot UI只调用GameEngine.Execute；表现使用原生节点，不依赖浏览器、Node或Python。内容校验在Catalog.Load发生；首版固定规则包不需要运行时SAT。窗口倍速只影响行动调用间隔，不改变随机采样地址。
