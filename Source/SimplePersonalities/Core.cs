using HarmonyLib;
using ReloaderAPI;
using System;
using UnityEngine;
using Verse;

namespace SPM2
{
    [Reloadable(ReloadBehaviour.All)]
    public class Core : Mod
    {
        public static SPM2Settings settings;
        public static void Log(string msg)
        {
            Verse.Log.Message($"<color=#34eb92>[Simple Personalities M2]</color> {msg}");
        }

        public static void Warn(string msg)
        {
            Verse.Log.Warning($"<color=#34eb92>[Simple Personalities M2]</color> {msg}");
        }

        public static void Error(string msg, Exception e = null)
        {
            Verse.Log.Error($"<color=#34eb92>[Simple Personalities M2]</color> {msg}");
            if(e != null)
                Verse.Log.Error(e.ToString());
        }

        public static void Trace(string msg)
        {
            Verse.Log.Message($"<color=#34eb92>[Simple Personalities M2] [TRACE] </color> {msg}");
        }
        public Core(ModContentPack pack) : base(pack)
        {
            Log("Module 2 reporting for duty!");
            var harmony = new Harmony("hahkethomemah.simplepersonalities.module2");
            Verse.Log.Message("PATCHALL");
            harmony.PatchAll();
            settings = GetSettings<SPM2Settings>();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }
    }
}
