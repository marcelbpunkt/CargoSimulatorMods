using BepInEx;
using HarmonyLib;
using Player.Tablet;
using static CargoBoxController;

namespace MakeItWork
{
    /**
     * <summary>
     *     Changes the refill behaviour of the cashiers such that they only refill the boxes
     *     when they are completely empty.
     * </summary>
     */
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class UseUpAllBoxesPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(UseUpAllBoxesPlugin), PluginInfo.PLUGIN_NAME);
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
         *     Resets the temporarily raised box count so we don't end up with weird behaviour
         * </summary>
         * <param name="__instance">the instance containing the boxes in question</param>
         * <param name="boxType">the type of box that is to be checked</param>
         */
        [HarmonyPatch(typeof(DeskShelfController), "AddOrResolveSupplyBoxNotificationIfNecessary")]
        [HarmonyPostfix]
        static void OverrideBoxesLowNotificationPostfix(ref DeskShelfController __instance, CargoBoxController.BoxType boxType)
        {
            // reset the respective box count (i.e. lower it by 2) so we don't have perpetual box supplies
            __instance.cargoBoxHolders[(int)boxType]._boxCount -= 2;
        }
    }
}
