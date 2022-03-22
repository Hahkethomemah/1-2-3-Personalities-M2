using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(FactionGiftUtility), "GetGoodwillChange", new Type[] {typeof(List<Tradeable>), typeof(Faction)})]
    public static class FactionGiftUtility_GetGoodwillChange_Patch
    {
        [TweakValue("0SimplePersonalities", 0f, 4f)] public static float harmoniousGiftMult = 1.2f;
        public static void Postfix(ref int __result)
        {
            if (Core.settings.SPM2_Trading && Tradeable_Patch.GetNegotiatorAndTrader(out Pawn negotiator, out Pawn trader))
            {
                if (PersonalityComparer.Compare(negotiator, trader) == PersonalityInteraction.Harmonious)
                {
                    __result = (int)(__result * harmoniousGiftMult);
                }
            }
        }
    }
}
