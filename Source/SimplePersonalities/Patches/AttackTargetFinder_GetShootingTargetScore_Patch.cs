using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(AttackTargetFinder), "GetShootingTargetScore")]
    public static class AttackTargetFinder_GetShootingTargetScore_Patch
    {
        [TweakValue("0SimplePersonalities", 0f, 4f)] public static float harmoniousFriendlyFireChanceMult = 0.66f;
        public static void Postfix(ref float __result, IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
        {
            if (Core.settings.SPM2_FriendlyFire && target is Pawn pawnTarget && searcher is Pawn pawnSearcher)
            {
                if (!pawnSearcher.HostileTo(pawnSearcher) && PersonalityComparer.Compare(pawnTarget, pawnSearcher) == PersonalityInteraction.Harmonious)
                {
                    __result *= harmoniousFriendlyFireChanceMult;
                }
            }
        }
    }
}
