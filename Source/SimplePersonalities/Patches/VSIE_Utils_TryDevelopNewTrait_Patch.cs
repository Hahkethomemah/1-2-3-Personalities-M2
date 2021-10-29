using HarmonyLib;
using RimWorld;
using SPM1;
using System.Collections.Generic;
using VanillaSocialInteractionsExpanded;
using Verse;
using Verse.Grammar;

namespace SPM2.Patches
{
    [HarmonyPatch(typeof(VSIE_Utils), nameof(VSIE_Utils.TryDevelopNewTrait))]
    public static class VSIE_Utils_TryDevelopNewTrait_Patch
    {
        private static HashSet<PersonalityTrait> tempHashset = new HashSet<PersonalityTrait>();
        private static void Postfix(Pawn pawn)
        {
            if (!SocialInteractionsManager_TryDevelopNewTrait_Patch.developedNewTrait && Core.settings.SPM2_ObtainingNewCharacterTraits && Rand.Chance(0.1f))
            {
                var gameComp = Current.Game.GetComponent<SimplePersonalitiesManagerM2>();
                if (gameComp.pawnsByChangeTraitImmunityTick.TryGetValue(pawn, out int tick) && tick > Find.TickManager.TicksGame)
                {
                    return;
                }

                PersonalityTrait personalityTraitNew = null;
                PersonalityTrait personalityTraitOld = null;

                var comp = pawn?.TryGetEnneagramComp();
                var gram = comp.Enneagram;
                if (gram.OptionalTrait is null)
                {
                    tempHashset.Clear();
                    tempHashset.Add(gram.MainTrait);
                    tempHashset.Add(gram.SecondaryTrait);
                    gram.OptionalTrait = PersonalityRoot.AllHuman.RandomElementExcept(gram.Root).GetRandomCompatibleTrait(gram.Variant, tempHashset);
                    personalityTraitNew = gram.OptionalTrait;
                }
                else
                {
                    var chance = Rand.Range(0f, 1f);
                    if (chance <= 0.1f)
                    {
                        var root = PersonalityRoot.AllHuman.RandomElementExcept(gram.Root);
                        gram.Root = root;
                        gram.Variant = root.GetRandomVariant();
                        personalityTraitOld = gram.MainTrait;
                        gram.MainTrait = root.GetRandomCompatibleTrait(gram.Variant, gram.MainTrait);
                        personalityTraitNew = gram.MainTrait;
                    }
                    else if (chance <= 0.3f)
                    {
                        personalityTraitOld = gram.SecondaryTrait;
                        gram.SecondaryTrait = PersonalityRoot.AllHuman.RandomElement().GetRandomCompatibleTrait(gram.Variant, gram.SecondaryTrait);
                        personalityTraitNew = gram.SecondaryTrait;
                    }
                    else if (chance <= 0.6f)
                    {
                        tempHashset.Clear();
                        tempHashset.Add(gram.MainTrait);
                        tempHashset.Add(gram.SecondaryTrait);
                        tempHashset.Add(gram.OptionalTrait);

                        personalityTraitOld = gram.OptionalTrait;
                        gram.OptionalTrait = PersonalityRoot.AllHuman.RandomElementExcept(gram.Root).GetRandomCompatibleTrait(gram.Variant, tempHashset);
                        personalityTraitNew = gram.OptionalTrait;
                    }
                }

                if (personalityTraitNew != null)
                {
                    if (personalityTraitOld != null)
                    {
                        Find.LetterStack.ReceiveLetter(GetText(pawn, personalityTraitNew, personalityTraitOld, SPM2DefOf.SP_ReplacingTraitTitle),
                            GetText(pawn, personalityTraitNew, personalityTraitOld, SPM2DefOf.SP_ObtainingNewTraitText), LetterDefOf.NeutralEvent, pawn);
                    }
                    else
                    {
                        Find.LetterStack.ReceiveLetter(GetText(pawn, personalityTraitNew, personalityTraitOld, SPM2DefOf.SP_ObtainingNewTraitTitle),
                            GetText(pawn, personalityTraitNew, personalityTraitOld, SPM2DefOf.SP_ObtainingNewTraitText), LetterDefOf.NeutralEvent, pawn);
                    }
                    gameComp.pawnsByChangeTraitImmunityTick[pawn] = Find.TickManager.TicksGame + (Rand.Range(30, 120) * GenDate.TicksPerDay);
                }
            }
        }

        public static TaggedString GetText(Pawn pawn, PersonalityTrait trait, PersonalityTrait oldTrait, RulePackDef rule)
        {
            Rand.PushState();
            Rand.Seed = pawn.thingIDNumber;
            string rootKeyword = "root";
            GrammarRequest request = default(GrammarRequest);
            request.Rules.AddRange(GrammarUtility.RulesForPawn("PAWN", pawn, request.Constants));
            request.Rules.Add(new Rule_String("TRAIT", trait.label));
            if (oldTrait != null)
            {
                request.Rules.Add(new Rule_String("OLDTRAIT", oldTrait.label));
            }
            request.Includes.Add(rule);
            string str = GrammarResolver.Resolve(rootKeyword, request);
            Rand.PopState();
            return str;
        }
    }

    public class SimplePersonalitiesManagerM2 : GameComponent
    {
        public Dictionary<Pawn, int> pawnsByChangeTraitImmunityTick = new Dictionary<Pawn, int>();
        public SimplePersonalitiesManagerM2()
        {

        }
        public SimplePersonalitiesManagerM2(Game game)
        {

        }


        private void Init()
        {
            pawnsByChangeTraitImmunityTick ??= new Dictionary<Pawn, int>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pawnsByChangeTraitImmunityTick, "pawnsByLastChangedTraitsTick", LookMode.Reference, LookMode.Value, ref pawnKeys, ref intValues);
        }

        private List<Pawn> pawnKeys;
        private List<int> intValues;
    }
}
