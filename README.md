# 异界刷宝录 · Godot / C#

**Godot 4.5 .NET 原生单机工程，v0.2.0。** 从 SHUABAO 浏览器 v0.1.0 迁移。玩法核心为独立 C# 类库，界面使用 Godot Control、原生窗口、TextureRect、ProgressBar 和音频节点，不使用 WebView 或 JavaScript 运行时。

96位名将、120个三波关卡、12栋洞天建筑、252个分类图声占位；包含中文字体与开放许可。人物与剧情仍是开发内容，正式独有技能、剧情与商业平衡需继续制作。

## 打开与运行

1. 安装 **Godot 4.5 的 .NET 版编辑器**与 **.NET 8 SDK**。标准版 Godot 不支持本工程的 C# 脚本；只装 .NET Runtime 也不够。
2. 克隆仓库：

```sh
git clone https://github.com/binlee1990/yijie-shuabao.git
cd yijie-shuabao
dotnet build Shuabao.csproj
```

3. 用 Godot .NET 导入根目录 `project.godot`，等待素材导入后按 **F6/F5** 运行。首次构建需要从 NuGet 获取 Godot SDK；安装与还原完成后游戏可离线运行。

Windows 可设置 `GODOT_NET` 为 .NET 版 Godot 可执行文件路径，再运行 `start.bat`。Linux/macOS 使用 `GODOT_NET=/path/to/godot-mono sh start.sh`。通过编辑器打开则不必设置环境变量。

开发验证使用 Godot `4.5.stable.mono.official.876b29033` 与 .NET SDK `8.0.408`。工程 SDK 锁定 `Godot.NET.Sdk/4.5.0`，目标框架 `net8.0`；这是一组已验证版本，并非声称它是最新版本。

## 核心系统

| 系统 | 原生实现 |
|---|---|
| 招募 | 九份随机+第十席三选一；单抽累计、补齐收费、许愿、保新、重复转片、满池定向培养 |
| 剧情 | 24位专属角色；普通池不产生稀有SSR/UR及其碎片；五项有限个人历练 |
| 阵容 | 三前排三后排、位置交换、五套预设、六种职责、两人职责共鸣 |
| 战斗 | 三波状态继承、确定性回合、护盾/嘲讽/治疗/灼烧/标记/眩晕/反击、Boss蓄力、暂停指令、倍速 |
| 养成 | 十属性、Lv1–120、乾卦六阶、20/40/60/100/160突破片、追赶等级、公共战技 |
| 装备 | 四部位、四套装、三品质、词缀、定向锻造、+10必成强化、锁定、分解、500格和溢出保管 |
| 洞天 | 12建筑、3级即时升级、主线解锁、排他岗位、派驻翻倍、统一时钟 |
| 委托 | 已通关凭证、普通10分钟10券/Boss30分钟30券、48h封顶、暂停保留进度、在线离线一致 |
| 结局 | 主线96人已招募+第120关胜利；不要求全员满级满阶 |
| 存档 | 原生文件原子替换、备份、校验、进程写锁、导入导出、浏览器v0.1.0迁移 |

每次普通胜利10券，Boss胜利共30券，重复关卡也给。失败不掉角色和装备。首通Boss额外构筑宝箱三选一，与招贤席一样在关闭游戏后保留。

## 项目结构

| 路径 | 用途 |
|---|---|
| `project.godot`、`Shuabao.csproj` | Godot入口和原生客户端项目 |
| `Game/Main.tscn`、`Game/Main.cs` | 主场景、七个原生页面、对话、输入与战斗演出 |
| `Core/` | 不依赖Godot的C#规则、状态、事务、战斗、经济与存档类库 |
| `Data/catalog.json` | 运行时权威内容包，96人物/120关/12建筑 |
| `Assets/manifest.json` | 稳定素材ID→文件路径 |
| `Assets/ASSET-CATALOG.csv` | 美术目录表；正式替换规范见文档 |
| `Tests/` | 无第三方测试框架依赖的可执行测试、浏览器黄金样例 |
| `Docs/QA/` | 实测报告、全流程逐关记录、Godot无头日志 |
| `export_presets.cfg` | Windows/Linux导出预设，需自行安装相同版本模板 |
| `.github/workflows/ci.yml` | 构建、核心回归、Godot运行烟测 |

## 旧存档转移

在浏览器旧版“设置→导出存档”，然后在Godot版“设置→导入存档”选择该JSON。导入前校验原校验和、内容版本与数据约束；导入后转为schema2，并保留当前本机主档为备份。

已验证：普通名册、资源、碎片、阵容、装备、剧情、建筑、委托、未决招贤席、战斗行动边界。原生版新档不能反向导入浏览器版。**不要直接复制浏览器localStorage内部文本以外的页面文件作为存档。**

存档位置为 Godot `user://game.save.json`，设置页可显示路径并打开目录。Windows通常位于 `%APPDATA%/Godot/app_userdata/异界刷宝录/`。导入/导出文件不进入Git仓库。

## 验证

```sh
dotnet build Shuabao.csproj
dotnet run --project Tests/Shuabao.Tests.csproj -- --playthrough
godot-mono --headless --path . --editor --import
godot-mono --headless --path . -- --smoke-test
```

最后两行把 `godot-mono` 替换为本机 .NET 版可执行文件。烟测使用独立 `user://smoke/` 目录，不读写正式玩家存档。

本次结果：**29项核心检查全部通过；真实开局资源跑通120关、集齐96人并触发结局；Godot无头七页及核心交互流程通过。** 详细限制见 `Docs/VALIDATION.md`。

## 美术与内容

美术已分类保留，替换时优先修改 `Assets/manifest.json`；ID稳定，不需要改战斗代码。Godot版将浏览器中的WOFF字体另转为OTF供原生显示，许可位于 `Assets/fonts/OFL.txt`。正式素材替换与已绑定/预留项见 `Docs/ASSETS.md`。

本次是工程迁移，沿用原版96人的开发样本，不声称已导入 ai-cards-os 全部人物与技能，也没有把生成式关卡变成手工完整战役。Godot无头验证不等同于Windows发行验收，仓库不附带未经验证的EXE。

官方环境说明：[Godot 4.5 C# 文档](https://docs.godotengine.org/en/4.5/tutorials/scripting/c_sharp/) · [Godot 4.5 下载](https://godotengine.org/download/archive/4.5-stable/)
