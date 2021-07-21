using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    // Edits the mood bonus from lovin if there is a relationship harmony between pawns.

    [HarmonyPatch(typeof(JobDriver_Lovin), "MakeNewToils")]
    static class Patch_JobDriver_Lovin_MakeNewToils
    {
        [TweakValue("Simple Personalities M2", 0, 20)]
        public static float MoodPowerMulti = 2;

        private static MethodInfo tickMethod;

        static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_Lovin __instance, TargetIndex ___PartnerInd)
        {
            var pawnA = __instance.pawn;
            var pawnB = (Pawn)((Thing)__instance.job.GetTarget(___PartnerInd));
            var interaction = PersonalityComparer.Compare(pawnA, pawnB);

            bool done = interaction != PersonalityInteraction.Diversive;

            foreach (var toil in values)
            {
                if (!done && toil.finishActions != null && toil.finishActions.Count == 1)
                {
                    toil.finishActions[0] = () => FinishAction(pawnA, pawnB, __instance);
                    done = true;
                }
                yield return toil;
            }
        }

        private static void FinishAction(Pawn pawn, Pawn partner, JobDriver_Lovin self)
        {
            // No pun intended.

            tickMethod ??= AccessTools.Method("RimWorld.JobDriver_Lovin:GenerateRandomMinTicksToNextLovin");

            // Just a copy of vanilla, with some slight modifications.
            Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.GotSomeLovin);
            if (pawn.health != null && pawn.health.hediffSet != null)
            {
                if (pawn.health.hediffSet.hediffs.Any((Hediff h) => h.def == HediffDefOf.LoveEnhancer))
                {
                    goto IL_C4; // These awful goto's are from my decompiler. They work, I don't question it.
                }
            }
            if (partner.health?.hediffSet == null)
            {
                goto IL_CF;
            }
            if (!partner.health.hediffSet.hediffs.Any((Hediff h) => h.def == HediffDefOf.LoveEnhancer))
            {
                goto IL_CF;
            }
            IL_C4:
            thought_Memory.moodPowerFactor = 1.5f * MoodPowerMulti;
            IL_CF:
            pawn.needs.mood?.thoughts.memories.TryGainMemory(thought_Memory, partner);
            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.GotLovin, pawn.Named(HistoryEventArgsNames.Doer)), true);
            HistoryEventDef def = pawn.relations.DirectRelationExists(PawnRelationDefOf.Spouse, partner) ? HistoryEventDefOf.GotLovin_Spouse : HistoryEventDefOf.GotLovin_NonSpouse;
            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(def, pawn.Named(HistoryEventArgsNames.Doer)), true);
            pawn.mindState.canLovinTick = Find.TickManager.TicksGame + (int)tickMethod.Invoke(self, new object[]{ pawn });
		}
    }
}
