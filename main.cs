using System;
using Rage;
using LSPD_First_Response.Mod.API;
using EverydayCallouts.Callouts;


public class EntryPoint : Plugin
{
    public override void Initialize()
    {
        Game.Console.Print("[EverydayCallouts] Version 0.0.0.2 by Stickbug813 has been initialised.");

        // Hook the OnDuty state change
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;

        Game.Console.Print("[EverydayCallouts] Plugin initialized.");
    }

    private static void OnOnDutyStateChangedHandler(bool OnDuty)
    {
        if (OnDuty)
        {
            RegisterCallouts();
            Game.Console.Print("[EverydayCallouts] Player went ON DUTY.");
            Game.DisplayNotification("~g~Everyday Callouts~s~: Callouts are now available. Version 0.0.0.1");
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

    public override void Finally()
    {
        Game.Console.Print("[EverydayCallouts] Plugin shutting down...");
    }

}
