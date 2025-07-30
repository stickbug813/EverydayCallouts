using Rage;
using EverydayCallouts.Engine.Mood;

namespace EverydayCallouts
{
    public static class DialogueConditions
    {
        public static Ped currentPed;

        public static bool IsMoodSad()
        {
            return IsMood(PedMood.Sad);
        }

        public static bool IsMoodScared()
        {
            return IsMood(PedMood.Scared);
        }

        public static bool IsMoodMad()
        {
            return IsMood(PedMood.Mad);
        }

        public static bool IsMoodNeutral()
        {
            return IsMood(PedMood.Neutral);
        }

        public static bool IsMoodHappy()
        {
            return IsMood(PedMood.Happy);
        }

        public static bool IsMoodSovereign()
        {
            return IsMood(PedMood.Sovereign);
        }

        private static bool IsMood(PedMood mood)
        {
            if (currentPed == null)
                return false;

            return PedMoodManager.GetStoredMood(currentPed) == mood;
        }
    }
}
