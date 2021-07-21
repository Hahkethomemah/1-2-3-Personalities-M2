using System.Collections.Generic;
using System.Linq;
using SPM1;
using SPM1.Comps;
using Verse;

namespace SPM2
{
    public static class PersonalityComparer
    {
        public static PersonalityInteraction Compare(Pawn a, Pawn b)
        {
            return Compare(a?.TryGetEnneagramComp(), b?.TryGetEnneagramComp());
        }

        public static PersonalityInteraction Compare(CompEnneagram a, CompEnneagram b)
        {
            if (a == null || b == null || a.Enneagram == null || b.Enneagram == null || !a.Enneagram.IsValid || !b.Enneagram.IsValid)
                return PersonalityInteraction.Undefined;

            var gramA = a.Enneagram;
            var gramB = b.Enneagram;

            // Same drives trigger harmonious.
            bool sameDrive = gramA.Root.drive == gramB.Root.drive;
            if (sameDrive)
                return PersonalityInteraction.Harmonious;

            // Otherwise they are diversive.
            return PersonalityInteraction.Diversive;
        }

        public static PersonalityInteraction Compare(IEnumerable<Pawn> pawns)
        {
            return Compare(pawns.Select(p => p?.TryGetEnneagramComp()));
        }

        public static PersonalityInteraction Compare(IEnumerable<CompEnneagram> grams)
        {
            if (grams == null)
                return PersonalityInteraction.Undefined;

            int count = grams.Count();

            // Cannot compare to self!
            if (count < 2)
                return PersonalityInteraction.Undefined;

            // Use default compare.
            if (count == 2)
                return Compare(grams.ElementAt(0), grams.ElementAt(1));

            PersonalityDrive firstDrive = null;
            bool allAreSame = true;

            int feelCount = 0;
            int instCount = 0;
            int thinkCount = 0;

            foreach (var comp in grams)
            {
                var gram = comp?.Enneagram;
                if (gram == null || !gram.IsValid)
                    continue;

                var drive = gram.Root.drive;

                if (firstDrive == null)
                    firstDrive = drive;

                if (drive != firstDrive)
                    allAreSame = false;

                if (drive == SPM2DefOf.SP_Drive_Feelings)
                    feelCount++;
                if (drive == SPM2DefOf.SP_Drive_Instincts)
                    instCount++;
                if (drive == SPM2DefOf.SP_Drive_Thinking)
                    thinkCount++;

                if (instCount > 0 && thinkCount > 0 && feelCount > 0)
                    return PersonalityInteraction.Turmoil;
            }

            Core.Log($"{feelCount} {instCount} {thinkCount}");

            return allAreSame ? PersonalityInteraction.Harmonious : PersonalityInteraction.Diversive;
        }
    }
}
