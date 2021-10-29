using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using VanillaSocialInteractionsExpanded;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(JobDriver_VentToFriend), nameof(JobDriver_VentToFriend.DoVentingTick))]
    public static class Vent_Transpiler_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.LoadsField(AccessTools.Field(typeof(VSIE_DefOf), nameof(VSIE_DefOf.VSIE_Vent))))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "pawn"));
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Vent_Transpiler_Patch), nameof(GetInteractionDef)));
                }
                else
                {
                    yield return code;
                }
            }
        }
        private static InteractionDef GetInteractionDef(Pawn initiator, Pawn friend)
        {
            if (Core.settings.SPM2_Venting)
            {
                var interaction = initiator.CompareWith(friend);
                if (interaction == PersonalityInteraction.Harmonious)
                {
                    return SPM2DefOf.VSIE_Vent_Harmonious;
                }
            }
            return VSIE_DefOf.VSIE_Vent;
        }
    }
}
