using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using VanillaSocialInteractionsExpanded;
using Verse;
using Verse.AI;

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
        [TweakValue("SimplePersonalities", 0, 4)] public static float complementarySpeedMult = 1.1f;
        [TweakValue("SimplePersonalities", 0, 4)] public static float disparateSpeedMult = 1.2f;
        private static void Postfix(List<Pawn> pawns, ref int __result, float massUsage, float massCapacity, StringBuilder explanation = null)
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

    [HarmonyPatch(new Type[]
    {
        typeof(List<Pawn>),
        typeof(bool),
        typeof(StringBuilder),
    })]
    [HarmonyPatch(typeof(CaravanVisibilityCalculator), "Visibility")]
    public static class CaravanVisibilityCalculator_Visibility
    {
        [TweakValue("SimplePersonalities", 0, 4)] public static float harmoniousVisibilityMult = 0.5f;
        [TweakValue("SimplePersonalities", 0, 4)] public static float disparateVisibilityMult = 1.5f;
        private static void Postfix(ref float __result, List<Pawn> pawns, bool caravanMovingNow, StringBuilder explanation = null)
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

    [HarmonyPatch(typeof(GetStatValue_Patch), nameof(GetStatValue_Patch.Postfix))]
    public static class Postfix_Transpiler_Patch
    {
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
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Postfix_Transpiler_Patch), nameof(GetPawnWorkAdditionalBonus)));
                    yield return new CodeInstruction(OpCodes.Mul);
                    yield return new CodeInstruction(OpCodes.Stind_R4);
                }
            }
        }
    
        [TweakValue("SimplePersonality", 0, 4)] public static float manyHandsLightWorkMult = 1.05f;
        private static float GetPawnWorkAdditionalBonus(List<Pawn> pawns)
        {
            var interaction = PersonalityComparer.Compare(pawns);
            if (interaction == PersonalityInteraction.Complementary || interaction == PersonalityInteraction.Disparate)
            {
                return manyHandsLightWorkMult;
            }
            return 1f;
        }
    }

    [HarmonyPatch(typeof(SocialInteractionsManager), nameof(SocialInteractionsManager.GameComponentTick))]
    public static class Discord_Transpiler_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                yield return code;
                if (code.opcode == OpCodes.Ldc_R4 && code.OperandIs(0.001f))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Discord_Transpiler_Patch), nameof(GetDiscordChance)));
                    yield return new CodeInstruction(OpCodes.Mul);
                }
            }
        }

        [TweakValue("SimplePersonality", 0, 4)] public static float harmoniousChanceMult = 0.5f;
        [TweakValue("SimplePersonality", 0, 4)] public static float disparateMult = 1.25f;
        private static float GetDiscordChance(KeyValuePair<Pawn, Workers> workers)
        {
            var worker = workers.Key;
            var pawns = worker.Map.mapPawns.SpawnedPawnsInFaction(worker.Faction).Where(x => x != worker && x != null && x.RaceProps.Humanlike
                && x.mindState != null && VSIE_Utils.workTags.Contains(x.mindState.lastJobTag) && x.needs?.mood?.CurLevelPercentage < 0.3f && !x.WorkTagIsDisabled(WorkTags.Violent)
                && x.Position.DistanceTo(worker.Position) < 10).ToHashSet();
            var candidates = workers.Value.workersWithWorkingTicks.Where(x => x.Key != null && x.Key.needs?.mood?.CurInstantLevelPercentage < 0.3f && x.Value.workTick > 3000
                    && worker.relations?.OpinionOf(x.Key) < 0).Select(x => x.Key).ToList();
            pawns.AddRange(candidates.AddItem(worker).ToList());
            var interaction = PersonalityComparer.Compare(pawns);
            if (interaction == PersonalityInteraction.Harmonious)
            {
                return harmoniousChanceMult;
            }
            else if (interaction == PersonalityInteraction.Disparate)
            {
                return disparateMult;
            }
            return 1f;
        }
    }

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
            var interaction = initiator.CompareWith(friend);
            if (interaction == PersonalityInteraction.Harmonious)
            {
                return SPM2DefOf.VSIE_Vent_Harmonious;
            }
            return VSIE_DefOf.VSIE_Vent;
        }
    }
}
