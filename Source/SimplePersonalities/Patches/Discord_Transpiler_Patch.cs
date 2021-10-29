using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using VanillaSocialInteractionsExpanded;
using Verse;

namespace SPM2.Patches
{
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

        [TweakValue("0SimplePersonalities", 0, 4)] public static float harmoniousChanceMult = 0.5f;
        [TweakValue("0SimplePersonalities", 0, 4)] public static float disparateMult = 1.25f;
        private static float GetDiscordChance(KeyValuePair<Pawn, Workers> workers)
        {
            if (Core.settings.SPM2_Discord)
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
            }
            return 1f;
        }
    }
}
