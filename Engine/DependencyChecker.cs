using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EverydayCallouts.Logging;
using Rage;

namespace EverydayCallouts.Engine
{
    internal static class DependencyChecker
    {
        public static bool IsAssemblyAvailable(string assemblyName, string minVersion)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string[] searchPaths =
            {
                baseDir,
                Path.Combine(baseDir, "Plugins"),
                Path.Combine(baseDir, "Plugins", "LSPDFR")
            };

            foreach (string path in searchPaths)
            {
                string fullPath = Path.Combine(path, assemblyName);

                if (!File.Exists(fullPath))
                    continue;

                try
                {
                    var assembly = AssemblyName.GetAssemblyName(fullPath);
                    Version actual = assembly.Version;
                    Version required = new Version(minVersion);

                    if (actual >= required)
                    {
                        InitializationErrors.DependencyFound(assemblyName, actual.ToString());
                        return true;
                    }

                    InitializationErrors.DependencyTooOld(assemblyName, actual.ToString(), required.ToString());
                    return false;
                }
                catch (BadImageFormatException)
                {
                    InitializationErrors.DependencyInvalid(assemblyName);
                    return false;
                }
            }

            InitializationErrors.DependencyMissing(assemblyName);
            return false;
        }

        public static (bool success, List<string> missingDependencies) CheckAll(List<(string Name, string MinVersion)> dependencies)
        {
            List<string> missing = new();

            foreach (var (name, version) in dependencies)
            {
                if (!IsAssemblyAvailable(name, version))
                {
                    missing.Add($"{name} (min {version})");
                }
            }

            if (missing.Count > 0)
                InitializationErrors.MissingDependencies(string.Join(", ", missing));

            return (missing.Count == 0, missing);
        }
    }
}
