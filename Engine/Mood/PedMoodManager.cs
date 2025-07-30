using System;
using Rage;

namespace EverydayCallouts.Engine.Mood
{
    public static class PedMoodManager
    {
        private static readonly IMoodProvider moodProvider = MoodProviderFactory.GetProvider();

        /// <summary>
        /// Assigns a mood to the ped, stores it for plugin, and sends it to PR (if installed).
        /// </summary>
        public static PedMood SetMood(Ped ped)
        {
            PedMood mood = moodProvider.GetPedMood(ped);
            StoreMoodForDialogue(ped, mood);
            TrySetPRPedMood(ped, mood);
            Game.LogTrivial($"Ped {ped.Handle} mood set to {mood}");
            return mood;
        }

        /// <summary>
        /// Logic to store mood for later use.
        /// </summary>
        private static void StoreMoodForDialogue(Ped ped, PedMood mood)
        {
            // Example: save to a dictionary
            DialogueMoodStore[ped] = mood;
        }

        /// <summary>
        /// If PR is installed, sets the mood using their API via reflection.
        /// </summary>
        private static void TrySetPRPedMood(Ped ped, PedMood mood)
        {
            try
            {
                var pedAPIType = Type.GetType("PolicingRedefined.API.PedAPI, PolicingRedefined");
                var moodEnumType = Type.GetType("PolicingRedefined.Interaction.Assets.PedAttributes.EPedMood, PolicingRedefined");

                if (pedAPIType == null || moodEnumType == null)
                    return;

                var setMoodMethod = pedAPIType.GetMethod("SetPedMood", new[] { typeof(Ped), moodEnumType });
                if (setMoodMethod == null)
                    return;

                object prMood = Enum.Parse(moodEnumType, mood.ToString());
                setMoodMethod.Invoke(null, new object[] { ped, prMood });
            }
            catch
            {

            }
        }

        // Example in-memory mood storage for your own plugin
        private static readonly System.Collections.Generic.Dictionary<Ped, PedMood> DialogueMoodStore = new();

        /// <summary>
        /// Retrieve the stored mood for your dialogue system.
        /// </summary>
        public static PedMood GetStoredMood(Ped ped)
        {
            return DialogueMoodStore.TryGetValue(ped, out var mood) ? mood : PedMood.Neutral;
        }
    }
}
