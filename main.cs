using Rage;
using LSPD_First_Response.Mod.API;
using EverydayCallouts.Callouts;


public class EntryPoint : Plugin
{
    public override void Initialize()
    {
        Game.Console.Print("[EverydayCallouts] Plugin initializing...");

        // Hook the OnDuty state change
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;

        Game.Console.Print("[EverydayCallouts] Plugin initialized.");
    }

    private void OnDutyStateChanged(bool onDuty)
    {
        if (onDuty)
        {
            Game.Console.Print("[EverydayCallouts] Player went ON DUTY.");
        }
        else
        {
            Game.Console.Print("[EverydayCallouts] Player went OFF DUTY.");
        }
    }

    public override void Finally()
    {
        Game.Console.Print("[EverydayCallouts] Plugin shutting down...");
    }

    private static void OnOnDutyStateChangedHandler(bool OnDuty)
    {
        if (OnDuty)
        {
            RegisterCallouts();
            Game.Console.Print("[EverydayCallouts] Player went ON DUTY.");
        }
        else
        {
            Game.Console.Print("[EverydayCallouts] Player went OFF DUTY.");
        }

    }

    private static void RegisterCallouts()
    {
        // Register your callouts here
        Functions.RegisterCallout(typeof(NoiseComplaint));

    }
}
