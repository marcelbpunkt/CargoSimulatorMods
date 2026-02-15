
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using static MakeItWork.Logging;

namespace MakeItWork
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MakeItWork : BaseUnityPlugin
    {
        ////////////////////
        // Initialization //
        ////////////////////
        
        private void Awake() {
            PluginConfig.Initialize();
            Harmony.CreateAndPatchAll(typeof(MakeItWork), PluginInfo.PLUGIN_NAME);
        }

        private void OnDestroy()
        {
            PluginConfig.Save();
        }

        
        
        

        //////////////////////////////
        // UseUpAllSupplies patches //
        //////////////////////////////

        [HarmonyPatch(typeof(DeskShelfController), nameof(DeskShelfController.AddOrResolveSupplyBoxNotificationIfNecessary))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> BoxesLowNotificationTrans(IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value) return instructions;
            return UseUpAllSuppliesPatch.BoxesLowNotificationTrans(instructions);
        }

        [HarmonyPatch(typeof(StickerInteractableController), nameof(StickerInteractableController.AddOrResolveLowStickerSupplyNotificationIfNecessary))]
        [HarmonyPrefix]
        internal static bool LabelsLowNotificationPre(ref StickerInteractableController __instance)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value) return true;
            return UseUpAllSuppliesPatch.LabelsLowNotificationPre(ref __instance);
        }

        [HarmonyPatch(typeof(TapeDispenserController), nameof(TapeDispenserController.AddNotificationOnStartIfNecessary))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> TapeLowNotificationStartTrans(IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value) return instructions;
            return UseUpAllSuppliesPatch.TapeLowNotificationStartTrans(instructions);
        }

        [HarmonyPatch(typeof(TapeDispenserController), nameof(TapeDispenserController.SuccessTaped))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> TapeLowNotificationSuccessTrans(IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value) return instructions;
            return UseUpAllSuppliesPatch.TapeLowNotificationSuccessTrans(instructions);
        }





        ////////////////////////
        // AutoLights patches //
        ////////////////////////

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnEnable))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> OnEnableTrans(IEnumerable<CodeInstruction> instructions)
        {
            // always subscribe all lightswitches to OnHourPassed
            return AutoLightsPatch.OnEnableTrans(instructions);
        }

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnHourPassed))]
        [HarmonyPrefix]
        internal static bool OnHourPassedPrefix(LightSwitchController __instance)
        {
            if (!PluginConfig.EnableAutoLights.Value) return true;
            return AutoLightsPatch.OnHourPassedPrefix(__instance);
        }
    }
}
