using HarmonyLib;
using RimWorld;
using System.Collections;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    /*
     * Changes Lovin' interval based on the involved pawn's personality harmony.
     */
    [HarmonyPatch(typeof(JobDriver_Lovin), "GenerateRandomMinTicksToNextLovin")]
    static class Patch_JobDriver_Lovin_GenerateRandomMinTicksToNextLovin
    {
        static void Postfix(JobDriver_Lovin __instance, Pawn pawn, ref int __result, TargetIndex ___PartnerInd)
        {
            if (Core.settings.SPM2_Couples)
            {
                var partner = (Pawn)((Thing)__instance.job.GetTarget(___PartnerInd));
                if (partner == null)
                    return;

                float multi = 1f;
                var interaction = PersonalityComparer.Compare(pawn, partner);
                switch (interaction)
                {
                    case PersonalityInteraction.Complementary:
                    case PersonalityInteraction.Harmonious:
                        multi = 0.75f;
                        break;
                }

                if (multi != 1f)
                    __result = Mathf.RoundToInt(__result * multi);
            }
        }
    }
}
