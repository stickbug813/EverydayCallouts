using System.Collections.Generic;
using System.IO;

namespace EverydayCallouts.Engine
{
    internal static class DependencyChecker
    {
        private static readonly string[] RequiredDlls =
        {
            "CalloutInterfaceAPI.dll",
            "CommonDataFramework.dll",
        };

        public static (bool success, List<string> missingDependencies) CheckAll()
        {
            string basePath = Directory.GetCurrentDirectory();
            var missing = new List<string>();

            foreach (var dll in RequiredDlls)
            {
                string fullPath = Path.Combine(basePath, dll);
                if (!File.Exists(fullPath))
                {
                    missing.Add(dll);
                }
            }

            bool allGood = missing.Count == 0;
            return (allGood, missing);
        }
    }
}
