using System.IO;
using System.Reflection;
using Rage;
using EverydayCallouts.Info;


namespace EverydayCallouts.Logging
{
    public static class InitializeLogging
    {
        public static void PrintStartupInfo()
        {
            PrintPluginInfo();
            PrintDependencyInfo();
            PrintCalloutRegistrationStatus();
        }

        private static void PrintPluginInfo()
        {
            Game.Console.Print("EverydayCallouts: [PluginInfo]");
            Game.Console.Print($"EverydayCallouts: [PluginInfo] Version            : {PluginInfo.Version}");
            Game.Console.Print($"EverydayCallouts: [PluginInfo] Author             : {PluginInfo.Author}");
            Game.Console.Print("EverydayCallouts: [PluginInfo] -------------------------------------------------------------------------------------------");
        }

        private static void PrintDependencyInfo()
        {
            Game.Console.Print("EverydayCallouts: [Dependencies]");

            PrintDependency("CalloutInterfaceAPI.dll");
            PrintDependency("CommonDataFramework.dll");
            PrintDependency("RageNativeUI.dll");

            Game.Console.Print("EverydayCallouts: [Dependencies] -------------------------------------------------------------------------------------------");
        }

        private static void PrintDependency(string fileName, string expectedVersion = null)
        {
            string gameDir = Directory.GetCurrentDirectory();
            string path = Path.Combine(gameDir, fileName);

            string actualVersion = null;
            try
            {
                actualVersion = AssemblyName.GetAssemblyName(path).Version.ToString();
            }
            catch
            {
                actualVersion = "Unknown";
            }

            if (expectedVersion != null)
                Game.Console.Print($"EverydayCallouts: [Dependencies] {fileName,-22} : Available ({actualVersion})");
            else
                Game.Console.Print($"EverydayCallouts: [Dependencies] {fileName,-22} : Available");
        }

        private static void PrintCalloutRegistrationStatus()
        {
            Game.Console.Print("EverydayCallouts: [CalloutHandler]");
            Game.Console.Print("EverydayCallouts: [CalloutHandler] registering callouts...");
            Game.Console.Print("EverydayCallouts: [CalloutHandler] -> NoiseComplaint");
            Game.Console.Print("EverydayCallouts: [CalloutHandler] registration complete.");
        }
    }
}
