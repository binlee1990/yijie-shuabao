# Godot 美术替换与目录

资源目录从浏览器工程迁移到 `Assets/`，原252个图声占位均保留。`Assets/manifest.json`中的`assets/...`是跨客户端的逻辑路径，Godot的AssetStore将此前缀映射为`res://Assets/...`；不要手动混写大小写。

| 目录 | 内容 | 当前绑定 |
|---|---|---|
| characters/portraits | 96张3:4人物卡面 | 名将、战斗、招募、剧情 |
| characters/avatars | 96个方形头像 | 预留；当前使用卡面裁切 |
| characters/sprites | 动作资源目录 | 预留；当前战斗为卡片表现 |
| enemies | 四类敌人/Boss占位 | 原生战斗 |
| buildings | 12栋透明底建筑 | 洞天、召唤台 |
| backgrounds | 四区与洞天场景 | 征途；洞天底图留作场景扩展 |
| items/equipment | 四部位图标 | 装备宝库、Boss宝箱 |
| items/materials | 材料资源目录 | 预留，现有材料图标在ui/icons |
| ui/icons | 导航、资源、状态、结算 | 导航/结算等按需消费 |
| ui/frames、ui/buttons | 贴图化资源目录 | 预留；当前用Godot StyleBoxFlat |
| vfx | 五种透明特效图 | 预留；当前使用状态/战报反馈 |
| audio/sfx | 合成提示音 | 招募选择、胜利；默认关闭 |
| audio/music、voices、ambience | 配乐/声音/环境占位 | 预留，不自动播放静音或测试低音 |
| fonts | 53个WOFF2原件和53个原生OTF转换件 | 原生使用OTF，保留OFL许可 |

## 替换

1. 对照 `Assets/ASSET-CATALOG.csv` 的资源ID和路径，把正式图放入同一分类目录。
2. 若文件名或格式改变，更新 `manifest.json` 的path；稳定资源ID不变。
3. 让Godot重新导入资源，再运行查看缩放、透明边缘和面部安全区。
4. 人物卡面建议720×960 PNG/WebP，建筑建议640×480透明PNG，背景建议1920×1080；图标在24px显示时仍应可读。
5. 不把名字、稀有度、等级烘焙进人物图；这些由界面叠加。

源SVG文字已经转轮廓，避免系统缺中文字体。界面优先使用系统中文字体，否则回退到随附OTF。更换字体应同步更新AssetStore.Font和导出include_filter，并保留授权文件。OTF由同版本Noto Serif SC WOFF转换容器，没有重做字体字形。

资源表只列最初252图声占位与53个源字体切片；额外OTF属于格式适配副本，不虚增原创素材数量。CSV自动生成的`.translation`是Godot导入缓存衍生物，不是游戏剧情内容。
