using System;
using System.Reflection;
using Rage;

namespace EverydayCallouts.Engine.Mood
{
    public class PRMoodProvider : IMoodProvider
    {
        private readonly MethodInfo getMoodMethod;
        private readonly Type moodEnumType;

        public PRMoodProvider()
        {
            var pedAPIType = Type.GetType("PolicingRedefined.API.PedAPI, PolicingRedefined");
            moodEnumType = Type.GetType("PolicingRedefined.Interaction.Assets.PedAttributes.EPedMood, PolicingRedefined");
            getMoodMethod = pedAPIType?.GetMethod("GetPedMood", new[] { typeof(Ped) });
        }

        public PedMood GetPedMood(Ped ped)
        {
            if (getMoodMethod == null || moodEnumType == null)
                return PedMood.Neutral;

            object mood = getMoodMethod.Invoke(null, new object[] { ped });
            return Enum.TryParse(mood.ToString(), out PedMood result) ? result : PedMood.Neutral;
        }
    }
}
