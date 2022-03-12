using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    [HarmonyPatch]
    static class Patch_JobDriver_LovinOneNightStand
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
            methodTarget = AccessTools.Method(typeof(VanillaSocialInteractionsExpanded.JobDriver_LovinOneNightStand), "MakeNewToils");
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;

        public static MethodInfo methodTarget;

        [TweakValue("0SimplePersonalities", 0, 4)] public static float harmoniousCoupleChance = 0.2f;
        static IEnumerable<Toil> Postfix(IEnumerable<Toil> __result, JobDriver __instance)
        {
            foreach (var r in __result) yield return r;
            if (Core.settings.SPM2_Couples)
            {
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
                        }
                    }
                };
            }

        }
        static void BecomeCouple(Pawn initiator, Pawn recipient)
        {
            if (VanillaSocialInteractionsExpanded.VSIE_Utils.GetSpouseOrLoverOrFiance(initiator) != recipient)
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
