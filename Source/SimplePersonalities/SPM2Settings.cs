using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SPM2
{
    public class SPM2Settings : ModSettings
    {
        public bool SPM2_Couples = true;
        public bool SPM2_OneNightStand = true;
        public bool SPM2_Caravans = true;
        public bool SPM2_ManyHandsMakeLightWork = true;
        public bool SPM2_Discord = true;
        public bool SPM2_Venting = true;
        public bool SPM2_Leaders = true;
        public bool SPM2_Trading = true;
        public bool SPM2_FriendlyFire = true;
        public bool SPM2_ObtainingNewCharacterTraits = true;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref SPM2_Couples, "SPM2_Couples", true);
            Scribe_Values.Look(ref SPM2_OneNightStand, "SPM2_OneNightStand", true);
            Scribe_Values.Look(ref SPM2_Caravans, "SPM2_Caravans", true);
            Scribe_Values.Look(ref SPM2_ManyHandsMakeLightWork, "SPM2_ManyHandsMakeLightWork", true);
            Scribe_Values.Look(ref SPM2_Discord, "SPM2_Discord", true);
            Scribe_Values.Look(ref SPM2_Venting, "SPM2_Venting", true);
            Scribe_Values.Look(ref SPM2_Leaders, "SPM2_Leaders", true);
            Scribe_Values.Look(ref SPM2_Trading, "SPM2_Trading", true);
            Scribe_Values.Look(ref SPM2_FriendlyFire, "SPM2_FriendlyFire", true);
            Scribe_Values.Look(ref SPM2_ObtainingNewCharacterTraits, "SPM2_ObtainingNewCharacterTraits", true);
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.CheckboxLabeled("SP.SPM2_Couples".Translate(), ref SPM2_Couples, "SP.SPM2_CouplesDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_OneNightStand".Translate(), ref SPM2_OneNightStand, "SP.SPM2_OneNightStandDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_Caravans".Translate(), ref SPM2_Caravans, "SP.SPM2_CaravansDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_ManyHandsMakeLightWork".Translate(), ref SPM2_ManyHandsMakeLightWork, "SP.SPM2_ManyHandsMakeLightWorkDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_Discord".Translate(), ref SPM2_Discord, "SP.SPM2_DiscordDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_Venting".Translate(), ref SPM2_Venting, "SP.SPM2_VentingDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_Leaders".Translate(), ref SPM2_Leaders, "SP.SPM2_LeadersDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_Trading".Translate(), ref SPM2_Trading, "SP.SPM2_TradingDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_FriendlyFire".Translate(), ref SPM2_FriendlyFire, "SP.SPM2_FriendlyFireDesc".Translate());
            listingStandard.CheckboxLabeled("SP.SPM2_ObtainingNewCharacterTraits".Translate(), ref SPM2_ObtainingNewCharacterTraits, "SP.SPM2_ObtainingNewCharacterTraitsDesc".Translate());
            listingStandard.End();
            base.Write();
        }

        private Vector2 scrollPosition = Vector2.zero;
    }
}
