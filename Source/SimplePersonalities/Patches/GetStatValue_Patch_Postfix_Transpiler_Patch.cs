using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch]
    public static class GetStatValue_Patch_Postfix_Transpiler_Patch
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
            methodTarget = AccessTools.Method(typeof(VanillaSocialInteractionsExpanded.GetStatValue_Patch), nameof(VanillaSocialInteractionsExpanded.GetStatValue_Patch.Postfix));
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;

        public static MethodInfo methodTarget;
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int num = 0;
            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Br_S)
                {
                    num++;
                }
                yield return code;
                if (num == 9 && code.opcode == OpCodes.Stind_R4)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    yield return new CodeInstruction(OpCodes.Ldind_R4);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GetStatValue_Patch_Postfix_Transpiler_Patch), nameof(GetPawnWorkAdditionalBonus)));
                    yield return new CodeInstruction(OpCodes.Mul);
                    yield return new CodeInstruction(OpCodes.Stind_R4);
                }
            }
        }
    
        [TweakValue("0SimplePersonalities", 0, 4)] public static float manyHandsLightWorkMult = 1.05f;
        private static float GetPawnWorkAdditionalBonus(List<Pawn> pawns)
        {
            if (Core.settings.SPM2_ManyHandsMakeLightWork)
            {
                var interaction = PersonalityComparer.Compare(pawns);
                if (interaction == PersonalityInteraction.Complementary || interaction == PersonalityInteraction.Disparate)
                {
                    return manyHandsLightWorkMult;
                }
            }
            return 1f;
        }
    }
}
