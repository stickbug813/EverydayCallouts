using LSPD_First_Response.Mod.API;
using EverydayCallouts.Callouts;
using EverydayCallouts.Logging;
using EverydayCallouts.Engine;

public class EntryPoint : Plugin
{
    public override void Initialize()
    {
        bool success = Initialization.Startup();

        if (!success)
        {
            return;
        }

        InitializeLogging.PrintStartupInfo();
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
