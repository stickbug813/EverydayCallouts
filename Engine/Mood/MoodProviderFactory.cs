using System;

namespace EverydayCallouts.Engine.Mood
{
    public static class MoodProviderFactory
    {
        public static IMoodProvider GetProvider()
        {
            bool prAvailable = Type.GetType("PolicingRedefined.API.PedAPI, PolicingRedefined") != null;
            return prAvailable ? new PRMoodProvider() : new RandomMoodProvider();
        }
    }
}
