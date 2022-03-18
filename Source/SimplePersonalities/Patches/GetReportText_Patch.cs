using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using SPM1;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(WorldObject), "GetDescription")]
    public static class GetDescription_Patch
    {
        public static void Postfix(WorldObject __instance, ref string __result)
        {
            if (Core.settings.SPM2_Leaders && __instance.Faction?.leader != null)
            {
                __result += "\n\n" + GetReportText_Patch.GetFactionLeaderDescription(SPM2DefOf.SP_FactionLeaderDescription, __instance.Faction);
            }
        }
    }

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
            var drive = faction.leader.TryGetEnneagramComp().Enneagram.Root.drive;
            request.Rules.Add(new Rule_String("DRIVE", $"<color={Settings.ForcedTraitColor ?? drive.color}>{drive.label}</color>"));
            request.Includes.Add(rule);
            string str = GrammarResolver.Resolve(rootKeyword, request);
            Rand.PopState();
            return str;
        }
    }
}
