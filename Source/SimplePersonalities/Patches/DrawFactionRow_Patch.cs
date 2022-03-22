using HarmonyLib;
using RimWorld;
using SPM1;
using UnityEngine;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(FactionUIUtility), "DrawFactionRow")]
    public static class DrawFactionRow_Patch
    {
        public static void Prefix(ref float __result, Faction faction, ref float rowY, ref Rect fillRect)
        {
            Text.WordWrap = false;
        }
        public static void Postfix(ref float __result, Faction faction, ref float rowY, ref Rect fillRect)
        {
            if (Core.settings.SPM2_Leaders)
            {
                if (faction.leader != null)
                {
                    Rect rect = new Rect(90, rowY + 48, 250, 25);
                    var drive = faction.leader.TryGetEnneagramComp().Enneagram.Root.drive;
                    Widgets.Label(rect, "SP.FactionLeaderDescriptionNoName".Translate(faction.leader.Possessive(), $"<color={Settings.ForcedTraitColor ?? drive.color}>{drive.label}</color>"));
                }
            }
            Text.WordWrap = true;
        }
    }
}
