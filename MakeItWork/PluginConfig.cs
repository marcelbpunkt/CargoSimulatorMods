using BepInEx;
using BepInEx.Configuration;
using System.IO;
using static MakeItWork.Logging;

namespace MakeItWork
{
    public static class PluginConfig
    {
        private static readonly string _cfgFileName = PluginInfo.PLUGIN_GUID + ".cfg";
        
        private static readonly ConfigFile _cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, _cfgFileName), true);

        internal static ConfigEntry<bool> EnableUseUpAllSupplies {  get; set; }
        internal static ConfigEntry<bool> EnableAutoLights { get; set; }
        internal static ConfigEntry<int> AutoLightsHourEnabled { get; set; }

        internal static void Initialize()
        {
            EnableUseUpAllSupplies = _cfgFile.Bind(
                "General",
                "enableUseUpAllSupplies",
                true,
                "If enabled, only pushes notifications about low supplies when they are empty or insufficient, "
                    + "and causes cashiers to use them up completely before refilling."
            );
            EnableAutoLights = _cfgFile.Bind(
                "General",
                "enableAutoLights",
                true,
                "If enabled, automatically switches on store and warehouse lights at dusk."
            );
            ConfigDescription description = new ConfigDescription(
                "The hour of the time (24h format) at which the lights automatically switch on.",
                new AcceptableValueRange<int>(8, 21));
            AutoLightsHourEnabled = _cfgFile.Bind("General", "autoLightsHourEnabled", 18, description);

            SetupWatcher();
        }

        /**
         * <summary>Watches for config file changes and applies them to the mod while running the game.</summary>
         */
        private static void SetupWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Paths.ConfigPath, _cfgFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(_cfgFile.ConfigFilePath))
            {
                logger.LogWarning($"Config file {_cfgFile.ConfigFilePath} not found.");
                return;
            }

            try
            {
                _cfgFile.Reload();
            }
            catch
            {
                logger.LogError($"Error while loading configuration file {_cfgFileName}. Using defaults.");
            }
        }
        
        internal static void Save()
        {
            _cfgFile.Save();
        }
    }
}
