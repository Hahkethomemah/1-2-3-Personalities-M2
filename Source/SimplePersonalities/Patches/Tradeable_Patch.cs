using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch]
    static class Tradeable_Patch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Tradeable), "InitPriceDataIfNeeded");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var priceGain_PlayerNegotiatorField = AccessTools.Field(typeof(Tradeable), "priceGain_PlayerNegotiator");
            foreach (var code in instructions)
            {
                yield return code;
                if (!found && code.StoresField(priceGain_PlayerNegotiatorField))
                {	
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Tradeable_Patch), nameof(ModifyTradePriceImprovement)));
                }
            }
        }

        [TweakValue("0SimplePersonalities", 0f, 4f)] public static float harmoniousPriceGainMult = 0.1f;
        private static void ModifyTradePriceImprovement(Tradeable __instance)
        {
            if (Core.settings.SPM2_Trading && GetNegotiatorAndTrader(out Pawn negotiator, out Pawn trader))
            {
                if (PersonalityComparer.Compare(negotiator, trader) == PersonalityInteraction.Harmonious)
                {
                    __instance.priceGain_PlayerNegotiator += harmoniousPriceGainMult;
                }
            }
        }
        public static bool GetNegotiatorAndTrader(out Pawn negotiator, out Pawn trader)
        {
            negotiator = TradeSession.playerNegotiator;
            trader = null;
            if (TradeSession.trader is Pawn pawn)
            {
                trader = pawn;
            }
            else if (TradeSession.trader.Faction?.leader != null)
            {
                trader = TradeSession.trader.Faction.leader;
            }
            return trader != null && negotiator != null;
        }
    }
}
