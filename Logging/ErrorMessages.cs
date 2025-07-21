using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverydayCallouts.Logging
{
    internal class ErrorMessages
    {
    }

    public static class InitializationErrors
    {
        public static string MissingDependencies(string missing) => $"You are missing one or more required dependencies: {missing}";
    }
}
