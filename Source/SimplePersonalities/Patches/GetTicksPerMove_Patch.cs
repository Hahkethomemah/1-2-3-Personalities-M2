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
        typeof(float),
        typeof(float),
        typeof(StringBuilder)
    })]
    [HarmonyPatch(typeof(CaravanTicksPerMoveUtility), "GetTicksPerMove")]
    public static class GetTicksPerMove_Patch
    {
        [TweakValue("0SimplePersonalities", 0, 4)] public static float complementarySpeedMult = 1.1f;
        [TweakValue("0SimplePersonalities", 0, 4)] public static float disparateSpeedMult = 1.2f;
        private static void Postfix(List<Pawn> pawns, ref int __result, float massUsage, float massCapacity, StringBuilder explanation = null)
        {
            if (Core.settings.SPM2_Caravans)
            {
                var interaction = PersonalityComparer.Compare(pawns);
                if (interaction == PersonalityInteraction.Complementary)
                {
                    __result = (int)(__result * complementarySpeedMult);
                }
                else if (interaction == PersonalityInteraction.Disparate)
                {
                    __result = (int)(__result * disparateSpeedMult);
                }
            }
        }
    }
}
