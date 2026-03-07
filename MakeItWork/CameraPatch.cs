using UnityEngine;
using UnityEngine.InputSystem;

namespace MakeItWork
{
    internal static class CameraPatch
    {
        //DisableResetCamera
        internal static bool RCCP_Camera_Orbit_Pre(ref RCCP_Camera __instance)
        {
            __instance.orbitReset = !PluginConfig.DisableCameraReset.Value;
            return true;
        }

        //FollowVehicleAtAngle
        internal static bool RCCP_Camera_LateUpdate_Pre(ref RCCP_Camera __instance)
        {
            // FollowVehicleAtAngle
            __instance.tPSMode = PluginConfig.EnableFollowVehicleAtAngle.Value ?
                RCCP_Camera.TPSMode.TPS1 :
                RCCP_Camera.TPSMode.TPS2;

            return true;
        }

        // Zoom
        internal static void RCCP_Camera_Inputs_Post(ref RCCP_Camera __instance)
        {
            __instance.zoomScrollMultiplier = PluginConfig.ZoomScrollMultiplier.Value;

            float input = Mouse.current.scroll.value.y;
            if (input != 0f)
            {
                __instance.zoomScroll = Mathf.Clamp(
                    __instance.zoomScroll - input / Mathf.Abs(input) * __instance.zoomScrollMultiplier,
                    PluginConfig.MinZoomDistance.Value,
                    PluginConfig.MaxZoomDistance.Value);
                Logging.logger.LogDebug($"Zoom scroll = {__instance.zoomScroll}");
            }
        }
    }
}
