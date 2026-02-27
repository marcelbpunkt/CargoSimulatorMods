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
    }
}
