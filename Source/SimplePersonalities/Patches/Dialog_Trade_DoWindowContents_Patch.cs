using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch]
    static class Dialog_Trade_DoWindowContents_Patch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Dialog_Trade), "DoWindowContents");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].LoadsField(AccessTools.Field(typeof(TradeSession), "playerNegotiator")) && codes[i + 1].LoadsField(AccessTools.Field(typeof(StatDefOf), "TradePriceImprovement")))
                {
                    i += 3;
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Dialog_Trade_DoWindowContents_Patch), nameof(ActualTradePriceImprovement)));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        public static void Postfix(Dialog_Trade __instance, Rect inRect)
        {
            if (Core.settings.SPM2_Trading && Tradeable_Patch.GetNegotiatorAndTrader(out Pawn negotiator, out Pawn trader))
            {
                if (PersonalityComparer.Compare(negotiator, trader) == PersonalityInteraction.Harmonious)
                {
                    var rect = new Rect(250, 27f, 24, 24);
                    GUI.DrawTexture(rect, ContentFinder<Texture2D>.Get("UI/Commands/FulfillTradeRequest"));
                    if (Mouse.IsOver(rect))
                    {
                        TooltipHandler.TipRegion(rect, "SP.TradeNegotiatorTooltip".Translate(negotiator.Named("NEGOTIATOR"), 
                            "SP.BetterPrices".Translate(Tradeable_Patch.harmoniousPriceGainMult * 100f) + "\n" 
                            + "SP.BetterReputations".Translate(Mathf.Abs(1 - FactionGiftUtility_GetGoodwillChange_Patch.harmoniousGiftMult) * 100f)));
                    }
                }
            }
        }

        private static float ActualTradePriceImprovement()
        {
            var tradePriceImprovementValue = TradeSession.playerNegotiator.GetStatValue(StatDefOf.TradePriceImprovement);
            if (Core.settings.SPM2_Trading && Tradeable_Patch.GetNegotiatorAndTrader(out Pawn negotiator, out Pawn trader))
            {
                if (PersonalityComparer.Compare(negotiator, trader) == PersonalityInteraction.Harmonious)
                {
                    tradePriceImprovementValue += Tradeable_Patch.harmoniousPriceGainMult;
                }
            }
            return tradePriceImprovementValue;
        }
    }
}
