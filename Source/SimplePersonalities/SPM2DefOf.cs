using RimWorld;
using SPM1;
using Verse;

namespace SPM2
{
    [DefOf]
    public static class SPM2DefOf
    {
        static SPM2DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SPM2DefOf));
        }

        public static PersonalityDrive SP_Drive_Instincts;
        public static PersonalityDrive SP_Drive_Feelings;
        public static PersonalityDrive SP_Drive_Thinking;
        public static ThoughtDef SP_PassionateLovin;
        public static ThoughtDef SP_PassionateLovinOneNightStand;
    }
}
