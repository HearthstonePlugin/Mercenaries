# [炉石传说佣兵挂机插件](https://github.com/Pik-4/Hearthstone_Mercenaries_BepInEx)

**Author: Pik4; Original Author: zyz2000**

基于[BepInEx](https://github.com/BepInEx/BepInEx)开发的炉石传说佣兵战纪插件，推荐和[MixMod](https://4pda.to/forum/index.php?showtopic=870696)搭配使用。

### 免责声明
使用辅助进行游戏的行为违反了暴雪用户协议 1.C.ii.2 使用机器人程序软件（BOT）： 任何未经暴雪和/或运营方明确授权，允许自动控制游戏，服务器和/或任何组件或其功能的的代码或/和软件，如自动操控游戏中的角色；因此根据协议，暴雪和/或运营方可以暂停、撤销或终止您使用本平台或本平台的部分功能或组成部分的许可。 
本项目仅用作`C#`的学习研究，由于使用该项目导致账号被封禁，本人不承担任何责任。如果该插件的开源分享行为损害了暴雪/运营方的利益，请联系我停止更新/删库，我会全力配合。 

### 问题处理

如果有问题请提交Issues。

### 加入该项目

当您的第一个PR通过后，我会邀请您加入该项目，成为collaborator。

### 文件、目录说明

[MercenariesHelper.sln](https://github.com/Pik-4/Hearthstone_Mercenaries_BepInEx/blob/main/MercenariesHelper.sln) 主程序解决方案

[strategy.sln](https://github.com/Pik-4/Hearthstone_Mercenaries_BepInEx/blob/main/strategy.sln) 策略解决方案

[binaryFile](https://github.com/Pik-4/Hearthstone_Mercenaries_BepInEx/tree/main/binaryFile) 预编译的主程序以及策略文件

[teamConifg](https://github.com/Pik-4/Hearthstone_Mercenaries_BepInEx/tree/main/teamConifg) 一些策略，目前主要用作PVP（PVE也可使用）

[zyz2000-mercenary](https://github.com/Pik-4/Hearthstone_Mercenaries_BepInEx/tree/main/zyz2000-mercenary) 原始项目

[BepInPluginTemplate](https://github.com/Pik-4/Hearthstone_Mercenaries_BepInEx/blob/main/BepInPluginTemplate.sln) BepInEx插件开发模板


### 编译说明

1. 安装`Visual Studio`，安装`C#`相关功能。
2. 打开`MercenariesHelper.sln`，添加引用，尝试编译，直到报错不缺引用(智能提示不提示`using`)为止。
3. 编译`strategy.sln`，编译策略时需要`MercenariesHelper.dll`，故2、3顺序固定。

注：引用优先在`Bepinex\unstripped_corlib`，其次在`%PROGRAMFILES(x86)%\Hearthstone\Hearthstone_Data\Managed`中查找。结合`MercenariesHelper.csproj`分析即可。

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
4. `火焰龙.dll`（火焰队PVE策略，成分为：巴琳达、迦顿、拉格纳罗斯、希奈丝特拉、珑心、奥妮克希亚）

### TODO

等个有缘人来完善。

1. **代码重构**（特别是主程序代码重构、策略合并进入主程序），修复各类延迟（添加sleep函数）。
2. 代码优化（优化程序执行逻辑、提高执行效率）。
3. 增加技能循环执行。
4. 增加技能不释放。
5. 优化技能抉择。（当前为执行抉择技能最后一个选项）
6. 优化技能联动。（当前逻辑可能存在些许混乱）
7. 优化优先技能（直接排序列表即可，不需要判断是否在场）
8. 多策略执行。（根据PVP佣兵颜色变阵，或根据PVE抉择合适策略）
9. 自动合成、升级佣兵。
10. 自动替换佣兵。
11. 自动推图（火焰队按顺序解锁未解锁的关卡，考虑做瓦莉拉任务）
12. PVE宝藏抉择（过英雄难度，双重宝箱处理）
13. 增加任务处理体系（长时间进度不变时，将佣兵加入黑名单，并自动放弃任务）。
14. 定向优化PVE任务（技能类别，斩杀指定类型随从）。
15. ~~独立UI~~`BepInExConfigManager`用着挺方便的，暂不考虑开发UI。故这里考虑优化配置选项。
16. PVP随机选择队伍（匹配策略）。
17. 对局记录、添加推送。
18. 优化中控，向中控发送心跳。
19. ~~合并MixMod的部分或全部功能（可以考虑开坑做成炉石助手）~~
20. 未完待续

