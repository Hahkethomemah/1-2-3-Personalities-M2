using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.UIElements;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch]
    public static class Discord_Transpiler_Patch
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
            methodTarget = AccessTools.Method(typeof(VanillaSocialInteractionsExpanded.SocialInteractionsManager), nameof(VanillaSocialInteractionsExpanded.SocialInteractionsManager.GameComponentTick));
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;

        public static MethodInfo methodTarget;

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
        private static float GetDiscordChance(KeyValuePair<Pawn, object> workers)
        {
            if (Core.settings.SPM2_Discord)
            {
                var worker = workers.Key;
                if (worker.Map != null)
                {
                    var pawns = worker.Map.mapPawns.SpawnedPawnsInFaction(worker.Faction).Where(x => x != worker && x != null && x.RaceProps.Humanlike
                        && x.mindState != null && VanillaSocialInteractionsExpanded.VSIE_Utils.workTags.Contains(x.mindState.lastJobTag) && x.needs?.mood?.CurLevelPercentage < 0.3f && !x.WorkTagIsDisabled(WorkTags.Violent)
                        && x.Position.DistanceTo(worker.Position) < 10).ToHashSet();
                    var value = workers.Value as VanillaSocialInteractionsExpanded.Workers;
                    foreach (var kvp in value.workersWithWorkingTicks)
                    {
                        if (kvp.Key != null && kvp.Key.needs?.mood?.CurInstantLevelPercentage < 0.3f && kvp.Value.workTick > 3000
                            && worker.relations?.OpinionOf(kvp.Key) < 0)
                        {
                            pawns.Add(kvp.Key);
                        }
                    }
                    pawns.Add(worker);
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
            }
            return 1f;
        }
    }
}
