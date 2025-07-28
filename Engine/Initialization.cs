using EverydayCallouts.Logging;
using Rage;
using System;
using System.Collections.Generic;

namespace EverydayCallouts.Engine
{
    internal class Initialization
    {
        public static bool Startup()
        {

            var dependencies = new List<(string Name, string MinVersion)>
{
                ("CalloutInterfaceAPI.dll", "1.0.0.0"),
                ("CommonDataFramework.dll", "1.0.0.0"),
                ("RageNativeUI.dll", "1.8.0.0")
};

            var (success, missing) = DependencyChecker.CheckAll(dependencies);

            if (!success)
            {
                InitializationErrors.MissingDependencies(string.Join(", ", missing));

                return false;
            }

            return true;


        }


    }
}
