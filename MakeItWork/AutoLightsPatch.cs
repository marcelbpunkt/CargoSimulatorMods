using Enviro;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MakeItWork
{
    /**
     * <summary>
     *     Automatically switches the lights on at dusk.
     * </summary>
     */
    public class AutoLightsPatch
    {
        internal static IEnumerable<CodeInstruction> OnEnableTrans(IEnumerable<CodeInstruction> instructions)
        {
            // remove "if (_isMainLight)" check so all lights are subscribed to OnHourPassed
            bool isMainLightChecked = false;

            foreach (var instruction in instructions)
            {
                if (isMainLightChecked)
                {
                    yield return instruction;
                    continue;
                }
                isMainLightChecked = instruction.opcode == OpCodes.Brfalse;
            }
        }

        internal static bool OnHourPassedPre(LightSwitchController __instance)
        {
            EnviroTimeModule currentTime = ((EnviroManagerBase)EnviroManager.instance).Time;
            // the actual time should always be 2-3 seconds before the full hour but just to be safe, round up from 30 minutes
            int hours = currentTime.minutes >= 30 ? currentTime.hours + 1 : currentTime.hours;
            if (hours >= PluginConfig.AutoLightsSwitchOnHour.Value && !__instance._isOn)
            {
                __instance.LightSwitch();
            }

            return true;
        }
    }
}
