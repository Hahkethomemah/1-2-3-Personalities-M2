using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForFreeIntercept))]
    public static class Projectile_CheckForFreeIntercept_Transpiler_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (i > 3 && codes[i - 2].LoadsField(AccessTools.Field(typeof(Difficulty), nameof(Difficulty.friendlyFireChanceFactor))) 
                    && codes[i - 1].opcode == OpCodes.Mul && codes[i].opcode == OpCodes.Stloc_S)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 6);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Projectile), "launcher"));
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Projectile_CheckForFreeIntercept_Transpiler_Patch), nameof(GetFriendlyFireMultiplicator)));
                    yield return new CodeInstruction(OpCodes.Mul);
                    yield return new CodeInstruction(OpCodes.Stloc_S, 6);
                }
            }
        }

        private static float GetFriendlyFireMultiplicator(Thing launcher, Pawn victim)
        {
            if (Core.settings.SPM2_FriendlyFire)
            {
                if (launcher is Pawn instigator && !instigator.HostileTo(victim))
                {
                    var interaction = instigator.CompareWith(victim);
                    if (interaction == PersonalityInteraction.Harmonious)
                    {
                        return AttackTargetFinder_GetShootingTargetScore_Patch.harmoniousFriendlyFireChanceMult;
                    }
                }
            }
            return 1f;
        }
    }
}
