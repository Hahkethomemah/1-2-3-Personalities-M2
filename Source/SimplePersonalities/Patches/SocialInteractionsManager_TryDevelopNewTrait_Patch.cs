using HarmonyLib;
using VanillaSocialInteractionsExpanded;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(SocialInteractionsManager), nameof(SocialInteractionsManager.TryDevelopNewTrait))]
    public static class SocialInteractionsManager_TryDevelopNewTrait_Patch
    {
        public static bool developedNewTrait;
        private static void Prefix(Pawn pawn)
        {
            developedNewTrait = false;
        }

        private static void DevelopedTrait()
        {

        }

        private static void Postfix()
        {
            developedNewTrait = true;

            if (!developedNewTrait && Core.settings.SPM2_ObtainingNewCharacterTraits)
            {

            }
        }
    }
}
