using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch(new Type[]
    {
        typeof(List<Pawn>),
        typeof(bool),
        typeof(StringBuilder),
    })]
    [HarmonyPatch(typeof(CaravanVisibilityCalculator), "Visibility")]
    public static class CaravanVisibilityCalculator_Visibility
    {
        [TweakValue("0SimplePersonalities", 0, 4)] public static float harmoniousVisibilityMult = 0.5f;
        [TweakValue("0SimplePersonalities", 0, 4)] public static float disparateVisibilityMult = 1.5f;
        private static void Postfix(ref float __result, List<Pawn> pawns, bool caravanMovingNow, StringBuilder explanation = null)
        {
            if (Core.settings.SPM2_Caravans)
            {
                var interaction = PersonalityComparer.Compare(pawns);
                if (interaction == PersonalityInteraction.Harmonious)
                {
                    __result *= harmoniousVisibilityMult;
                }
                else if (interaction == PersonalityInteraction.Disparate)
                {
                    __result *= disparateVisibilityMult;
                }
            }
        }
    }
}
