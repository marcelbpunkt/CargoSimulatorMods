using BepInEx;
using Enviro;
using HarmonyLib;
using static Enviro.EnviroManager;
using static MakeItWork.Logging;

namespace MakeItWork
{
    /**
     * <summary>
     *     Automatically switches the lights on at dusk.
     *     TODO make configurable
     * </summary>
     */
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class AutoLightsPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(AutoLightsPlugin), PluginInfo.PLUGIN_NAME);
        }

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnEnable))]
        [HarmonyPrefix]
        internal static bool OnEnablePrefix(LightSwitchController __instance)
        {
            // complements the "if (_isMainLight)" condition so all switches are subscribed
            if (!__instance._isMainLight)
            {
                EnviroManager.instance.OnHourPassed += __instance.OnHourPassed;
            }

            return true;
        }

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnHourPassed))]
        [HarmonyPrefix]
        internal static bool AutoEnableLightsPrefix(LightSwitchController __instance) 
        {
            EnviroTimeModule currentTime = ((EnviroManagerBase)EnviroManager.instance).Time;
            // the actual time should always be 2-3 seconds before the full hour but just to be safe, round up from 30 minutes
            int hours = currentTime.minutes >=30 ? currentTime.hours + 1 : currentTime.hours;
            if (hours >= 18 && !__instance._isOn)
            {
                __instance.LightSwitch();
            }

            return true;
        }
    }
}
