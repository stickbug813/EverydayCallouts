

using Rage;

namespace EverydayCallouts.Logging
{
    internal class CalloutWarnMessages
    {
        public static void PedDoesNotExist(){ Game.LogTrivial("[EverydayCallouts] Ped does not exist. Callout will now end."); }

        public static void FailedToCreatePed(){ Game.LogTrivial("[EverydayCallouts] Failed to create Ped. Callout will now end."); }

        

    }

    internal class CalloutInfoMessages
    {
        // GUI related messages
        public static void RNUIMenuAdded() { Game.LogTrivial("[EverydayCallouts] Menu added to RageNativeUI."); }

        public static void RNUIMenuRemoved() { Game.LogTrivial("[EverydayCallouts] Menu removed from RageNativeUI."); }

        public static void RNUIMenuOpening() { Game.LogTrivial("[EverydayCallouts] RageNativeUI menu is opening."); }

        public static void RNUIMenuClosing() { Game.LogTrivial("[EverydayCallouts] RageNativeUI menu is closing."); }


        // Entity related messages
        public static void CreatingMarker() { Game.LogTrivial("[EverydayCallouts] Callout marker is being created."); }

        public static void CalloutCleanedUp() { Game.LogTrivial("[EverydayCallouts] Callout has been cleaned up."); }

        // Ped related messages
        public static void PedCreated() { Game.LogTrivial("[EverydayCallouts] Ped has been created successfully."); }

    }
}
        