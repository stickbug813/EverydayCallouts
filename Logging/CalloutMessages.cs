

using Rage;

namespace EverydayCallouts.Logging
{
    internal class CalloutWarnMessages
    {
        public static void PedDoesNotExist(){ Game.LogTrivial("[EverydayCallouts]: Ped does not exist. Callout will now end."); }

    }

    internal class CalloutInfoMessages
    {
        public static void RNUIMenuAdded() { Game.LogTrivial("[EverydayCallouts]: Menu added to RageNativeUI."); }

    }
}
        