using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static MakeItWork.Logging;

namespace MakeItWork
{
    internal static class CameraPatch
    {
        //DisableResetCamera
        internal static bool RCCP_CameraOrbitPre(ref RCCP_Camera __instance)
        {
            __instance.orbitReset = !PluginConfig.DisableCameraReset.Value;
            return true;
        }

        internal static IEnumerable<CodeInstruction> RCCP_CameraTPS2Trans(IEnumerable<CodeInstruction> instructions)
        {
            bool reacheduseOrbitInTPSCameraMode = false;
            bool reachedOrbitYSmoothed = false;
            bool reachedOrbitXSmoothed = false;
            bool modFinished = false;
            CodeInstruction loadEulerAngles2 = null;
            foreach (CodeInstruction i in instructions)
            {
                if (modFinished)
                {
                    logger.LogInfo("Mod finished.");
                    yield return i;
                    continue;
                }

                if (!reacheduseOrbitInTPSCameraMode)
                {
                    reacheduseOrbitInTPSCameraMode = i.opcode == OpCodes.Ldfld
                        && i.operand is not null
                        && ((FieldInfo)i.operand).Name.Equals(nameof(RCCP_Camera.useOrbitInTPSCameraMode));
                    yield return i;
                    continue;
                }

                logger.LogInfo("Reached 'if (useOrbitInTPSCameraMode)'.");


                if (!reachedOrbitYSmoothed)
                {
                    // get "load local variable 'eulerAngles2' instruction so we can re-use it
                    if (i.opcode == OpCodes.Ldloca_S)
                    {
                        loadEulerAngles2 = i.Clone();
                    }

                    reachedOrbitYSmoothed = i.opcode == OpCodes.Ldfld
                        && ((FieldInfo)i.operand).Name.Equals(nameof(RCCP_Camera.orbitY_Smoothed));
                    yield return i;
                    continue;
                }

                logger.LogInfo("Reached 'orbitY_Smoothed'.");
                yield return loadEulerAngles2;
                yield return CodeInstruction.LoadField(typeof(Vector3), nameof(Vector3.x));
                yield return new CodeInstruction(OpCodes.Add);

                if (!reachedOrbitXSmoothed)
                {
                    reachedOrbitXSmoothed = i.opcode == OpCodes.Ldfld
                        && ((FieldInfo)i.operand).Name.Equals(nameof(RCCP_Camera.orbitX_Smoothed));
                    yield return i;
                    continue;
                }

                logger.LogInfo("Reached 'orbitX_Smoothed'.");
                // change "eulerAngles2.y = orbitX_Smoothed" to "eulerAngles2.y += orbitX_Smoothed"
                yield return loadEulerAngles2;
                yield return CodeInstruction.LoadField(typeof(Vector3), nameof(Vector3.y));
                yield return new CodeInstruction(OpCodes.Add);
                modFinished = true;
            }
            /*
             * ldfld System.Boolean useOrbitInTPSCameraMode
brfalse System.Reflection.Emit.Label
ldarg.0
ldfld System.Single orbitY
ldc.r4 0
beq System.Reflection.Emit.Label
ldloca.s UnityEngine.Vector3 (6)
ldarg.0
ldfld System.Single orbitY_Smoothed
stfld System.Single x
ldarg.0
ldfld System.Single orbitX
ldc.r4 0
beq System.Reflection.Emit.Label
ldloca.s UnityEngine.Vector3 (6)
ldarg.0
ldfld System.Single orbitX_Smoothed
stfld System.Single y
ldarg.0
ldfld System.Single orbitX
call Single Abs(Single)
ldsfld System.Single Epsilon
bgt System.Reflection.Emit.Label
ldarg.0
ldfld System.Single orbitY
call Single Abs(Single)
ldsfld System.Single Epsilon
ble.un System.Reflection.Emit.Label
ldloca.s UnityEngine.Vector3 (6)
ldc.r4 0
stfld System.Single z
ldarg.0
ldfld System.Single orbitX
call Single Abs(Single)
ldsfld System.Single Epsilon
bgt System.Reflection.Emit.Label
ldarg.0
ldfld System.Single orbitY
call Single Abs(Single)
ldsfld System.Single Epsilon
ble.un System.Reflection.Emit.Label
ldarg.0
call UnityEngine.Transform get_transform()
ldloc.s UnityEngine.Vector3 (6)
call UnityEngine.Quaternion Euler(UnityEngine.Vector3)
callvirt Void set_rotation(UnityEngine.Quaternion)
ldarg.0
ldfld System.Boolean TPSFreeFall
             */
        }
    }
}
