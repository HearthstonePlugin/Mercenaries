using System.Reflection;
using BepInEx;
using PegasusShared;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MercenariesHelper
{
    [BepInPlugin("MercenariesHelper", "佣兵挂机插件", "1.0.3.0")]
    public class MercenariesHelper : BaseUnityPlugin
    {

        public static bool enableAutoPlay = false;
        public static bool Initialize = false;
        public static float FindingGameTime;
        public static float sleeptime;
        public static bool fakeClick;
        public static bool fakeClickDown;
        public static bool fakeClickUp;
        public static bool fakePos;
        public static int fakeClickDownCount = 0;
        public static float idleTime;
        public static float fakeMouseX;
        public static float fakeMouseY;
        //private static float queuetimer;
        //private static IntPtr WindowHandle;
        public static BepInEx.Configuration.ConfigEntry<bool> autorun;
        public static BepInEx.Configuration.ConfigEntry<bool> 商店推销;
        public static BepInEx.Configuration.ConfigEntry<bool> 只打电脑;
        public static BepInEx.Configuration.ConfigEntry<bool> PVP;
        public static BepInEx.Configuration.ConfigEntry<string> PVPteam;
        public static BepInEx.Configuration.ConfigEntry<string> PVEteam;
        public static BepInEx.Configuration.ConfigEntry<bool> autoConcede;
        public static BepInEx.Configuration.ConfigEntry<int> Concedeline;
        public static BepInEx.Configuration.ConfigEntry<bool> autoSwitch;
        public static BepInEx.Configuration.ConfigEntry<int> SwitchLine;
        public static BepInEx.Configuration.ConfigEntry<int> 地图ID;
        public static BepInEx.Configuration.ConfigEntry<bool> PVE模式;      //true任务模式 false刷图模式
        public static BepInEx.Configuration.ConfigEntry<int> 步数;
        public static BepInEx.Configuration.ConfigEntry<string> PVP策略;
        public static BepInEx.Configuration.ConfigEntry<string> PVE策略;
        public static BepInEx.Configuration.ConfigEntry<long> 开始时间;
        public static BepInEx.Configuration.ConfigEntry<bool> Hidemain;
        public static BepInEx.Configuration.ConfigEntry<int> 投降延迟;
        public static bool 认输;
        public static int concedeDelay;
        public static bool isPVP;
        public static bool isViewCount;
        public static bool PVEMode;
        public static int PVEstep;
        public static string StrategyPVP;
        public static string StrategyPVE;
        public static long StartTime;
        public static bool onlyPC;
        public static string PVPteamName;
        public static string PVEteamName;
        public static int mapID;
        public static int 分数线;
        public static bool 自动切换;
        public static int 切换线;
        //private float 投降时间;
        private static List<PegasusLettuce.LettuceMapNode> minNode = new List<PegasusLettuce.LettuceMapNode>();
        private static bool flag = true;
        private static bool isHaveRewardTask;
        private static List<string> TaskMercenary = new List<string>();
        private static List<string> TaskAbilityName = new List<string>();
        private static string path;
        private static string loginfo;
        private static object StrategyInstance;
        private static MethodInfo Entrance;
        private static MethodInfo Battle;
        private static bool StrategyOK;
        private static int phaseID;
        private static bool StrategyRun = false;
        public static Queue<Entity> EntranceQueue = new Queue<Entity>();
        public static Queue<Battles> BattleQueue = new Queue<Battles>();
        private static Battles battles;
        private static bool HandleQueueOK = true;

        public struct Battles
        {
            public Entity source;
            public Entity target;
            public Entity Ability;
            public string SubName;
        };

        public struct State
        {
            public bool IsPVP
            {
                get
                {
                    return isPVP;
                }
            }
            public long 开始时间
            {
                get
                {
                    return StartTime;
                }
            }
            public List<string> 任务佣兵
            {
                get
                {
                    return TaskMercenary;
                }
            }
            public List<string> 任务技能
            {
                get
                {
                    return TaskAbilityName;
                }
            }
        };

        // 在插件启动时会直接调用Awake()方法
        void Awake()
        {
            // 使用Debug.Log()方法来将文本输出到控制台
            Debug.Log("Enabled!!!");
        }


        // 在所有插件全部启动完成后会调用Start()方法，执行顺序在Awake()后面；
        void Start()
        {
            path = @"BepInEx\idleTime.log";
            loginfo = @"BepInEx\Mercenaries.log";
            Config.Clear();                             //上面这一段是清除多余的配置
            autorun = Config.Bind("配置", "AutoRun", false, "是否自动佣兵挂机");
            enableAutoPlay = autorun.Value;
            PVP = Config.Bind("配置", "PVP", false, "PVP或者PVE");
            isPVP = PVP.Value;
            商店推销 = Config.Bind("配置", "商店推销", false, "false屏蔽商店推销");
            isViewCount = 商店推销.Value;
            PVE模式 = Config.Bind("配置", "PVE模式", true, "true任务模式 false刷图模式");
            PVEMode = PVE模式.Value;
            步数 = Config.Bind("配置", "步数", 2, "距离神秘选项怪物数，超过则重开地图。");
            PVEstep = 步数.Value;
            地图ID = Config.Bind("配置", "地图ID", 92, "地图ID");
            mapID = 地图ID.Value;
            PVPteam = Config.Bind("配置", "PVPteam", "", "PVP队伍名字");
            PVPteamName = PVPteam.Value;
            PVEteam = Config.Bind("配置", "PVEteam", "", "PVE队伍名字");
            PVEteamName = PVEteam.Value;
            PVP策略 = Config.Bind("配置", "PVP策略", "", "PVP策略文件名");
            StrategyPVP = PVP策略.Value;
            PVE策略 = Config.Bind("配置", "PVE策略", "", "PVE策略文件名");
            StrategyPVE = PVE策略.Value;
            只打电脑 = Config.Bind("配置", "onlyPC", false, "PVP只打电脑");
            onlyPC = 只打电脑.Value;
            autoConcede = Config.Bind("配置", "投降", false, "是否自动认输");
            认输 = autoConcede.Value;
            投降延迟 = Config.Bind("配置", "投降延迟", 0, "投降延迟（毫秒）");
            concedeDelay = 投降延迟.Value;

            Concedeline = Config.Bind("配置", "分数线", 6000, "自动认输分数线，高于此分数自动认输");
            分数线 = Concedeline.Value;
            autoSwitch = Config.Bind("配置", "切换", false, "是否切换至PVE");
            自动切换 = autoSwitch.Value;
            SwitchLine = Config.Bind("配置", "切换线", 12000, "达到此分数时切换至PVE");
            切换线 = SwitchLine.Value;
            开始时间 = Config.Bind("配置", "开始时间", 0L, "对局开始时间");
            StartTime = 开始时间.Value;
            //GetVer();   //对比sha1，不对则下载
            LoadPolicy();

            Harmony harmony = new Harmony("MercenariesHelper.patch");
            harmony.PatchAll();

            //MethodInfo method1 = typeof(InputCollection).GetMethod("GetMousePosition");
            //MethodInfo method2 = typeof(MercenariesHelper).GetMethod("MousePos");
            //harmony.Patch(method1, null, new HarmonyMethod(method2));

            //method1 = typeof(InputCollection).GetMethod("GetMouseButton");
            //method2 = typeof(MercenariesHelper).GetMethod("MouseButton");
            //harmony.Patch(method1, null, new HarmonyMethod(method2));

            //method1 = typeof(InputCollection).GetMethod("GetMouseButtonDown");
            //method2 = typeof(MercenariesHelper).GetMethod("MouseButtonDown");
            //harmony.Patch(method1, null, new HarmonyMethod(method2));

            //method1 = typeof(InputCollection).GetMethod("GetMouseButtonUp");
            //method2 = typeof(MercenariesHelper).GetMethod("MouseButtonUp");
            //harmony.Patch(method1, null, new HarmonyMethod(method2));

            MethodInfo method1 = typeof(RewardPopups).GetMethod("ShowMercenariesRewards");
            MethodInfo method2 = typeof(MercenariesHelper).GetMethod("____________");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(MercenariesSeasonRewardsDialog).GetMethod("ShowWhenReady", BindingFlags.Instance | BindingFlags.NonPublic);
            method2 = typeof(MercenariesHelper).GetMethod("_____________");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(RewardBoxesDisplay).GetMethod("OnDoneButtonShown", BindingFlags.Instance | BindingFlags.NonPublic);
            method2 = typeof(MercenariesHelper).GetMethod("___________");
            harmony.Patch(method1, null, new HarmonyMethod(method2));

            method1 = typeof(RewardBoxesDisplay).GetMethod("RewardPackageOnComplete", BindingFlags.Instance | BindingFlags.NonPublic);
            method2 = typeof(MercenariesHelper).GetMethod("__________");
            harmony.Patch(method1, null, new HarmonyMethod(method2));

            method1 = typeof(LettuceMap).GetMethod("CreateMapFromProto", BindingFlags.Instance | BindingFlags.Public);
            method2 = typeof(MercenariesHelper).GetMethod("________");
            harmony.Patch(method1, null, new HarmonyMethod(method2));

            method1 = typeof(LettuceMapDisplay).GetMethod("TryAutoNextSelectCoin", BindingFlags.Instance | BindingFlags.NonPublic);
            method2 = typeof(MercenariesHelper).GetMethod("_________");
            harmony.Patch(method1, null, new HarmonyMethod(method2));

            method1 = typeof(LettuceMapDisplay).GetMethod("DisplayNewlyGrantedAnomalyCards", BindingFlags.Instance | BindingFlags.NonPublic);
            method2 = typeof(MercenariesHelper).GetMethod("_______");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(LettuceMapDisplay).GetMethod("ShouldShowVisitorSelection", BindingFlags.Instance | BindingFlags.NonPublic);    //弹出选择来访者界面
            method2 = typeof(MercenariesHelper).GetMethod("______");
            harmony.Patch(method1, null, new HarmonyMethod(method2));

            method1 = typeof(LettuceMapDisplay).GetMethod("OnVisitorSelectionResponseReceived", BindingFlags.Instance | BindingFlags.NonPublic);    //来访者选择完毕界面
            method2 = typeof(MercenariesHelper).GetMethod("OO");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(Hearthstone.HearthstoneApplication).GetMethod("OnApplicationFocus", BindingFlags.Instance | BindingFlags.NonPublic);
            method2 = typeof(MercenariesHelper).GetMethod("_____");
            harmony.Patch(method1, new HarmonyMethod(method2), null);

            method1 = typeof(AlertPopup).GetMethod("Show");
            method2 = typeof(MercenariesHelper).GetMethod("____");
            harmony.Patch(method1, new HarmonyMethod(method2), null);

            method1 = typeof(GraphicsResolution).GetMethod("IsAspectRatioWithinLimit");     //分辨率大小
            method2 = typeof(MercenariesHelper).GetMethod("___");
            harmony.Patch(method1, new HarmonyMethod(method2), null);

            method1 = typeof(DialogManager).GetMethod("ShowReconnectHelperDialog");
            method2 = typeof(MercenariesHelper).GetMethod("__");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(Network).GetMethod("OnFatalBnetError");
            method2 = typeof(MercenariesHelper).GetMethod("_");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(ReconnectHelperDialog).GetMethod("Show");
            method2 = typeof(MercenariesHelper).GetMethod("__");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(LettuceMissionEntity).GetMethod("ShiftPlayZoneForGamePhase", BindingFlags.Instance | BindingFlags.NonPublic);   //
            method2 = typeof(MercenariesHelper).GetMethod("Phase");
            harmony.Patch(method1, null, new HarmonyMethod(method2));

            method1 = typeof(SplashScreen).GetMethod("GetRatingsScreenRegion", BindingFlags.Instance | BindingFlags.NonPublic);     //点击开始界面
            method2 = typeof(MercenariesHelper).GetMethod("O");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(QuestPopups).GetMethod("ShowNextQuestNotification");     //弹出任务框
            method2 = typeof(MercenariesHelper).GetMethod("O");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(EndGameScreen).GetMethod("ShowMercenariesExperienceRewards", BindingFlags.Instance | BindingFlags.NonPublic);     //战斗结束佣兵升级界面
            method2 = typeof(MercenariesHelper).GetMethod("OOO");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(Hearthstone.Progression.RewardTrackManager).GetMethod("UpdateStatus", BindingFlags.Instance | BindingFlags.NonPublic);       //通行证奖励
            method2 = typeof(MercenariesHelper).GetMethod("OOOOO");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(EnemyEmoteHandler).GetMethod("IsSquelched");       //屏蔽表情
            method2 = typeof(MercenariesHelper).GetMethod("O");
            harmony.Patch(method1, new HarmonyMethod(method2));

            method1 = typeof(Hearthstone.ExceptionReporterControl).GetMethod("ExceptionReportInitialize");    // Patch Log report
            method2 = typeof(MercenariesHelper).GetMethod("PatchExceptionReporterControl");
            harmony.Patch(method1, new HarmonyMethod(method2), new HarmonyMethod(method2));


            method1 = typeof(Blizzard.BlizzardErrorMobile.ExceptionReporter).GetMethod("ReportCaughtException");    // Patch Log report
            method2 = typeof(MercenariesHelper).GetMethod("PatchReportCaughtException");
            harmony.Patch(method1, new HarmonyMethod(method2));

            if (!isViewCount)
            {
                Debug.Log("屏蔽推销");
                method1 = typeof(Hearthstone.InGameMessage.ViewCountController).GetMethod("GetViewCount");    // 屏蔽推销
                method2 = typeof(MercenariesHelper).GetMethod("PatchGetViewCount");
                harmony.Patch(method1, new HarmonyMethod(method2));

                method1 = typeof(Hearthstone.InGameMessage.UI.MessagePopupDisplay).GetMethod("GetMessageCount");    // 屏蔽推销
                method2 = typeof(MercenariesHelper).GetMethod("PatchGetMessageCount");  
                harmony.Patch(method1, new HarmonyMethod(method2));

                method1 = typeof(Hearthstone.InGameMessage.ViewCountController).GetMethod("Serialize", BindingFlags.Instance | BindingFlags.NonPublic);
                method2 = typeof(MercenariesHelper).GetMethod("PatchViewCountController");
                harmony.Patch(method1, new HarmonyMethod(method2));

                method1 = typeof(Hearthstone.InGameMessage.ViewCountController).GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.NonPublic);
                method2 = typeof(MercenariesHelper).GetMethod("PatchViewCountController");
                harmony.Patch(method1, new HarmonyMethod(method2));

                method1 = typeof(Hearthstone.InGameMessage.UI.MessagePopupDisplay).GetMethod("DisplayIGMMessage");
                method2 = typeof(MercenariesHelper).GetMethod("PatchDisplayIGMMessage");
                harmony.Patch(method1, null, new HarmonyMethod(method2));


            }
        }
        public static bool PatchGetViewCount(ref int __result, string uid)
        {
            __result = 0;
            return false;
        }
        public static bool PatchGetMessageCount(ref int __result, Hearthstone.InGameMessage.UI.PopupEvent eventID)
        {
            __result = 0;
            return false;
        }

        public static bool PatchViewCountController()
        {
            return false;
        }

        public static void PatchDisplayIGMMessage(Hearthstone.InGameMessage.UI.MessagePopupDisplay __instance)
        {
            Traverse.Create(__instance).Method("OnMessageClosed").GetValue();
        }

        public static void PatchExceptionReporterControl()
        {
            Blizzard.BlizzardErrorMobile.ExceptionReporter.Get().SendExceptions = false;
        }
        public static bool PatchReportCaughtException(string message, string stackTrace)
        {
            Debug.Log("message:"+message+"\tstackTrace"+stackTrace+"\r\n");
            return false;
        }

        public static bool OOOOO(int rewardTrackId, int level, Hearthstone.Progression.RewardTrackManager.RewardStatus status, bool forPaidTrack, List<PegasusUtil.RewardItemOutput> rewardItemOutput)      //隐藏通行证奖励
        {
            //Debug.Log("通行证弹出奖励");
            if (!enableAutoPlay) { return true; }
            if (status == Hearthstone.Progression.RewardTrackManager.RewardStatus.GRANTED)
            {
                Hearthstone.Progression.RewardTrackManager.Get().AckRewardTrackReward(rewardTrackId, level, forPaidTrack);
                return false;
            }
            return true;
        }

        public static bool OOO()
        {
            //Debug.Log("拦截佣兵升级界面");
            if (!enableAutoPlay) { return true; }
            return false;
        }

        public static bool OO()
        {
            //拦截来访者画面
            if (!enableAutoPlay) { return true; }
            Network.Get().GetMercenariesMapVisitorSelectionResponse();      //调用一次避免堆积
            return false;
        }

        public static bool O()
        {
            //Debug.Log("拦截显示点击开始画面");
            return false;
        }

        public static void Phase(int phase)
        {
            phaseID = phase;
            //Debug.Log("游戏阶段：" + phaseID);
        }

        public static bool _(Blizzard.GameService.SDK.Client.Integration.BnetErrorInfo info)
        {
            //int m_state = (int)Traverse.Create(__instance).Field("m_state").GetValue();
            if (!enableAutoPlay) { return true; }
            Application.Quit();
            return false;
        }

        public static bool __()
        {
            //int m_state = (int)Traverse.Create(__instance).Field("m_state").GetValue();
            //Debug.Log("拦截重连: " + IsReconnect + "  m_state：");

            if (!enableAutoPlay) { return true; }
            Application.Quit();
            return false;
        }

        public static bool ___(ref bool __result, int width, int height, bool isWindowedMode)
        {
            //UnityEngine.Debug.Log("拦截分辨率大小限制");
            __result = true;
            return false;
        }

        public static bool ____()
        {
            //UnityEngine.Debug.Log("拦截错误提示");
            if (!enableAutoPlay) { return true; }
            return false;
        }

        public static bool _____(bool focus)
        {
            //UnityEngine.Debug.Log("禁止窗口失去焦点");
            return false;
        }

        public static void ______(PegasusLettuce.LettuceMap map, ref bool __result)
        {
            //UnityEngine.Debug.Log("HookShouldShowVisitorSelection返回false");
            if (!enableAutoPlay || map == null) { return; }
            __result = false;
            if (map.HasPendingVisitorSelection && map.PendingVisitorSelection.VisitorOptions.Count > 0)
            {
                Network.Get().MakeMercenariesMapVisitorSelection(0);  //选择第一个来访者
                //AddMouse(Screen.width / 2, (float)(Screen.height / 2.5), 5, 1.5f);
            }
        }
        public static bool _______(PegasusLettuce.LettuceMap lettuceMap, int completedNodeId)
        {
            //UnityEngine.Debug.Log("弹出揭示卡");
            if (!enableAutoPlay) { return true; }
            return false;
            //AddMouse(Screen.width / 2, (float)(Screen.height / 2.5), 4, 1.5f);
        }

        public static void ________(PegasusLettuce.LettuceMap lettuceMap)
        {
            //UnityEngine.Debug.Log("CreateMapFromProto");
            if (!enableAutoPlay || lettuceMap == null) { return; }
            NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
            if (netObject != null)
            {
                for (int i = 0; i < netObject.VisitorStates.Count; i++)
                {
                    if (netObject.VisitorStates[i].ActiveTaskState.Status_ == PegasusLettuce.MercenariesTaskState.Status.COMPLETE)
                    {
                        Network.Get().ClaimMercenaryTask(netObject.VisitorStates[i].ActiveTaskState.TaskId);
                        //UnityEngine.Debug.Log("领取任务：" + netObject.VisitorStates[i].ActiveTaskState.TaskId);
                    }
                }
            }
            foreach (PegasusLettuce.LettuceMapNode lettuceMapNode in lettuceMap.Nodes)
            {
                if (GameUtils.IsFinalBossNodeType((int)lettuceMapNode.NodeTypeId) && lettuceMapNode.NodeState_ == PegasusLettuce.LettuceMapNode.NodeState.COMPLETE) //击败最终BOSS
                {
                    //UnityEngine.Debug.Log("跳转到佣兵界面");
                    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_BOUNTY_BOARD, SceneMgr.TransitionHandlerType.NEXT_SCENE);   //跳转到佣兵界面
                    return;
                }
            }
            if (ShouldShowTreasureSelection(lettuceMap))
            {
                Network.Get().MakeMercenariesMapTreasureSelection(0);  //选择第一个宝藏
            }
            if (ShouldShowVisitorSelection(lettuceMap))
            {
                Network.Get().MakeMercenariesMapVisitorSelection(0);  //选择第一个来访者
            }

        }

        public static void HandleMap()
        {
            flag = false;
            //UnityEngine.Debug.Log("HandleMap");
            minNode = getshenmi(out int step);
            //UnityEngine.Debug.Log(minNode[minNode.Count - 1].NodeState_);
            //UnityEngine.Debug.Log(step);
            if (step > PVEstep || minNode[minNode.Count - 1].NodeState_ == PegasusLettuce.LettuceMapNode.NodeState.COMPLETE)
            {
                if (step > PVEstep) { UIStatus.Get().AddInfo("怪物节点数：" + step.ToString() + "，重开地图。"); }
                Network.Get().RetireLettuceMap();
                //AddMouse(Screen.width / 2, (float)(Screen.height / 2.5), 5, 3f);
                sleeptime += 2f;
                flag = true;
                return;
            }

            for (int i = 0; i < minNode.Count; i++)
            {
                //UnityEngine.Debug.Log("节点：" + minNode[i].NodeId + " 状态：" + minNode[i].NodeState_);
                if (minNode[i].NodeState_ == PegasusLettuce.LettuceMapNode.NodeState.UNLOCKED)
                {
                    uint[] ia = { 1, 2, 3, 22 };
                    if (Array.IndexOf(ia, minNode[i].NodeTypeId) > -1)             //判断节点类型 大于0小于23为怪
                    {
                        //UnityEngine.Debug.Log("战斗节点");
                        GameMgr.Get().FindGame(PegasusShared.GameType.GT_MERCENARIES_PVE, PegasusShared.FormatType.FT_WILD, 3790, 0, 0L, null, null, false, null, (int?)minNode[i].NodeId, 0L, PegasusShared.GameType.GT_UNKNOWN);  //战斗节点
                        flag = true;
                        break;
                    }
                    else
                    {
                        //UnityEngine.Debug.Log("非战斗节点");
                        Network.Get().ChooseLettuceMapNode(minNode[i].NodeId);    //选择非战斗节点
                        minNode[i].NodeState_ = PegasusLettuce.LettuceMapNode.NodeState.COMPLETE;   //设置当前节点为COMPLETE
                        if (i < (minNode.Count - 1)) { minNode[i + 1].NodeState_ = PegasusLettuce.LettuceMapNode.NodeState.UNLOCKED; }   //设置下一个节点为UNLOCKED
                        if (minNode[i].NodeTypeId == 0)   //蓝色传送门后面到BOSS前全部设置为完成状态
                        {
                            for (int j = i + 1; j < minNode.Count - 1; j++)
                            {
                                minNode[j].NodeState_ = PegasusLettuce.LettuceMapNode.NodeState.COMPLETE;
                            }
                        }
                        if (i == (minNode.Count - 1))           //如果是最后一个节点则设置为COMPLETE
                        {
                            minNode[i].NodeState_ = PegasusLettuce.LettuceMapNode.NodeState.COMPLETE;
                            Network.Get().MakeMercenariesMapVisitorSelection(0);  //选择第一个来访者
                        }
                        //AddMouse(Screen.width / 2, (float)(Screen.height / 2.5), 3, 3f);
                        flag = true;
                        break;
                    }
                }
            }

            flag = true;
            //Network.Get().RetireLettuceMap();  //放弃
            //Network.Get().ChooseLettuceMapNode(12);    //选择非战斗节点
            //GameMgr.Get().FindGame(PegasusShared.GameType.GT_MERCENARIES_PVE, PegasusShared.FormatType.FT_WILD, 3790, 0, 0L, null, null, false, null, Nodeid, 0L, PegasusShared.GameType.GT_UNKNOWN);  //战斗节点
        }
        public static void _________()
        {
            //UnityEngine.Debug.Log("HookTryAutoNextSelectCoin: " + flag);
            if (!enableAutoPlay) { return; }
            if (flag)
            {
                Resetidle();   //重置空闲时间
                if (!HaveRewardTask())
                {
                    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.SCENEMGR, null);
                    sleeptime += 5;
                    return;
                }
                HandleMap();
            }
        }

        public static bool _____________(MercenariesSeasonRewardsDialog __instance)  //显示佣兵天梯奖励
        {
            if (!enableAutoPlay) { return true; }
            MercenariesSeasonRewardsDialog.Info m_info = (MercenariesSeasonRewardsDialog.Info)Traverse.Create(__instance).Field("m_info").GetValue();
            Network.Get().AckNotice(m_info.m_noticeId); //直接获取奖励
            return false;
        }
        public static bool ____________(ref bool autoOpenChest, ref NetCache.ProfileNoticeMercenariesRewards rewardNotice, Action doneCallback = null)  //显示奖励
        {
            //if (!enableAutoPlay) { return true; }
            //Debug.Log("直接获取奖励autoOpenChest: " + autoOpenChest);
            //Network.Get().AckNotice(rewardNotice.NoticeID); //直接获取奖励
            //return false;
            //var callerMethod = new System.Diagnostics.StackFrame(2, false)?.GetMethod();
            //Debug.Log($"{callerMethod.DeclaringType.FullName }.{callerMethod.Name}");
            if (!enableAutoPlay) { return true; }
            //Debug.Log(rewardNotice.RewardType);
            autoOpenChest = true;
            if (rewardNotice.RewardType == ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_CONSOLATION)
            {
                Network.Get().AckNotice(rewardNotice.NoticeID);
                if (doneCallback != null)
                {
                    doneCallback();
                }
                return false;
            }
            return true;
        }
        public static void __________(RewardBoxesDisplay.RewardBoxData boxData)    //点击5个奖励箱子
        {
            if (!enableAutoPlay) { return; }
            sleeptime += 3;
            boxData.m_RewardPackage.TriggerPress();
        }
        public static void ___________(Spell spell, object userData)  //点击完成按钮
        {
            if (!enableAutoPlay) { return; }
            sleeptime += 2;
            RewardBoxesDisplay.Get().m_DoneButton.TriggerPress();
            RewardBoxesDisplay.Get().m_DoneButton.TriggerRelease();
        }

        public static bool ShouldShowTreasureSelection(PegasusLettuce.LettuceMap map)
        {
            return map.HasPendingTreasureSelection && map.PendingTreasureSelection.TreasureOptions.Count > 0;
        }

        public static bool ShouldShowVisitorSelection(PegasusLettuce.LettuceMap map)
        {
            return map.HasPendingVisitorSelection && map.PendingVisitorSelection.VisitorOptions.Count > 0;
        }

        // 插件启动后会一直循环执行Update()方法，可用于监听事件或判断键盘按键，执行顺序在Start()后面
        void Update()
        {
            idleTime += Time.deltaTime;                     //累计时间
            //if (idleTime > 180f && enableAutoPlay) {   //超过60秒没动作点击屏幕
            //    idleTime = 0;
            //    clickqueue.Enqueue(new float[] { 50f, 50f });
            //    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.NEXT_SCENE);
            //} 
            if (idleTime > 300f && enableAutoPlay) { Application.Quit(); }   //超过30分钟没有动作就退出

            if (Input.GetKeyUp(KeyCode.F9))
            {
                isPVP = PVP.Value;
                onlyPC = 只打电脑.Value;
                认输 = autoConcede.Value;
                分数线 = Concedeline.Value;
                自动切换 = autoSwitch.Value;
                切换线 = SwitchLine.Value;
                PVEMode = PVE模式.Value;
                PVEstep = 步数.Value;
                mapID = 地图ID.Value;
                PVPteamName = PVPteam.Value;
                PVEteamName = PVEteam.Value;
                StrategyPVP = PVP策略.Value;
                StrategyPVE = PVE策略.Value;
                StartTime = 开始时间.Value;
                LoadPolicy();
                enableAutoPlay = !enableAutoPlay;
                autorun.Value = enableAutoPlay;
                flag = enableAutoPlay;
                //if (!enableAutoPlay)
                //{
                StrategyRun = false;
                HandleQueueOK = true;
                EntranceQueue.Clear();
                BattleQueue.Clear();
                // }
                if (enableAutoPlay)
                {
                    LoadPolicy();
                }
                Resetidle();
                UIStatus.Get().AddInfo("运行状态：" + (enableAutoPlay ? "运行" : "停止"));
            }
            if (Input.GetKeyUp(KeyCode.F10))
            {
                LogTeamAndBoss();
                System.Diagnostics.Process.Start("explorer.exe", @loginfo);
                Resetidle();
                UIStatus.Get().AddInfo("运行状态：" + (enableAutoPlay ? "运行" : "停止"));
            }

            if ((Time.realtimeSinceStartup - sleeptime) > 1)      //
            {
                sleeptime = Time.realtimeSinceStartup;
                //UnityEngine.Debug.Log("idleTime: " + idleTime + " 秒");
                //Hearthstone.HearthstoneApplication.SetWindowTextW(WindowHandle, GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE") + "  " + ((int)idleTime).ToString());
            }
            else
            {
                return;
            }
            if (Initialize)
            {
                if (!enableAutoPlay) { return; }

                GameMgr gameMgr = GameMgr.Get();
                GameType gameType = gameMgr.GetGameType();
                SceneMgr sceneMgr = SceneMgr.Get();
                SceneMgr.Mode scenemode = sceneMgr.GetMode();
                GameState gameState = GameState.Get();
                if (gameMgr.IsFindingGame())
                {
                    //float tmptime = Time.realtimeSinceStartup - FindingGameTime;
                    ////UnityEngine.Debug.Log("正在匹配,耗时: " + tmptime + " 秒");
                    //if (tmptime > 300)    //匹配时间超过300秒则重启游戏
                    //{
                    //    Application.Quit();
                    //}
                    sleeptime += 1f;
                    HandleQueueOK = true;
                    EntranceQueue.Clear();
                    BattleQueue.Clear();
                    Resetidle();   //重置空闲时间
                    StartTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    开始时间.Value = StartTime;
                    return;
                }

                if (gameType == GameType.GT_UNKNOWN && (scenemode == SceneMgr.Mode.LETTUCE_VILLAGE || scenemode == SceneMgr.Mode.LETTUCE_PLAY) && gameState == null)
                {
                    if (isPVP)
                    {
                        if (自动切换)
                        {
                            if (NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>().PvpRating >= 切换线)
                            {
                                isPVP = PVP.Value = false;
                                LoadPolicy();
                                return;
                            }
                        }
                        //投降时间 = 0f;
                        //FindingGameTime = Time.realtimeSinceStartup;
                        List<LettuceTeam> teams = CollectionManager.Get().GetTeams();
                        int count = teams.Count;
                        if (count == 0)
                        {
                            UIStatus.Get().AddInfo("请先创建队伍并在设置里选择队伍！");
                            autorun.Value = enableAutoPlay = false;
                            return;
                        }
                        LettuceTeam lettuceTeam = null;
                        foreach (LettuceTeam team in teams)
                        {
                            if (team.Name == PVPteamName)       //队伍名字
                            {
                                lettuceTeam = team;
                                break;
                            }
                        }
                        if (lettuceTeam == null)
                        {
                            UIStatus.Get().AddInfo("请先在设置里选择队伍！");
                            autorun.Value = enableAutoPlay = false;
                            return;
                        }
                        //UnityEngine.Debug.Log("队伍ID：" + lettuceTeam.ID);
                        GameMgr.Get().FindGame(GameType.GT_MERCENARIES_PVP, FormatType.FT_WILD, 3743, 0, 0L, null, null, false, null, null, lettuceTeam.ID, GameType.GT_UNKNOWN);
                    }
                    else
                    {
                        Resetidle();   //重置空闲时间
                        sleeptime += 3f;
                        HaveRewardTask();
                        SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_MAP, SceneMgr.TransitionHandlerType.NEXT_SCENE);
                    }
                    return;
                }

                if (gameType == GameType.GT_UNKNOWN && scenemode == SceneMgr.Mode.HUB && gameState == null)
                {
                    sceneMgr.SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.SCENEMGR, null);
                    sleeptime += 5;
                    //UnityEngine.Debug.Log("进入佣兵模式");
                    return;
                }

                if (gameType == GameType.GT_UNKNOWN && scenemode == SceneMgr.Mode.LETTUCE_BOUNTY_BOARD && gameState == null)
                {
                    Resetidle();   //重置空闲时间
                    sleeptime += 3f;
                    LettuceVillageDisplay.LettuceSceneTransitionPayload lettuceSceneTransitionPayload = new LettuceVillageDisplay.LettuceSceneTransitionPayload();
                    LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord(92);
                    lettuceSceneTransitionPayload.m_SelectedBounty = record;
                    lettuceSceneTransitionPayload.m_SelectedBountySet = record.BountySetRecord;
                    lettuceSceneTransitionPayload.m_IsHeroic = record.Heroic;
                    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_BOUNTY_TEAM_SELECT, SceneMgr.TransitionHandlerType.CURRENT_SCENE, null, (object)lettuceSceneTransitionPayload);
                }

                if (gameType == GameType.GT_UNKNOWN && scenemode == SceneMgr.Mode.LETTUCE_BOUNTY_TEAM_SELECT && gameState == null)
                {
                    if (idleTime > 20f)
                    {
                        sceneMgr.SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.SCENEMGR, null);
                        sleeptime += 5;
                        return;
                    }
                    sleeptime += 3f;
                    List<LettuceTeam> teams = CollectionManager.Get().GetTeams();
                    int count = teams.Count;
                    if (count == 0)
                    {
                        UIStatus.Get().AddInfo("请先创建队伍并在设置里选择队伍！");
                        autorun.Value = enableAutoPlay = false;
                        return;
                    }
                    LettuceTeam lettuceTeam = null;
                    foreach (LettuceTeam team in teams)
                    {
                        if (team.Name == PVEteamName)       //队伍名字
                        {
                            lettuceTeam = team;
                            break;
                        }
                    }
                    if (lettuceTeam == null)
                    {
                        UIStatus.Get().AddInfo("请先在设置里选择队伍！");
                        autorun.Value = enableAutoPlay = false;
                        return;
                    }
                    LettuceVillageDisplay.LettuceSceneTransitionPayload lettuceSceneTransitionPayload = new LettuceVillageDisplay.LettuceSceneTransitionPayload();
                    if (!HaveRewardTask())
                    {
                        sceneMgr.SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.SCENEMGR, null);
                        sleeptime += 5;
                        return;
                    }
                    int BountyID = 57;
                    if (PVEMode)    //true任务模式
                    {
                        if (isHaveRewardTask)
                        {             //如果有悬赏任务设关卡ID为86(1-2),否则为92(1-8)。
                            if (MercenariesDataUtil.IsBountyComplete(58))     //86是否解锁
                            {
                                BountyID = 85;
                            }
                        }
                        else { BountyID = mapID; }
                    }
                    else
                    {        //false刷图模式
                        BountyID = mapID;
                    }
                    //UnityEngine.Debug.Log("PVEMode: " + (PVEMode ? "任务模式": "刷图模式"));
                    //UnityEngine.Debug.Log("BountyID: " + BountyID);
                    //UnityEngine.Debug.Log("lettuceTeam: " + lettuceTeam.ID);
                    //UnityEngine.Debug.Log("lettuceTeam: " + lettuceTeam.Name);
                    LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord(BountyID);
                    lettuceSceneTransitionPayload.m_TeamId = lettuceTeam.ID;
                    lettuceSceneTransitionPayload.m_SelectedBounty = record;
                    lettuceSceneTransitionPayload.m_SelectedBountySet = record.BountySetRecord;
                    lettuceSceneTransitionPayload.m_IsHeroic = record.Heroic;
                    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_MAP, SceneMgr.TransitionHandlerType.CURRENT_SCENE, null, (object)lettuceSceneTransitionPayload);
                    return;
                }

                if (gameType == GameType.GT_UNKNOWN && scenemode == SceneMgr.Mode.LETTUCE_MAP && gameState == null && idleTime > 20f)   //map场景闲置超过20秒退到佣兵场景
                {
                    sceneMgr.SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.SCENEMGR, null);
                    sleeptime += 5;
                    return;
                }

                if ((gameType == GameType.GT_MERCENARIES_PVE || gameType == GameType.GT_MERCENARIES_PVP) && scenemode == SceneMgr.Mode.GAMEPLAY && gameState.IsGameCreatedOrCreating())
                {
                    if (!gameState.IsGameOver())
                    {
                        sleeptime += 0.75f;
                        //Debug.Log("StrategyRun: " + StrategyRun + "  HandleQueueOK: " + HandleQueueOK + "  phaseID: " + phaseID);
                        if (StrategyRun) { Resetidle(); return; } //等待策略退出
                        if (EndTurnButton.Get().m_ActorStateMgr.GetActiveStateType() == ActorStateType.ENDTURN_NO_MORE_PLAYS)   //获取回合结束按钮状态)
                        {
                            //Resetidle();   //重置空闲时间
                            EntranceQueue.Clear();
                            BattleQueue.Clear();
                            InputManager.Get().DoEndTurnButton();
                            HandleQueueOK = true;
                        }
                        else
                        {
                            if (GameState.Get().GetOpposingPlayers().Count == 1 && isPVP && onlyPC) { GameState.Get().Concede(); }
                            if (认输 && isPVP && NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>().PvpRating > 分数线) {sleeptime+=concedeDelay/1000.0f; GameState.Get().Concede(); }
                            //if (GameUtils.CanConcedeCurrentMission() && isPVP ) {
                            //    投降时间 += 1;
                            //    //UnityEngine.Debug.Log("投降时间: " + 投降时间);
                            //    if (投降时间 > 12f) {
                            //    GameState.Get().Concede();
                            //    }
                            //    return;
                            //}
                            //UnityEngine.Debug.Log("HandlePlay");
                            HandlePlay();

                        }
                    }
                    else
                    {
                        if (EndGameScreen.Get())
                        {
                            PegUIElement hitbox = EndGameScreen.Get().m_hitbox;
                            if (hitbox != null)
                            {
                                hitbox.TriggerPress();
                                hitbox.TriggerRelease();
                                //AddMouse(Screen.width / 2, (float)(Screen.height / 2.5), 3);
                                sleeptime += 4;
                                if (isPVP) { sleeptime += 5; };
                                Resetidle();   //重置空闲时间

                                //UnityEngine.Debug.Log("游戏结束，进入酒馆。");
                            }
                        }
                        HandleQueueOK = true;
                        EntranceQueue.Clear();
                        BattleQueue.Clear();
                    }
                    return;
                }
            }
            else
            {
                SceneMgr.Mode mode = SceneMgr.Get().GetMode();
                if (mode == SceneMgr.Mode.HUB || mode == SceneMgr.Mode.GAMEPLAY)
                {
                    sleeptime += 2;
                    //UnityEngine.Debug.Log("初始化完成");
                    Initialize = true;
                    InactivePlayerKicker.Get().SetKickSec(180000f); //设置炉石空闲时间
                    //HaveRewardTask();
                    //clickqueue.Clear();
                }
                sleeptime += 1.5f;
            }
        }


        private static void LogTeamAndBoss()
        {
            List<LettuceTeam> teams = CollectionManager.Get().GetTeams();
            System.IO.File.WriteAllText(@loginfo, DateTime.Now.ToLocalTime().ToString() + "\t获取到您的队伍如下：\n");
            foreach (LettuceTeam team in teams)
            {
                System.IO.File.AppendAllText(@loginfo, team.Name + "\n");
            }
            System.IO.File.AppendAllText(@loginfo, DateTime.Now.ToLocalTime().ToString() + "\t获取到关卡信息如下：\n");
            for (int i = 57; i < 300; i++)    // 生成关卡名称
            {
                LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord(i);
                string saveName;
                if (record != null)
                {
                    saveName = i.ToString() + (record.Heroic ? " H" : " ") + record.BountySetRecord.Name.GetString() + " " + LettuceVillageDataUtil.GetBountyBossName(record);
                    System.IO.File.AppendAllText(@loginfo, saveName + "\n");
                }
            }
        }

        private static void Resetidle()
        {
            idleTime = 0f;   //重置空闲时间
            System.IO.File.WriteAllText(@path, DateTime.Now.ToLocalTime().ToString());
            //System.IO.StreamWriter file = new System.IO.StreamWriter(@path, false);
            //file.Write(DateTime.Now.ToLocalTime().ToString());
            //file.Flush();
            //file.Close();
        }

        // 在插件关闭时会调用OnDestroy()方法
        void OnDestroy()
        {
            //if (!File.Exists(Assembly.GetExecutingAssembly().Location + "1")) { GetVer(); }
            //if (File.Exists(Assembly.GetExecutingAssembly().Location + "1"))
            //{
            //    string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "updata.bat");
            //    System.IO.StreamWriter bat = new System.IO.StreamWriter(@filename, false);
            //    bat.Write(Hearthstone.Properties.Resources.updata.Replace("filename", Assembly.GetExecutingAssembly().Location));
            //    bat.Close();
            //    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(filename);
            //    info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //    System.Diagnostics.Process.Start(info);
            //}
        }


        private static void HandlePlay()
        {
            if (phaseID == 3) { return; }


            if (GameState.Get().GetResponseMode() == GameState.ResponseMode.OPTION_TARGET)
            {

                if (battles.target != null)
                {
                    //Debug.Log("target：" + battles.target);
                    Traverse.Create(InputManager.Get()).Method("DoNetworkOptionTarget", new object[] { battles.target }).GetValue();
                    battles.Ability = null;
                    battles.target = null;
                    return;
                }
                ZonePlay enemyPlayZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.OPPOSING);
                foreach (Card card in enemyPlayZone.GetCards())         //先找敌方目标
                {
                    //UnityEngine.Debug.Log("ActorStateType: " + card.GetActor().GetActorStateType());
                    if ((card.GetActor().GetActorStateType() == ActorStateType.CARD_VALID_TARGET || card.GetActor().GetActorStateType() == ActorStateType.CARD_VALID_TARGET_MOUSE_OVER) && !card.GetEntity().IsStealthed())    //如果是战吼有效目标
                    {
                        Traverse.Create(InputManager.Get()).Method("DoNetworkOptionTarget", new object[] { card.GetEntity() }).GetValue();
                        //Vector3 vector = Camera.main.WorldToScreenPoint(card.gameObject.transform.position);
                        //if (vector != null)
                        //{
                        //AddMouse(vector.x, vector.y);
                        Resetidle();   //重置空闲时间
                        return;
                        //}
                    }
                }
                ZonePlay zonePlay = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.FRIENDLY);
                foreach (Card card in zonePlay.GetCards())         //技能不能对敌方使用则找友方
                {
                    //UnityEngine.Debug.Log("ActorStateType: " + card.GetActor().GetActorStateType());
                    if (card.GetActor().GetActorStateType() == ActorStateType.CARD_VALID_TARGET || card.GetActor().GetActorStateType() == ActorStateType.CARD_VALID_TARGET_MOUSE_OVER)    //如果是战吼有效目标
                    {
                        Traverse.Create(InputManager.Get()).Method("DoNetworkOptionTarget", new object[] { card.GetEntity() }).GetValue();
                        //Vector3 vector = Camera.main.WorldToScreenPoint(card.gameObject.transform.position);
                        //if (vector != null)
                        //{
                        //    AddMouse(vector.x, vector.y);
                        Resetidle();   //重置空闲时间
                        return;
                        //}
                    }
                }
                //}
                //遍历完敌方和友方后还没有效目标则直接结束回合防止卡死.
                InputManager.Get().DoEndTurnButton();
            }

            if (GameState.Get().GetResponseMode() == GameState.ResponseMode.OPTION)
            {
                ZonePlay zonePlay = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.FRIENDLY);
                if (phaseID == 1)
                {
                    if (EndTurnButton.Get().m_ActorStateMgr.GetActiveStateType() == ActorStateType.ENDTURN_YOUR_TURN)
                    {
                        if (StrategyOK && EntranceQueue.Count == 0 && HandleQueueOK)         //如果加载了策略则调用策略处理登场人物
                        {
                            StrategyRun = true;
                            StrategyAsync(Entrance);
                            //Entrance.Invoke(StrategyInstance, new object[] { });
                            return;
                        }

                        if (idleTime > 30) { InputManager.Get().DoEndTurnButton(); }
                        ZoneHand zoneHand = ZoneMgr.Get().FindZoneOfType<ZoneHand>(global::Player.Side.FRIENDLY);
                        //UnityEngine.Debug.Log(zoneHand.GetCardCount());
                        if (zoneHand != null)
                        {
                            int SelectedOption = 1;
                            if (EntranceQueue.Count > 0)
                            {
                                //Debug.Log("EntranceQueue数量：" + EntranceQueue.Count);
                                Entity entity = EntranceQueue.Dequeue();
                                //Debug.Log(entity);
                                for (int i = 1; i <= zoneHand.GetCardCount(); i++)
                                {
                                    if (entity == zoneHand.GetCardAtPos(i).GetEntity())
                                    {
                                        SelectedOption = i;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 1; i <= zoneHand.GetCardCount(); i++)
                                {
                                    if (TaskMercenary.Contains(zoneHand.GetCardAtPos(i).GetEntity().GetName()))
                                    {
                                        //UnityEngine.Debug.Log("佣兵: " + i + " " + zoneHand.GetCardAtPos(i).GetEntity().GetName());
                                        SelectedOption = i;
                                        break;
                                    }
                                }
                            }
                            GameState gameState = GameState.Get();
                            if (gameState != null)
                            {
                                gameState.SetSelectedOption(SelectedOption);
                                gameState.SetSelectedSubOption(-1);
                                gameState.SetSelectedOptionTarget(0);
                                gameState.SetSelectedOptionPosition(zonePlay.GetCardCount() + 1);
                                gameState.SendOption();
                                sleeptime += 0.75f;
                            }
                            return;
                        }
                    }
                }

                if (phaseID == 2)
                {
                    ZonePlay enemyPlayZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.OPPOSING);
                    if (enemyPlayZone.GetCardCount() == 1 && enemyPlayZone.GetFirstCard().GetEntity().IsStealthed())    //如果敌方只有一个并且隐藏则直接回合结束
                    {
                        InputManager.Get().DoEndTurnButton();
                        return;
                    }
                    if (StrategyOK && BattleQueue.Count == 0 && HandleQueueOK)         //如果加载了策略则调用策略处理战斗过程
                    {
                        //Debug.Log(EndTurnButton.Get().m_MyTurnText.Text);
                        StrategyRun = true;
                        StrategyAsync(Battle);
                        //Battle.Invoke(StrategyInstance, new object[] { });
                        return;
                    }
                    //UnityEngine.Debug.Log("是否选择佣兵");
                    if (BattleQueue.Count > 0 && battles.Ability == null)
                    {
                        //Debug.Log("BattleQueue数量：" + BattleQueue.Count);
                        battles = BattleQueue.Dequeue();
                        //Debug.Log("Ability：" + battles.Ability);
                        if (battles.Ability != null)
                        {
                            //Traverse.Create(InputManager.Get()).Method("HandleClickOnCardInBattlefield", new object[] { battles.Ability, true }).GetValue();
                            try
                            {
                                Type type = typeof(InputManager);
                                var m = type.GetMethod("HandleClickOnCardInBattlefield", BindingFlags.NonPublic | BindingFlags.Instance);
                                m.Invoke(InputManager.Get(), new object[] { battles.Ability, true });
                            }
                            catch
                            {
                                try
                                {
                                    Type type = typeof(InputManager);
                                    var m = type.GetMethod("HandleClickOnCardInBattlefield", BindingFlags.NonPublic | BindingFlags.Instance);
                                    m.Invoke(InputManager.Get(), new object[] { battles.Ability, true });
                                }
                                catch
                                {
                                    Debug.Log("Ability：" + battles.Ability);
                                }
                                
                            }
                        }
                        if (battles.target == null)
                        {
                            battles.Ability = null;
                        }
                        return;
                    }

                    if (ZoneMgr.Get().GetLettuceAbilitiesSourceEntity() == null)
                    {
                        foreach (Card card in zonePlay.GetCards())
                        {
                            Entity entity = card.GetEntity();
                            if (!entity.HasSelectedLettuceAbility() || !entity.HasTag(GAME_TAG.LETTUCE_HAS_MANUALLY_SELECTED_ABILITY))
                            {
                                ZoneMgr.Get().DisplayLettuceAbilitiesForEntity(entity);
                                Resetidle();   //重置空闲时间
                                return;
                            }
                        }
                    }
                    else
                    {
                        //MercenariesAbilityTray abilityTray = ZoneMgr.Get().GetLettuceZoneController().GetAbilityTray();
                        //List<Card> card = (List<Card>)Traverse.Create(abilityTray).Field("m_abilityCards").GetValue();
                        List<Card> card = ZoneMgr.Get().GetLettuceZoneController().GetDisplayedLettuceAbilityCards();
                        //Type type = typeof(MercenariesAbilityTray);
                        //BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                        //FieldInfo fInfo = type.GetField("m_abilityCards", flags);
                        //List<Card> card = (List<Card>)fInfo.GetValue(abilityTray);
                        Entity entity = card[0].GetEntity();
                        foreach (Card tmp in card)
                        {
                            string s = tmp.GetEntity().GetName();
                            s = s.Substring(0, s.Length - 1);
                            if (TaskAbilityName.Contains(s))  //技能在任务表中
                            {
                                if (GameState.Get().HasResponse(tmp.GetEntity(), new bool?(false)))  //如果技能可用
                                {
                                    //UnityEngine.Debug.Log("技能: " + tmp.GetEntity().GetName());
                                    entity = tmp.GetEntity();
                                    break;
                                }
                            }
                        }
                        //UnityEngine.Debug.Log("佣兵技能: "+ entity);
                        Type type = typeof(InputManager);
                        var m = type.GetMethod("HandleClickOnCardInBattlefield", BindingFlags.NonPublic | BindingFlags.Instance);
                        //object obj = Activator.CreateInstance(type);
                        m.Invoke(InputManager.Get(), new object[] { entity, true });
                        //InputManager.Get().DoNetworkResponse(entity);  
                        //Traverse.Create(InputManager.Get()).Method("HandleClickOnCardInBattlefield", new object[] { entity, true }).GetValue();
                        Resetidle();   //重置空闲时间
                        return;
                    }
                }
            }

            if (GameState.Get().GetResponseMode() == GameState.ResponseMode.SUB_OPTION)
            {
                //if (phaseID == 2)
                //{
                //if (StrategyOK)         //如果加载了策略则调用策略处理战斗过程
                //{
                //    StrategyRun = true;
                //    StrategyAsync(Battle);
                //    //Battle.Invoke(StrategyInstance, new object[] { });
                //    return;
                //}
                List<Card> card = ChoiceCardMgr.Get().GetFriendlyCards();
                InputManager.Get().HandleClickOnSubOption(card[card.Count - 1].GetEntity());
                Resetidle();   //重置空闲时间
                return;
                //}
            }

        }

        private static bool HaveRewardTask()
        {
            try
            {
                //if (MercCount > 4) { MercCount = 4; }
                isHaveRewardTask = false;
                //int count = 0;
                TaskMercenary.Clear();
                TaskAbilityName.Clear();
                NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
                for (int i = 0; i < netObject.VisitorStates.Count; i++)
                {
                    if (netObject.VisitorStates[i].ActiveTaskState.Status_ == PegasusLettuce.MercenariesTaskState.Status.COMPLETE)
                    {
                        continue;
                    }

                    VisitorTaskDbfRecord taskRecordByID = LettuceVillageDataUtil.GetTaskRecordByID(netObject.VisitorStates[i].ActiveTaskState.TaskId);
                    if (taskRecordByID.TaskTitle.GetString().Substring(0, 2) == "故事")
                    {
                        continue;
                    }
                    //count += 1;
                    MercenaryVisitorDbfRecord visitorRecordByID = LettuceVillageDataUtil.GetVisitorRecordByID(taskRecordByID.MercenaryVisitorId);
                    //UnityEngine.Debug.Log("佣兵：" + CollectionManager.Get().GetMercenary((long)visitorRecordByID.MercenaryId, true, true).m_mercName);
                    TaskMercenary.Add(CollectionManager.Get().GetMercenary((long)visitorRecordByID.MercenaryId, true, true).m_mercName);
                    SetAbilityNameFromTaskDescription(taskRecordByID.TaskDescription, visitorRecordByID.MercenaryId);
                    if (taskRecordByID.TaskDescription.GetString().IndexOf("悬赏") > -1 || taskRecordByID.TaskDescription.GetString().IndexOf("英雄难度首领") > -1)
                    {
                        isHaveRewardTask = true;
                    }
                }
                int currentTierPropertyForBuilding = LettuceVillageDataUtil.GetCurrentTierPropertyForBuilding(Assets.MercenaryBuilding.Mercenarybuildingtype.TASKBOARD, Assets.TierProperties.Buildingtierproperty.TASKSLOTS);
                int numberOfSpecialTasks = LettuceVillageDataUtil.GetNumberOfTasksByType(Assets.MercenaryVisitor.VillageVisitorType.SPECIAL);
                int idleslot = currentTierPropertyForBuilding + numberOfSpecialTasks - LettuceVillageDataUtil.VisitorStates.Count;
                //Debug.Log("空闲任务栏：" + idleslot + " isHaveRewardTask:" + isHaveRewardTask);

                if (idleslot > 0) { isHaveRewardTask = false; }

                if (!PVEMode) { isHaveRewardTask = true; }  //刷图模式则isHaveRewardTask=true，忽略神秘人。
                                                            //UnityEngine.Debug.Log("isHaveRewardTask: " + isHaveRewardTask);
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        private static void SetAbilityNameFromTaskDescription(string taskDescription, int mercenaryId)
        {
            int num = taskDescription.IndexOf("$ability(");
            if (num == -1)
            {
                return;
            }
            num += "$ability(".Length;
            int num2 = taskDescription.IndexOf(")", num);
            if (num2 == -1)
            {
                return;
            }
            string[] array = taskDescription.Substring(num, num2 - num).Split(new char[]
            {
            ','
            });
            int num3 = 0;
            int num4 = 0;
            if (!int.TryParse(array[0], out num3) || !int.TryParse(array[1], out num4))
            {
                return;
            }
            LettuceMercenaryDbfRecord record = GameDbf.LettuceMercenary.GetRecord(mercenaryId);
            if (num3 >= record.LettuceMercenarySpecializations.Count)
            {
                return;
            }
            LettuceMercenarySpecializationDbfRecord lettuceMercenarySpecializationDbfRecord = record.LettuceMercenarySpecializations[num3];
            if (num4 >= lettuceMercenarySpecializationDbfRecord.LettuceMercenaryAbilities.Count)
            {
                return;
            }
            int lettuceAbilityId = lettuceMercenarySpecializationDbfRecord.LettuceMercenaryAbilities[num4].LettuceAbilityId;
            LettuceAbilityDbfRecord record2 = GameDbf.LettuceAbility.GetRecord(lettuceAbilityId);
            TaskAbilityName.Add(record2.AbilityName);
            //UnityEngine.Debug.Log("技能名：" + record2.AbilityName);
        }

        private static List<PegasusLettuce.LettuceMapNode> getshenmi(out int step)
        {
            List<List<PegasusLettuce.LettuceMapNode>> list = new List<List<PegasusLettuce.LettuceMapNode>>();
            List<List<PegasusLettuce.LettuceMapNode>> donelist = new List<List<PegasusLettuce.LettuceMapNode>>();
            NetCache.NetCacheLettuceMap netObject = NetCache.Get().GetNetObject<NetCache.NetCacheLettuceMap>();
            if (netObject.Map.BountyId == mapID && PVEMode) { isHaveRewardTask = false; }
            foreach (PegasusLettuce.LettuceMapNode lettuceMapNode in netObject.Map.Nodes)           //遍历地图节点
            {
                //GameDbf.LettuceMapNodeType.GetRecord(i).NodeVisualId;
                uint[] ia = { 0, 14, 18, 19, 23, 44 };
                if ((Array.IndexOf(ia, lettuceMapNode.NodeTypeId) > -1 && !isHaveRewardTask) || (lettuceMapNode.NodeTypeId == 3))         //如果节点类型为神秘选项并且没有悬赏任务则终止循环，或者节点为最终BOSS则终止循环
                {
                    foreach (List<PegasusLettuce.LettuceMapNode> tmp in list)
                    {
                        if (tmp.Exists(t => t.NodeId == lettuceMapNode.NodeId))
                        {
                            donelist.Add(tmp);
                        }
                    }
                    if (donelist.Count < 1) { donelist = list; }
                    break;
                }
                if (lettuceMapNode.Row == 1)
                {          //如果是第1行则初始化list
                    List<PegasusLettuce.LettuceMapNode> array = new List<PegasusLettuce.LettuceMapNode>();
                    array.Add(lettuceMapNode);
                    list.Add(array);
                }
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    if (list[i][list[i].Count - 1].NodeId == lettuceMapNode.NodeId)    //判断节点ID是否等于list里最后一位
                    {
                        if (lettuceMapNode.ChildNodeIds.Count == 2)             //添加子节点
                        {
                            List<PegasusLettuce.LettuceMapNode> t2 = new List<PegasusLettuce.LettuceMapNode>(list[i].ToArray());
                            list[i].Add(netObject.Map.Nodes[(int)lettuceMapNode.ChildNodeIds[0]]);
                            t2.Add(netObject.Map.Nodes[(int)lettuceMapNode.ChildNodeIds[1]]);
                            list.Add(t2);
                        }
                        else if (lettuceMapNode.ChildNodeIds.Count == 1)
                        {
                            list[i].Add(netObject.Map.Nodes[(int)lettuceMapNode.ChildNodeIds[0]]);
                        }
                    }
                }
            }
            //UnityEngine.Debug.Log("找到" + donelist.Count + "条前往神秘选项的路径");
            List<int> lujinCount = new List<int>();
            foreach (List<PegasusLettuce.LettuceMapNode> tmp in donelist)
            {
                string t = null;
                int count = 0;
                uint[] ia = { 1, 2, 3, 22 };    //节点为怪物
                foreach (PegasusLettuce.LettuceMapNode tmp1 in tmp)
                {
                    t = t + tmp1.NodeId.ToString() + " ";
                    if (Array.IndexOf(ia, tmp1.NodeTypeId) > -1)   //节点为怪物
                    {
                        count += 1;
                    }
                }
                lujinCount.Add(count);
                //UnityEngine.Debug.Log(t);
            }
            var min = 0;
            var min2 = lujinCount[0];
            for (int i = 0; i < lujinCount.Count; i++)
            {
                if (min2 > lujinCount[i])
                {
                    min2 = lujinCount[i];
                    min = i;
                }
            }
            //UnityEngine.Debug.Log("最短路径" + (min + 1) + " 步数：" + (min2));
            step = min2;
            if (isHaveRewardTask)   //如果有悬赏任务处理下 NodeTypeId==0   蓝色传送门后面到BOSS前全部设置为完成状态
            {
                step = 1;
                bool door = false;
                for (int i = 0; i < donelist[min].Count - 1; i++)
                {
                    if (door) { donelist[min][i].NodeState_ = PegasusLettuce.LettuceMapNode.NodeState.COMPLETE; }
                    if (donelist[min][i].NodeTypeId == 0)   //蓝色传送门后面到BOSS前全部设置为完成状态
                    {
                        door = true;
                    }
                }
            }
            return donelist[min];
        }


        public static void LoadPolicy()    //载入策略
        {
            string Strategy;
            if (isPVP)
            {
                Strategy = StrategyPVP;
            }
            else
            {
                Strategy = StrategyPVE;
            }
            string FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Strategy;
            if (File.Exists(FileName))
            {
                try
                {
                    //Debug.Log("策略文件存在，正在加载..." + FileName);
                    Assembly tmp = Assembly.Load(File.ReadAllBytes(FileName));         //载入策略DLL
                    //Debug.Log("Types数量：" + tmp.GetTypes().Length.ToString());
                    //Debug.Log("TypeName：" + tmp.GetTypes()[0].Name);
                    //Debug.Log("TypeFullName：" + tmp.GetTypes()[0].FullName);
                    Type[] types = tmp.GetTypes();
                    StrategyInstance = tmp.CreateInstance(types[0].Name);       //实例化Strategy类
                    //Debug.Log("StrategyInstance: "+StrategyInstance);
                    Entrance = StrategyInstance.GetType().GetMethod("Nomination");    //获取Entrance方法
                    Battle = StrategyInstance.GetType().GetMethod("Combat");        //获取Battle方法
                    //Debug.Log("Entrance:"+Entrance);
                    //Debug.Log("Battle:"+Battle);
                    //Entrance.Invoke(StrategyInstance, new object[] { });
                    if (Entrance != null && Battle != null)
                    {
                        StrategyOK = true;
                        if (UIStatus.Get())
                        {
                            UIStatus.Get().AddInfo("策略加载成功！");
                        }
                    }
                    else
                    {
                        StrategyOK = false;
                        if (UIStatus.Get())
                        {
                            UIStatus.Get().AddInfo("策略加载失败！");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("空间名：" + ex.Source + "；" + '\n' +
                                  "方法名：" + ex.TargetSite + '\n' +
                                  "故障点：" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf("\\") + 1, ex.StackTrace.Length - ex.StackTrace.LastIndexOf("\\") - 1) + '\n' +
                                  "错误提示：" + ex.Message);

                    StrategyOK = false;
                    if (UIStatus.Get())
                    {
                        UIStatus.Get().AddInfo("策略加载错误！");
                    }
                }

            }
            else
            {
                StrategyOK = false;
                StrategyInstance = null;
                Entrance = null;
                Battle = null;
                if (UIStatus.Get())
                {
                    UIStatus.Get().AddInfo("策略已取消！");
                }
            }
        }

        private static async Task StrategyAsync(MethodInfo methodInfo)
        {
            //Debug.Log("异步测试前");
            Task<bool> task = Task.Run(() =>
            {
                HandleQueueOK = false;
                Thread.Sleep(2500);
                methodInfo.Invoke(StrategyInstance, new object[] { });
                StrategyRun = false;
                return true;
            });
            bool taskResult1 = await task;  //内部自己执行了GetAwaiter() 
        }
    }

    public static class PluginInfo
    {
        // Token: 0x04000038 RID: 56
        public const string PLUGIN_GUID = "MercenariesHelper";

        // Token: 0x04000039 RID: 57
        public const string PLUGIN_NAME = "MercenariesHelper";

        // Token: 0x0400003A RID: 58
        public const string PLUGIN_VERSION = "1.0.3.0";
    }
}

