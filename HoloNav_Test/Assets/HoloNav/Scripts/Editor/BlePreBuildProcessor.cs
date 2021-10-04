#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BlePreBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.WSAPlayer && !PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.Bluetooth))
        {
            EditorUtility.DisplayDialog("Warning", "WSA Bluetooth capability is required and is being enabled in the player settings.", "Accept");
            PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Bluetooth, true);
        }
    }
}
#endif