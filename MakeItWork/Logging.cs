
using BepInEx.Logging;

namespace MakeItWork
{
    internal static class Logging
    {
        public static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_NAME);
    }
}
