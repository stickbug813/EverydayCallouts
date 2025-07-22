using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverydayCallouts.Logging
{
    public static class InitializationErrors
    {
        public static string MissingDependencies(string missing) => $"[EverydayCallouts]: You are missing one or more required dependencies: {missing}";
    }
}
