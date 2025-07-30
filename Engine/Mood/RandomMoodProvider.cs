using System;
using Rage;

namespace EverydayCallouts.Engine.Mood
{
    public class RandomMoodProvider : IMoodProvider
    {
        private static readonly Random rng = new Random();
        private static readonly PedMood[] Moods = (PedMood[])Enum.GetValues(typeof(PedMood));

        public PedMood GetPedMood(Ped ped)
        {
            return Moods[rng.Next(Moods.Length)];
        }
    }
}
