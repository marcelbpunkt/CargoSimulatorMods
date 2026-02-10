using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static CargoBoxController;

namespace MakeItWork
{
    /**
     * <summary>
     *     Changes the refill behaviour of the cashiers such that they only refill the boxes
     *     and labels when they are completely empty. The notifications will also only pop up
     *     when these supplies are completely used up.
     *     Standard and fragile tape are excluded from this mod because you need to refill it earlier than at 0%.
     *     
     *     TODO change notification text from "low" to "empty", preferably in all supported languages (help?)
     *     TODO make trigger point configurable, or anything configurable really
     * </summary>
     */
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class UseUpAllSuppliesPlugin : BaseUnityPlugin
    {
        public static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_NAME);

        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(UseUpAllSuppliesPlugin), PluginInfo.PLUGIN_NAME);
            UnityEngine.Debug.unityLogger.logEnabled = false;
        }

        /**
         * <summary>
         *     Suppresses the notification trigger when there are one or two boxes of a given type left in the tray by
         *     temporarily increasing the stack size by 2.
         * </summary>
         * <param name="__instance">the instance containing the boxes in question</param>
         * <param name="boxType">the type of box that is to be checked</param>
         * <returns>Always returns true.</returns>
         */
        [HarmonyPatch(typeof(DeskShelfController), "AddOrResolveSupplyBoxNotificationIfNecessary")]
        [HarmonyPrefix]
        static bool OverrideBoxesLowNotificationPrefix(ref DeskShelfController __instance, CargoBoxController.BoxType boxType)
        {
            if (boxType == CargoBoxController.BoxType.None)
            {
                return true;
            }

            /* 
             * Honestly, this is probably the dirtiest way possible but they hardcoded the notifications trigger box count to 3,
             * so no other quick way to control the triggerification.
             * Also I chose incrementing the private member over using CargoBoxHolderController.AddBoxCount() since that would
             * trigger a visual update and also be less performant.
             */
            __instance.cargoBoxHolders[(int)boxType]._boxCount += 2;

            return true;
        }

        /**
         * <summary>
         *     Resets the temporarily raised box count.
         * </summary>
         * <param name="__instance">the instance containing the boxes in question</param>
         * <param name="boxType">the type of box that is to be checked</param>
         */
        [HarmonyPatch(typeof(DeskShelfController), "AddOrResolveSupplyBoxNotificationIfNecessary")]
        [HarmonyPostfix]
        static void OverrideBoxesLowNotificationPostfix(ref DeskShelfController __instance, CargoBoxController.BoxType boxType)
        {
            __instance.cargoBoxHolders[(int)boxType]._boxCount -= 2;
        }





        /**
         * <summary>
         *     Suppresses the notification trigger when the label printer has enough charge left to print at least
         *     one label by temporarily raising the charge just enough for the notification not to trigger.
         * </summary>
         * <param name="__instance">the instance containing the label printer in question</param>
         * <returns>Always returns true.</returns>
         */
        [HarmonyPatch(typeof(StickerInteractableController), "AddOrResolveLowStickerSupplyNotificationIfNecessary")]
        [HarmonyPrefix]
        static bool OverrideLabelsLowNotificationPrefix(ref StickerInteractableController __instance)
        {
            // check is done every time BEFORE a label is printed
            __instance._currentChargeAmount += 20 - 2 * __instance._currentUsageChargeAmount;

            return true;
        }

        /**
         * <summary>
         *     Resets the temporarily raised label charge.
         * </summary>
         * <param name="__instance">the instance containing the label printer in question</param>
         */
        [HarmonyPatch(typeof(StickerInteractableController), "AddOrResolveLowStickerSupplyNotificationIfNecessary")]
        [HarmonyPostfix]
        static void OverrideLabelsLowNotificationPostfix(ref StickerInteractableController __instance)
        {
            __instance._currentChargeAmount -= 20 - 2 * __instance._currentUsageChargeAmount;
        }
    }
}
