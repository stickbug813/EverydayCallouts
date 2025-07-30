using EverydayCallouts.Info;
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
                ("RageNativeUI.dll", "1.8.0.0"),
                ("DialogueSystem.dll", "1.0.0.0")
            };

            var (success, missing) = DependencyChecker.CheckAll(dependencies);

            if (!success)
            {
                InitializationErrors.MissingDependencies(string.Join(", ", missing));

                return false;
            }
            InitializeLogging.PrintStartupInfo();

            Game.DisplayNotification(
                "CHAR_CALL911",                         // textureDictionaryName
                "CHAR_CALL911",                         // textureName
                "EverydayCallouts",                 // title (blue color tag)
                $"By ~b~{PluginInfo.Author}~s~", // subtitle
                $"Everyday Callouts ~g~loaded successfully~s~! Enjoy your patrol!\nVersion: ~g~{PluginInfo.Version}" // message
            );

            return true;


        }


    }
}
