using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    [HarmonyPatch]
    public static class Vent_Transpiler_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare()
        {
            if (Core.VSIEInstalled)
            {
                FindMethod();
                return methodTarget != null;
            }
            return false;
        }

        private static void FindMethod()
        {
            methodTarget = AccessTools.Method(typeof(VanillaSocialInteractionsExpanded.JobDriver_VentToFriend), nameof(VanillaSocialInteractionsExpanded.JobDriver_VentToFriend.DoVentingTick));
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;

        public static MethodInfo methodTarget;
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.LoadsField(AccessTools.Field(typeof(VanillaSocialInteractionsExpanded.VSIE_DefOf), nameof(VanillaSocialInteractionsExpanded.VSIE_DefOf.VSIE_Vent))))
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
                    return DefDatabase<InteractionDef>.GetNamed("VSIE_Vent_Harmonious");
                }
            }
            return VanillaSocialInteractionsExpanded.VSIE_DefOf.VSIE_Vent;
        }
    }
}
