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

namespace BlankStone
{
    [BepInPlugin("BlankStone", "佣兵挂机插件", "1.0.3.0")]
    public class BlankStone : BaseUnityPlugin
    {
        public static string logPath = @"BepInEx\BlankStone.log";
        // 在插件启动时会直接调用Awake()方法
        void Awake()
        {
            // 使用Debug.Log()方法来将文本输出到控制台
            Debug.Log("BlankStone Awake!");
        }


        // 在所有插件全部启动完成后会调用Start()方法，执行顺序在Awake()后面；
        void Start()
        {
            Debug.Log("BlankStone Start!");
            //Harmony harmony = new Harmony("BlankStone.patch");
            //harmony.PatchAll();
            //MethodInfo srcMethod = typeof(Actor).GetMethod("Start");
            //MethodInfo dstMethod = typeof(BlankStone).GetMethod("PatchActorStart");
            //// harmony.Patch(srcMethod, new HarmonyMethod(dstMethod));
            //// srcMethod = typeof(Renderer).GetMethod("enabled", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            //srcMethod = typeof(RendererExtension).GetMethod("SetMaterial", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            //dstMethod = typeof(BlankStone).GetMethod("PatchSetMaterial");
            //harmony.Patch(srcMethod, null, new HarmonyMethod(dstMethod));
        }
        //public static void PatchSetMaterial(Renderer renderer, int materialIndex, Material material)
        //{
        //    renderer.material = null;
        //    renderer.enabled = false;
        //}

        // 插件启动后会一直循环执行Update()方法，可用于监听事件或判断键盘按键，执行顺序在Start()后面
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F6))
            {
                Debug.Log("$\"Update -> Removing material from renderer {renderer.transform.name}\"");
                foreach (var renderer in FindObjectsOfType<Renderer>())
                {
                    if (renderer != null) renderer.enabled = false;
                    if (renderer.material != null) renderer.material = null;
                }
            }
        }

        // 在插件关闭时会调用OnDestroy()方法
        void OnDestroy()
        {
            ;
        }

        //[HarmonyPatch(typeof(Renderer), "enabled", MethodType.Setter)]
        //class PatchRendererEnabledSet
        //{
        //    public static bool Prefix(Renderer __instance)
        //    {
        //        Traverse.Create(__instance).Property("enabled").SetValue(false);
        //        return false;
        //    }
        //}

    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "BlankStone";
        public const string PLUGIN_NAME = "BlankStone";
        public const string PLUGIN_VERSION = "1.0.0.0";
    }
}

