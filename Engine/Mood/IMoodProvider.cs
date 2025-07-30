using Rage;

namespace EverydayCallouts.Engine.Mood
{
    public interface IMoodProvider
    {
        PedMood GetPedMood(Ped ped);
    }
}
