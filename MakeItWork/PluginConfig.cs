using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace MakeItWork
{
    public static class PluginConfig
    {
        private static readonly string _cfgFileName = PluginInfo.PLUGIN_GUID + ".cfg";

        private static readonly ConfigFile _cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, _cfgFileName), true);

        // private static readonly FileSystemWatcher watcher = new FileSystemWatcher(Paths.ConfigPath, _cfgFileName);

        internal static ConfigEntry<bool> EnableUseUpAllSupplies { get; set; }
        internal static ConfigEntry<bool> EnableAutoLights { get; set; }
        internal static ConfigEntry<int> AutoLightsSwitchOnHour { get; set; }
        internal static ConfigEntry<bool> EnableRebalanceQueue { get; set; }
        internal static ConfigEntry<bool> SkipTutorials { get; set; }
        internal static ConfigEntry<bool> DisableCameraReset { get; set; }
        internal static ConfigEntry<bool> EnableFollowVehicleAtAngle { get; set; }
        internal static ConfigEntry<bool> EnableZoomScroll { get; set; }
        internal static ConfigEntry<float> ZoomScrollMultiplier { get; set; }
        internal static ConfigEntry<float> MinZoomDistance { get; set; }
        internal static ConfigEntry<float> MaxZoomDistance { get; set; }
        internal static ConfigEntry<bool> EnableMinVehicleRequirement { get; set; }

        internal static void Initialize()
        {
            // General

            EnableUseUpAllSupplies = _cfgFile.Bind(
                "General",
                "EnableUseUpAllSupplies",
                true,
                "If enabled, only pushes notifications about low supplies when they are empty or insufficient, "
                    + "and causes cashiers to use them up completely before refilling."
            );
            EnableAutoLights = _cfgFile.Bind(
                "General",
                "EnableAutoLights",
                true,
                "If enabled, automatically switches on store and warehouse lights at dusk."
            );
            ConfigDescription description = new ConfigDescription(
                "The hour of the time (24h format) at which the lights automatically switch on.",
                new AcceptableValueRange<int>(8, 21));
            AutoLightsSwitchOnHour = _cfgFile.Bind(
                "General",
                "AutoLightsSwitchOnHour",
                18,
                description);
            SkipTutorials = _cfgFile.Bind(
                "General",
                "SkipTutorial",
                true,
                "Marks all tutorials as solved so they will not appear on screen.");
            EnableRebalanceQueue = _cfgFile.Bind(
                "General",
                "EnableRebalanceQueue",
                true,
                "When set to true, customers that are already queuing move to shorter queues if possible.");

            // Camera

            DisableCameraReset = _cfgFile.Bind(
                "Camera",
                "DisableCameraReset",
                true,
                "When set to true, the car camera (both first and third person view) "
                    + "does not 'snap back' after mouse-looking for 2s.");
            EnableFollowVehicleAtAngle = _cfgFile.Bind(
                "Camera",
                "EnableFollowVehicleAtAngle",
                true,
                "When set to true, the chase cam follows the vehicle and keeps the relative angle to the vehicle.");
            EnableZoomScroll = _cfgFile.Bind(
                "Camera",
                "EnableZoomScroll",
                true,
                "Toggles chase cam zoom functionality via mouse wheel.");
            ZoomScrollMultiplier = _cfgFile.Bind(
                "Camera",
                "ZoomScrollMultiplier",
                1.0f,
                new ConfigDescription(
                    "The zoom rate of the chase cam per mouse scroll input.",
                    new AcceptableValueRange<float>(0.5f, 10f)));
            MinZoomDistance = _cfgFile.Bind(
                "Camera",
                "MinZoomDistance",
                -2.0f,
                new ConfigDescription(
                    "The lower the number, the closer you can zoom in on the vehicle.",
                    new AcceptableValueRange<float>(-10.0f, 10.0f)));
            MaxZoomDistance = _cfgFile.Bind(
                "Camera",
                "MaxZoomDistance",
                10.0f,
                new ConfigDescription(
                    "The lower the number, the closer you can zoom in on the vehicle.",
                    new AcceptableValueRange<float>(0.0f, 50.0f)));

            EnableMinVehicleRequirement = _cfgFile.Bind(
                "Partnerships",
                "EnableMinVehicleRequirement",
                true,
                "If enabled, changes the vehicle requirements for partnerships from 'needs exactly this vehicle' to " +
                "'needs this or a bigger vehicle'.");

            // FileSystemWatcher does not seem to fire events
            // SetupWatcher();
        }

        /**
         * <summary>Watches for config file changes and applies them to the mod while running the game.</summary>
         */
        //private static void SetupWatcher()
        //{
        //    watcher.Changed += ReadConfigValues;
        //    watcher.Created += ReadConfigValues;
        //    watcher.Renamed += ReadConfigValues;
        //    watcher.IncludeSubdirectories = true;
        //    watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        //    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
        //    watcher.EnableRaisingEvents = true;
        //}

        //private static void ReadConfigValues(object sender, FileSystemEventArgs e)
        //{
        //    logger.LogDebug("PluginConfig: Reading config values.");
        //    if (!File.Exists(_cfgFile.ConfigFilePath))
        //    {
        //        logger.LogWarning($"Config file {_cfgFile.ConfigFilePath} not found.");
        //        return;
        //    }

        //    try
        //    {
        //        _cfgFile.Reload();
        //    }
        //    catch
        //    {
        //        logger.LogError($"Error while loading configuration file {_cfgFileName}. Using defaults.");
        //    }
        //}

        //internal static void Save()
        //{
        //    _cfgFile.Save();
        //}
    }
}
