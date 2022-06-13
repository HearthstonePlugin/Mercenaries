# [炉石传说佣兵挂机插件](https://github.com/Pik-4/HS_MRC_BepInEx)

#### Author: Pik4; Original Author: zyz2000

基于[BepInEx](https://github.com/BepInEx/BepInEx)开发的炉石传说佣兵战纪插件，推荐和[MixMod](https://4pda.to/forum/index.php?showtopic=870696)搭配使用。

### 文件、目录说明

[MercenariesHelper.sln](https://github.com/Pik-4/HS_MRC_BepInEx/blob/main/MercenariesHelper.sln) 主程序解决方案

[strategy.sln](https://github.com/Pik-4/HS_MRC_BepInEx/blob/main/strategy.sln) 策略解决方案

[binaryFile](https://github.com/Pik-4/HS_MRC_BepInEx/tree/main/binaryFile) 预编译的主程序以及策略文件

[teamConifg](https://github.com/Pik-4/HS_MRC_BepInEx/tree/main/teamConifg) 一些策略，目前主要用作PVP（PVE也可使用）

[zyz2000-mercenary](https://github.com/Pik-4/HS_MRC_BepInEx/tree/main/zyz2000-mercenary) 原始项目

[BepInPluginTemplate](https://github.com/Pik-4/HS_MRC_BepInEx/blob/main/BepInPluginTemplate.sln) BepInEx插件开发模板

### 安装说明

1. 分别编译`MercenariesHelper`与`strategy`。
2. 配置`BepInEx`，该部分可以参考[这里](https://4pda.to/forum/index.php?showtopic=870696&st=4780#entry114865283)。
  2.1. 下载[BepInEx_x86](https://github.com/BepInEx/BepInEx/releases)，并将其解压到炉石根目录`Hearthstone\`下。
  2.2. 创建一个目录`Hearthstone\BepInEx\unstripped_corlib\`；下载[Mono](https://unity.bepinex.dev/corlibs/2019.4.37.zip)和[Unity](https://unity.bepinex.dev/libraries/2019.4.37.zip)，并将其解压在该目录。
  2.3. 修改`Hearthstone\doorstop_config.ini`，将`dllSearchPathOverride=` 替换成`dllSearchPathOverride=BepInEx\unstripped_corlib`
3. 安装BepInEx配置管理[BepInExConfigManager.Mono](https://github.com/sinai-dev/BepInExConfigManager/releases)，解压到`Hearthstone\BepInEx\`即可；进入游戏后`F5`进行相关控制。
4. 将编译好的两个`dll`文件，存放在`Hearthstone\BepInEx\plugins`即可。

### 使用说明

1. 进入游戏后按下`F5`，找到佣兵挂机插件进行配置（需要点击保存，或者修改`BepInExConfigManager`的`AutoSave`）。注：策略要带上`.dll`，空策略为任务策略。
2. 按下`F9`开始挂机。
3. 当按下`F10`时，会列出当前你的佣兵队伍以及所有关卡ID。并打开`Config`目录。此时，你可以修改创建`MercenaryTeam.cfg`（该文件会在加载`strategy.dll`后，在战斗界面自行创建，创建策略的是王冰冰），并根据模板自行修改即可。

### 策略说明

1. 行前缀`#`、`+`、`-`、`>`、`~`分别代表注释、优先技能、排除上场、优先攻击、联动技能。
2. 当没有行前缀时，则按顺序加入佣兵策略，上场顺序与该策略一致。
3. 策略说明：以`-`作为分割符，分割每一个功能块，除佣兵名称、技能顺序、技能指向之外，其他参数均为可选。详细可参考默认配置。

### TODO

1. 重构主程序代码。
2. 增加技能循环执行。
3. 增加技能不释放。
4. 优化技能联动。
5. 优化优先技能（直接排序列表即可，不需要判断是否在场）。
6. 自动合成、升级佣兵。
7. 自动替换佣兵。
