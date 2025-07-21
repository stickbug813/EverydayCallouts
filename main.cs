using System;
using Rage;
using LSPD_First_Response.Mod.API;
using EverydayCallouts.Callouts;
using EverydayCallouts.Logging;
using EverydayCallouts.Engine;


public class EntryPoint : Plugin
{
    public override void Initialize()
    {
        bool success = Initialization.Initialize();

        if (!success)
        {
            return;
        }

        // Only runs if initialization succeeded
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
    }


    private static void OnOnDutyStateChangedHandler(bool OnDuty)
    {
        if (OnDuty)
        {
            RegisterCallouts();
        }
        else
        {
            return;
        }

    }

    private static void RegisterCallouts()
    {
        Functions.RegisterCallout(typeof(NoiseComplaint));
    }

    public override void Finally()
    {

    }

}
