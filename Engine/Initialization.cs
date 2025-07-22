using System;
using EverydayCallouts.Logging;
using Rage;

namespace EverydayCallouts.Engine
{
    internal class Initialization
    {
        public static bool Startup()
        {

            var (success, missing) = DependencyChecker.CheckAll();

            if (!success)
            {
                string error = InitializationErrors.MissingDependencies(string.Join(", ", missing));

                return false;
            }

            return true;
        }
    }
}
