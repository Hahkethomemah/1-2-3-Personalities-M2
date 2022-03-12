using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse.AI;

namespace SPM2.Patches
{

    [HarmonyPatch]
    static class JobDriver_Lovin_FinishAction_Vanilla
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.GetDeclaredMethods(typeof(JobDriver_Lovin)).LastOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.LoadsField(AccessTools.Field(typeof(ThoughtDefOf), "GotSomeLovin")))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_Lovin_FinishAction_Vanilla), nameof(LoadThoughtDef)));
                }
                else
                {
                    yield return code;
                }
            }
        }
        private static ThoughtDef LoadThoughtDef(JobDriver_Lovin jobDriver)
        {
            if (jobDriver.collideWithPawns)
            {
                return SPM2DefOf.SP_PassionateLovin;
            }
            return ThoughtDefOf.GotSomeLovin;
        }
    }

    [HarmonyPatch]
    static class JobDriver_Lovin_FinishAction_VSIE
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
            methodTarget = AccessTools.GetDeclaredMethods(typeof(VanillaSocialInteractionsExpanded.JobDriver_LovinOneNightStand)).LastOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;

        public static MethodInfo methodTarget;
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.LoadsField(AccessTools.Field(typeof(VanillaSocialInteractionsExpanded.VSIE_DefOf), "VSIE_GotSomeLovin")))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_Lovin_FinishAction_VSIE), nameof(LoadThoughtDef)));
                }
                else
                {
                    yield return code;
                }
            }
        }
        private static ThoughtDef LoadThoughtDef(JobDriver jobDriver)
        {
            if (jobDriver.collideWithPawns)
            {
                return SPM2DefOf.SP_PassionateLovinOneNightStand;
            }
            return ThoughtDefOf.GotSomeLovin;
        }
    }
}
