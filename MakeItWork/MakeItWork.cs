using BepInEx;
using HarmonyLib;
using System.Collections.Generic;

namespace MakeItWork
{

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MakeItWork : BaseUnityPlugin
    {
        private static bool oneTimeMessageDisplayed = false;

        ////////////////////
        // Initialization //
        ////////////////////

        private void Awake()
        {
            PluginConfig.Initialize();
            Harmony.CreateAndPatchAll(typeof(MakeItWork), PluginInfo.PLUGIN_NAME);
        }

        private void OnDestroy()
        {
            // not necessary since config manager does not work in this game
            // PluginConfig.Save();
        }





        //////////////////////////////
        // UseUpAllSupplies patches //
        //////////////////////////////

        [HarmonyPatch(typeof(DeskShelfController),
            nameof(DeskShelfController.AddOrResolveSupplyBoxNotificationIfNecessary))]
        [HarmonyTranspiler]
        // Boxes
        internal static IEnumerable<CodeInstruction>
            DeskShelfController_AddOrResolveSupplyBoxNotificationIfNecessary_Trans(
            IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value)
                return instructions;
            return UseUpAllSuppliesPatch.DeskShelfController_AddOrResolveSupplyBoxNotificationIfNecessary_Trans(
                instructions);
        }

        [HarmonyPatch(typeof(StickerInteractableController),
            nameof(StickerInteractableController.AddOrResolveLowStickerSupplyNotificationIfNecessary))]
        [HarmonyPrefix]
        // Labels
        internal static bool StickerInteractableController_AddOrResolveLowStickerSupplyNotificationIfNecessary_Pre(
            ref StickerInteractableController __instance)
        {
            return !PluginConfig.EnableUseUpAllSupplies.Value ||
                UseUpAllSuppliesPatch
                .StickerInteractableController_AddOrResolveLowStickerSupplyNotificationIfNecessary_Pre(ref __instance);
        }

        [HarmonyPatch(typeof(TapeDispenserController),
            nameof(TapeDispenserController.AddNotificationOnStartIfNecessary))]
        [HarmonyTranspiler]
        // Standard and Fragile Tape
        internal static IEnumerable<CodeInstruction> TapeDispenserController_AddNotificationOnStartIfNecessary_Trans(
            IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value)
                return instructions;
            return UseUpAllSuppliesPatch.TapeDispenserController_AddNotificationOnStartIfNecessary_Trans(instructions);
        }

        [HarmonyPatch(typeof(TapeDispenserController), nameof(TapeDispenserController.SuccessTaped))]
        [HarmonyTranspiler]
        // Standard and Fragile Tape
        internal static IEnumerable<CodeInstruction> TapeDispenserController_SuccessTaped_Trans(
            IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value)
                return instructions;
            return UseUpAllSuppliesPatch.TapeDispenserController_SuccessTaped_Trans(instructions);
        }





        ////////////////////////
        // AutoLights patches //
        ////////////////////////

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnEnable))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> LightSwitchController_OnEnable_Trans(
            IEnumerable<CodeInstruction> instructions)
        {
            // always subscribe all lightswitches to OnHourPassed
            return AutoLightsPatch.LightSwitchController_OnEnable_Trans(instructions);
        }

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnHourPassed))]
        [HarmonyPrefix]
        internal static bool LightSwitchController_OnHourPassed_Pre(ref LightSwitchController __instance)
        {
            return !PluginConfig.EnableAutoLights.Value ||
                AutoLightsPatch.LightSwitchController_OnHourPassed_Pre(ref __instance);
        }

        ////////////////////////
        // SkipTutorial patch //
        ////////////////////////

        [HarmonyPatch(typeof(QuestGoal), nameof(QuestGoal.Init))]
        [HarmonyPostfix]
        internal static void QuestGoal_Init_Post(ref QuestGoal __instance)
        {
            if (!PluginConfig.SkipTutorials.Value)
                return;
            SkipTutorialPatch.QuestGoal_Init_Post(ref __instance);
        }

        //////////////////////////
        // RebalanceQueue patch //
        //////////////////////////

        [HarmonyPatch(typeof(DeskQueueController), nameof(DeskQueueController.DequeueCustomer))]
        [HarmonyPostfix]
        internal static void DeskQueueController_DequeueCustomer_Post(ref DeskQueueController __instance)
        {
            if (!PluginConfig.EnableRebalanceQueue.Value)
                return;
            RebalanceQueuePatch.DeskQueueController_DequeueCustomer_Post(ref __instance);
        }

        //////////////////
        // Camera patch //
        //////////////////

        // DisableResetCamera
        [HarmonyPatch(typeof(RCCP_Camera), nameof(RCCP_Camera.ORBIT))]
        [HarmonyPrefix]
        internal static bool RCCP_Camera_Orbit_Pre(ref RCCP_Camera __instance)
        {
            return CameraPatch.RCCP_Camera_Orbit_Pre(ref __instance);
        }

        // FollowVehicleAtAngle
        [HarmonyPatch(typeof(RCCP_Camera), nameof(RCCP_Camera.LateUpdate))]
        [HarmonyPrefix]
        internal static bool RCCP_Camera_LateUpdate_Pre(ref RCCP_Camera __instance)
        {
            return CameraPatch.RCCP_Camera_LateUpdate_Pre(ref __instance);
        }

        // Zoom
        [HarmonyPatch(typeof(RCCP_Camera), nameof(RCCP_Camera.Inputs))]
        [HarmonyPrefix]
        internal static void RCCP_Camera_Inputs_Post(ref RCCP_Camera __instance)
        {
            if (!PluginConfig.EnableZoomScroll.Value)
            {
                __instance.zoomScroll = 0f;
                __instance.zoomScrollMultiplier = (float)PluginConfig.ZoomScrollMultiplier.DefaultValue;
                return;
            }

            CameraPatch.RCCP_Camera_Inputs_Post(ref __instance);
        }

        //////////////////////////
        // Partnerships patches //
        //////////////////////////

        // MinVehicleRequirement
        [HarmonyPatch(typeof(VehicleManager), nameof(VehicleManager.HasAllRequiredVehicles))]
        [HarmonyPrefix]
        internal static bool VehicleManager_HasAllRequiredVehicles_Pre(ref VehicleManager __instance)
        {
            return !PluginConfig.EnableMinVehicleRequirement.Value ||
                PartnershipsPatch.VehicleManager_HasAllRequiredVehicles_Pre(ref __instance);
        }
    }
}
