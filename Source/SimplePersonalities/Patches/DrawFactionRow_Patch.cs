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
        public static void Postfix(ref float __result, Faction faction, float rowY, Rect fillRect)
        {
            if (Core.settings.SPM2_Leaders)
            {
                if (faction.leader != null)
                {
                    Rect rect = new Rect(90, rowY + 48, 250, 25);

                    Widgets.Label(rect, "SP.FactionLeaderDescriptionNoName".Translate(faction.leader.Possessive(), faction.leader.TryGetEnneagramComp().Enneagram.Root.drive.label));
                }
            }
        }
    }
}
