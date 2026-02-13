using BepInEx;
using BepInEx.Configuration;

namespace MakeItWork
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class PluginConfig : BaseUnityPlugin
    {
        internal static ConfigEntry<bool> enableUseUpAllSupplies;
        internal static ConfigEntry<bool> enableAutoLights;
        
        private void Awake()
        {
            enableUseUpAllSupplies = Config.Bind(
                "General",
                "enableUseUpAllSupplies",
                true,
                "If enabled, only pushes notifications about low supplies when they are empty or insufficient, "
                    + "and causes cashiers to use them up completely before refilling."
            );

            enableAutoLights = Config.Bind(
                "General",
                "enableAutoLights",
                true,
                "If enabled, automatically switches on store and warehouse lights at dusk."
            );
        }
    }
}
