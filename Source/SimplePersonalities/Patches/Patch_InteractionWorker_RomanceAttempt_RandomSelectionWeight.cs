using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight")]
    static class Patch_InteractionWorker_RomanceAttempt_RandomSelectionWeight
    {
        static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            __result *= Mathf.Min(GetWeightModifierFor(initiator, recipient), GetWeightModifierFor(recipient, initiator));
        }

        static float GetWeightModifierFor(Pawn pawn, Pawn anotherPawn)
        {
            var spouse = pawn.GetFirstSpouse();
            if (spouse != null && spouse != anotherPawn)
            {
                var interaction = PersonalityComparer.Compare(pawn, spouse);
                if (interaction == PersonalityInteraction.Harmonious)
                {
                    return 0.5f;
                }
            }
            return 1f;
        }
    }
}
