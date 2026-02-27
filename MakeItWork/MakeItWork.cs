
using BepInEx;
using HarmonyLib;
using System.Collections.Generic;

namespace MakeItWork
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MakeItWork : BaseUnityPlugin
    {
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
            PluginConfig.Save();
        }





        //////////////////////////////
        // UseUpAllSupplies patches //
        //////////////////////////////

        [HarmonyPatch(typeof(DeskShelfController),
            nameof(DeskShelfController.AddOrResolveSupplyBoxNotificationIfNecessary))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> BoxesAddOrResolveSupplyBoxNotificationIfNecessaryTrans(
            IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value)
                return instructions;
            return UseUpAllSuppliesPatch.BoxesAddOrResolveSupplyBoxNotificationIfNecessaryTrans(instructions);
        }

        [HarmonyPatch(typeof(StickerInteractableController),
            nameof(StickerInteractableController.AddOrResolveLowStickerSupplyNotificationIfNecessary))]
        [HarmonyPrefix]
        internal static bool LabelsAddOrResolveLowStickerSupplyNotificationIfNecessaryPre(
            ref StickerInteractableController __instance)
        {
            return !PluginConfig.EnableUseUpAllSupplies.Value ||
                UseUpAllSuppliesPatch.LabelsAddOrResolveLowStickerSupplyNotificationIfNecessaryPre(ref __instance);
        }

        [HarmonyPatch(typeof(TapeDispenserController),
            nameof(TapeDispenserController.AddNotificationOnStartIfNecessary))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> TapeAddNotificationOnStartIfNecessaryTrans(
            IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value)
                return instructions;
            return UseUpAllSuppliesPatch.TapeAddNotificationOnStartIfNecessaryTrans(instructions);
        }

        [HarmonyPatch(typeof(TapeDispenserController), nameof(TapeDispenserController.SuccessTaped))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> TapeSuccessTapedTrans(IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginConfig.EnableUseUpAllSupplies.Value)
                return instructions;
            return UseUpAllSuppliesPatch.TapeSuccessTapedTrans(instructions);
        }





        ////////////////////////
        // AutoLights patches //
        ////////////////////////

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnEnable))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> LightSwitchControllerOnEnableTrans(
            IEnumerable<CodeInstruction> instructions)
        {
            // always subscribe all lightswitches to OnHourPassed
            return AutoLightsPatch.OnEnableTrans(instructions);
        }

        [HarmonyPatch(typeof(LightSwitchController), nameof(LightSwitchController.OnHourPassed))]
        [HarmonyPrefix]
        internal static bool LightSwitchControllerOnHourPassedPre(ref LightSwitchController __instance)
        {
            return !PluginConfig.EnableAutoLights.Value ||
                AutoLightsPatch.OnHourPassedPre(ref __instance);
        }

        //////////////////
        // Camera patch //
        //////////////////

        // DisableResetCamera
        [HarmonyPatch(typeof(RCCP_Camera), nameof(RCCP_Camera.ORBIT))]
        [HarmonyPrefix]
        internal static bool RCCP_CameraOrbitPre(ref RCCP_Camera __instance)
        {
            return CameraPatch.RCCP_CameraOrbitPre(ref __instance);
        }

        ////////////////////////
        // SkipTutorial patch //
        ////////////////////////

        [HarmonyPatch(typeof(QuestGoal), nameof(QuestGoal.Init))]
        [HarmonyPostfix]
        internal static void QuestGoalInitPost(ref QuestGoal __instance)
        {
            if (!PluginConfig.SkipTutorials.Value)
                return;
            SkipTutorialPatch.QuestGoalInitPost(ref __instance);
        }

        //////////////////////////
        // RebalanceQueue patch //
        //////////////////////////

        [HarmonyPatch(typeof(DeskQueueController), nameof(DeskQueueController.DequeueCustomer))]
        [HarmonyPostfix]
        internal static void DeskQueueControllerDequeueCustomerPost(ref DeskQueueController __instance)
        {
            if (!PluginConfig.EnableRebalanceQueue.Value)
                return;
            RebalanceQueuePatch.DeskQueueControllerDequeueCustomerPost(ref __instance);
        }
    }
}
