using HarmonyLib;
using System.Reflection;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch]
    public static class SocialInteractionsManager_TryDevelopNewTrait_Patch
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
            methodTarget = AccessTools.Method(typeof(VanillaSocialInteractionsExpanded.SocialInteractionsManager), nameof(VanillaSocialInteractionsExpanded.SocialInteractionsManager.TryDevelopNewTrait));
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;

        public static MethodInfo methodTarget;

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
        }
    }
}
