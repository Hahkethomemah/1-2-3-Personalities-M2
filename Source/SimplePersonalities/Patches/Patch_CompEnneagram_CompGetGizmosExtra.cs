using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using SPM1.Comps;
using SPM1;
using Verse;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(CompEnneagram), "CompGetGizmosExtra")]
    static class Patch_CompEnneagram_CompGetGizmosExtra
    {
        static void Postfix(CompEnneagram __instance, ref IEnumerable<Gizmo> __result)
        {
            __result = Process(__result, __instance is not CompEnneagramAnimal, __instance.parent as Pawn);
        }

        static IEnumerable<Gizmo> Process(IEnumerable<Gizmo> original, bool addExtra, Pawn pawn)
        {
            foreach (var value in original)
                yield return value;

            if (!addExtra)
                yield break;

            var pawns = Find.Selector?.SelectedPawns;
            pawns?.RemoveAll(p => p == null || p.RaceProps.Animal || p.TryGetEnneagramComp() == null);
            int count = (pawns?.Count ?? 0);

            if (count <= 1)
                yield break;

            bool isGroup = count > 2;

            yield return new Command_Action()
            {
                defaultLabel = (isGroup ? "SP.CheckInteractionGroup" : "SP.CheckInteraction").Translate(),
                alsoClickIfOtherInGroupClicked = false,
                action = () =>
                {
                    Core.Trace($"Group selection ({pawns.Count} pawns)");

                    PersonalityInteraction outcome = PersonalityComparer.Compare(pawns);

                    Core.Trace($"Group interaction: {outcome}");

                    string msgTrs = isGroup ? "SP.InteractionMessageGroup" : "SP.InteractionMessage";
                    string desc = $"SP.Int_{outcome}".Translate();
                    string msg;
                    if (isGroup)
                        msg = msgTrs.Translate(pawns.Count, desc);
                    else
                        msg = msgTrs.Translate(pawns[0].NameShortColored, pawns[1].NameShortColored, desc);

                    Messages.Message(msg, MessageTypeDefOf.PositiveEvent, false);
                }
            };
        }
    }
}
