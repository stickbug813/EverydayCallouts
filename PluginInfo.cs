using System.Reflection;

namespace EverydayCallouts.Info
{
    public static class PluginInfo
    {
        public static readonly string Title;
        public static readonly string Version;
        public static readonly string Author;
        public static readonly string Description;

        static PluginInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();

            Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "Unknown Plugin";
            Version = assembly.GetName().Version?.ToString() ?? "Unknown Version";
            Author = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown Author";
            Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Unknown Description";
        }
    }
}
