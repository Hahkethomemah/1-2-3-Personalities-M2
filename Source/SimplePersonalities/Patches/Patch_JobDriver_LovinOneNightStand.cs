using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using VanillaSocialInteractionsExpanded;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(JobDriver_LovinOneNightStand), "MakeNewToils")]
    static class Patch_JobDriver_LovinOneNightStand
    {
        [TweakValue("0SimplePersonality", 0, 2)] public static float harmoniousCoupleChance = 0.2f;
        static IEnumerable<Toil> Postfix(IEnumerable<Toil> __result, JobDriver_LovinOneNightStand __instance)
        {
            foreach (var r in __result) yield return r;
            yield return new Toil
            {
                initAction = delegate
                {
                    var pawn = __instance.pawn;
                    var partner = (Pawn)(Thing)__instance.job.GetTarget(TargetIndex.A);
                    var interaction = pawn.CompareWith(partner);
                    if (interaction == PersonalityInteraction.Harmonious && Rand.Chance(harmoniousCoupleChance))
                    {
                        BecomeCouple(pawn, partner);
                        if (Rand.Chance(0.5f))
                        {
                            // add other poison
                        } 
                        else 
                        {
                            // add another one
                        }
                    }
                }
            };
        }
        static void BecomeCouple(Pawn initiator, Pawn recipient)
        {
            if (initiator.GetSpouseOrLoverOrFiance() != recipient)
            {
                var worker = new InteractionWorker_RomanceAttempt();
                worker.BreakLoverAndFianceRelations(initiator, out var oldLoversAndFiances);
                worker.BreakLoverAndFianceRelations(recipient, out var oldLoversAndFiances2);
                worker.RemoveBrokeUpAndFailedRomanceThoughts(initiator, recipient);
                worker.RemoveBrokeUpAndFailedRomanceThoughts(recipient, initiator);
                for (int i = 0; i < oldLoversAndFiances.Count; i++)
                {
                    worker.TryAddCheaterThought(oldLoversAndFiances[i], initiator);
                }
                for (int j = 0; j < oldLoversAndFiances2.Count; j++)
                {
                    worker.TryAddCheaterThought(oldLoversAndFiances2[j], recipient);
                }
                initiator.relations.TryRemoveDirectRelation(PawnRelationDefOf.ExLover, recipient);
                initiator.relations.AddDirectRelation(PawnRelationDefOf.Lover, recipient);
                TaleRecorder.RecordTale(TaleDefOf.BecameLover, initiator, recipient);
                if (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient))
                {
                    worker.GetNewLoversLetter(initiator, recipient, oldLoversAndFiances, oldLoversAndFiances2, out _, out _, out _, out _);
                }
                LovePartnerRelationUtility.TryToShareBed(initiator, recipient);
            }
        }
    }
}
