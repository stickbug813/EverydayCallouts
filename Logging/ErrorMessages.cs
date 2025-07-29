using Rage;

namespace EverydayCallouts.Logging
{
    public static class InitializationErrors
    {
        public static void MissingDependencies(string missing)
        {
            string msg = $"[EverydayCallouts] You are missing one or more required dependencies: {missing}";
            Game.LogTrivial(msg);
        }

        public static void DependencyFound(string name, string version)
        {
            string msg = $"[EverydayCallouts] [Dependency] {name,-22} Found (v{version})";
            Game.LogTrivial(msg);
        }

        public static void DependencyTooOld(string name, string actualVersion, string requiredVersion)
        {
            string msg = $"[EverydayCallouts] [Dependency] {name,-22}  Version too old (v{actualVersion}, required v{requiredVersion})";
            Game.LogTrivial(msg);
        }

        public static void DependencyMissing(string name)
        {
            string msg = $"[EverydayCallouts] [Dependency] {name,-22} Missing";
            Game.LogTrivial(msg);
        }

        public static void DependencyInvalid(string name)
        {
            string msg = $"[EverydayCallouts] [Dependency] {name,-22} Invalid assembly format";
            Game.LogTrivial(msg);
        }
    }
}
