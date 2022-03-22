using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    [HarmonyPatch]
    static class JobDriver_Lovin_InitAction_Vanilla
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.GetDeclaredMethods(typeof(JobDriver_Lovin)).FirstOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var ticksLeftField = AccessTools.Field(typeof(JobDriver_Lovin), "ticksLeft");
            foreach (var code in instructions)
            {
                yield return code;
                if (!found && code.StoresField(ticksLeftField))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_Lovin_InitAction_Vanilla), nameof(ModifyLovinTicks)));
                }
            }
        }

        [TweakValue("0SimplePersonalities", 0, 4)] public static float chanceOfPassionateLovin = 0.1f;
        [TweakValue("0SimplePersonalities", 0, 4)] public static float passionateLovinDurationMultiplier = 2f;
        private static void ModifyLovinTicks(JobDriver_Lovin jobDriver)
        {
            if (Core.settings.SPM2_Couples)
            {
                var pawn = jobDriver.pawn;
                var parther = jobDriver.Partner;
                var interaction = PersonalityComparer.Compare(pawn, parther);
                if (interaction == PersonalityInteraction.Complementary && Rand.Chance(chanceOfPassionateLovin))
                {
                    jobDriver.collideWithPawns = true; // we treat it as a bool to indicate that it's a passionate lovin
                    parther.jobs.curDriver.collideWithPawns = true;
                    jobDriver.ticksLeft *= 2;
                }
            }
        }
    }

    [HarmonyPatch]
    static class JobDriver_Lovin_InitAction_VSIE
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
            methodTarget = AccessTools.GetDeclaredMethods(typeof(VanillaSocialInteractionsExpanded.JobDriver_LovinOneNightStand)).FirstOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
        }
    
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;
    
        public static MethodInfo methodTarget;
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var ticksLeftField = AccessTools.Field(typeof(VanillaSocialInteractionsExpanded.JobDriver_LovinOneNightStand), "ticksLeft");
            foreach (var code in instructions)
            {
                yield return code;
                if (!found && code.StoresField(ticksLeftField))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_Lovin_InitAction_VSIE), nameof(ModifyLovinTicks)));
                }
            }
        }
    
        [TweakValue("0SimplePersonalities", 0, 4)] public static float chanceOfPassionateLovin = 0.1f;
        [TweakValue("0SimplePersonalities", 0, 4)] public static float passionateLovinDurationMultiplier = 2f;
        private static void ModifyLovinTicks(JobDriver jobDriver)
        {
            if (Core.settings.SPM2_Couples)
            {
                var pawn = jobDriver.pawn;
                var jobdriver2 = jobDriver as VanillaSocialInteractionsExpanded.JobDriver_LovinOneNightStand;
                var parther = jobdriver2.Partner;
                var interaction = PersonalityComparer.Compare(pawn, parther);
                if (interaction == PersonalityInteraction.Complementary && Rand.Chance(chanceOfPassionateLovin))
                {
                    jobDriver.collideWithPawns = true; // we treat it as a bool to indicate that it's a passionate lovin
                    parther.jobs.curDriver.collideWithPawns = true;
                    jobdriver2.ticksLeft *= 2;
                }
            }
        }
    }
}
