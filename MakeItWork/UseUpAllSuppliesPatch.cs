using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static MakeItWork.Logging;

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
     * </summary>
     */
    internal static class UseUpAllSuppliesPatch
    {
        internal static IEnumerable<CodeInstruction> BoxesLowNotificationTrans(IEnumerable<CodeInstruction> instructions)
        {
            // We manipulate the next instruction after CargoBoxHolderController.GetBoxCount() is called
            // then the "< 3" comparison is changed to "< 1" so the respective box type needs to be completely
            // empty to trigger the notification.
            // This is technically not necessary since the ldc.i4.3 ops are only used for the "< 3" comparison
            // but in case more box types are added in the future this safer approach was chosen.
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            bool isGetBoxCount = false;
            for (int i = 0;  i < list.Count; i++)
            {
                if (isGetBoxCount)
                {
                    list[i].opcode = OpCodes.Ldc_I4_1;
                    list[i].operand = null;
                    isGetBoxCount = false;
                }
                else
                {
                    isGetBoxCount = list[i].opcode == OpCodes.Callvirt &&
                        ((MethodInfo)list[i].operand).Name.Equals(nameof(CargoBoxHolderController.GetBoxCount));
                }

            }

            // TODO try yield return
            return list.AsEnumerable();
        }

        internal static bool LabelsLowNotificationPre(ref StickerInteractableController __instance)
        {
            bool isLow = false;
            switch (__instance._deskController._currentDeskState)
            {
                // player or cashier wants to print a label so immediately before that the charge is checked,
                // so if there is exactly one label (4%) left, we push a notification so the player or cashier
                // can refill it afterwards.
                case DeskController.DeskState.CreateSticker:
                case DeskController.DeskState.StickerPlacement:
                    isLow = __instance._currentChargeAmount <= __instance._currentUsageChargeAmount;
                    break;
                // label charge check outside of currently printing labels is different since no label is used,
                // so if there is exactly one label (4%) left, no refill is needed at this point
                default:
                    isLow = __instance._currentChargeAmount < __instance._currentUsageChargeAmount;
                    break;
            }

            if (isLow)
            {
                MonoSingleton<NotificationManager>.Instance.AddLowStickerSupplySupplyNotification(((Component)__instance._deskController).gameObject);
            }
            else
            {
                MonoSingleton<NotificationManager>.Instance.ResolveLowStickerSupplySupplyNotification(((Component)__instance._deskController).gameObject);
            }

            return false;
        }

        internal static IEnumerable<CodeInstruction> TapeLowNotificationStartTrans(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            for (int i = 0; i < list.Count; i++)
            {
                // We want to change the hard-coded minimum amount of charge left to not trigger a notification from 20f to 10f
                // (as found in "_tapeChargeAmount < 20f" and "_fragileTapeChargeAmount < 20f")
                // for both occurrences types of tape since the largest box needs a charge of 10% or more
                // to get taped. So only push a notification if we have less than that.
                // Also no need to check for "is float" since this is implied by the OpCode being OpCodes.Ldc_R4.
                if (list[i].opcode == OpCodes.Ldc_R4 && (float)list[i].operand == 20f)
                {
                    list[i].operand = 10f;
                }
            }

            return list.AsEnumerable();
        }

        internal static IEnumerable<CodeInstruction> TapeLowNotificationSuccessTrans(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction> (instructions);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldc_R4 && (float)list[i].operand == 20f) list[i].operand = 10f;
            }

            return list.AsEnumerable();
        }
    }
}
