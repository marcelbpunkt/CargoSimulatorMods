namespace MakeItWork
{
    internal class CameraPatch
    {
        //DisableResetCamera
        internal static bool RCCP_CameraOrbitPre(RCCP_Camera __instance)
        {
            __instance.orbitReset = !PluginConfig.DisableCameraReset.Value;
            return true;
        }
    }
}
