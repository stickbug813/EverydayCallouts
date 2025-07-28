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
           Game.LogTrivial("EverydayCallouts: [PluginInfo]");
           Game.LogTrivial($"EverydayCallouts: [PluginInfo] Version            : {PluginInfo.Version}");
           Game.LogTrivial($"EverydayCallouts: [PluginInfo] Author             : {PluginInfo.Author}");
           Game.LogTrivial("EverydayCallouts: [PluginInfo] -------------------------------------------------------------------------------------------");
        }

        private static void PrintDependencyInfo()
        {
           Game.LogTrivial("EverydayCallouts: [Dependencies]");

            PrintDependency("CalloutInterfaceAPI.dll");
            PrintDependency("CommonDataFramework.dll");
            PrintDependency("RageNativeUI.dll");

           Game.LogTrivial("EverydayCallouts: [Dependencies] -------------------------------------------------------------------------------------------");
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
               Game.LogTrivial($"EverydayCallouts: [Dependencies] {fileName,-22} : Available ({actualVersion})");
            else
               Game.LogTrivial($"EverydayCallouts: [Dependencies] {fileName,-22} : Available");
        }

        private static void PrintCalloutRegistrationStatus()
        {
           Game.LogTrivial("EverydayCallouts: [CalloutHandler]");
           Game.LogTrivial("EverydayCallouts: [CalloutHandler] registering callouts...");
           Game.LogTrivial("EverydayCallouts: [CalloutHandler] -> NoiseComplaint");
           Game.LogTrivial("EverydayCallouts: [CalloutHandler] registration complete.");
        }
    }
}
