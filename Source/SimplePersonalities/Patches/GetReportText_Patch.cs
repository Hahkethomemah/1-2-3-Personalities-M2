using HarmonyLib;
using RimWorld;
using SPM1;
using Verse;
using Verse.Grammar;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(Faction), "GetReportText", MethodType.Getter)]
    public static class GetReportText_Patch
    {
        public static void Postfix(Faction __instance, ref string __result)
        {
            if (Core.settings.SPM2_Leaders && __instance.leader != null)
            {
                __result += "\n\n" + GetFactionLeaderDescription(SPM2DefOf.SP_FactionLeaderDescription, __instance);
            }
        }

        public static TaggedString GetFactionLeaderDescription(RulePackDef rule, Faction faction)
        {
            Rand.PushState();
            Rand.Seed = faction.leader.thingIDNumber;
            string rootKeyword = "root";
            GrammarRequest request = default(GrammarRequest);
            request.Rules.AddRange(GrammarUtility.RulesForPawn("FACTION_LEADER", faction.leader, request.Constants));
            request.Rules.Add(new Rule_String("DRIVE", faction.leader.TryGetEnneagramComp().Enneagram.Root.drive.label));
            request.Includes.Add(rule);
            string str = GrammarResolver.Resolve(rootKeyword, request);
            Rand.PopState();
            return str;
        }
    }
}
