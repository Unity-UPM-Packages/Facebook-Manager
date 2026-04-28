using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class FBBuildPreProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Linking FB plugins");

        const string packagesFolderPath = "Packages/com.thelegends.facebook.sdk";
        const string packageLinkXmlPath = "Packages/com.thelegends.facebook.sdk/Runtime/FacebookSDK/link.xml";

        const string assetsFolderPath = "Assets/Plugins/FacebookSDK";
        const string destLinkXmlPath = "Assets/Plugins/FacebookSDK/link.xml";

        if (AssetDatabase.IsValidFolder(assetsFolderPath) && File.Exists(destLinkXmlPath))
        {
            Debug.Log("FB link file exists in Assets/Plugins/FacebookSDK folder.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(packagesFolderPath))
        {
            StopBuildWithMessage("FB packages folder not found.");
            return;
        }

        if (!File.Exists(packageLinkXmlPath))
        {
            StopBuildWithMessage($"FB link file not found at {packageLinkXmlPath}.");
            return;
        }

        if (!AssetDatabase.IsValidFolder("Assets/Plugins"))
        {
            AssetDatabase.CreateFolder("Assets", "Plugins");
        }

        if (!AssetDatabase.IsValidFolder(assetsFolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Plugins", "FacebookSDK");
        }

        // Thực hiện copy
        bool success = AssetDatabase.CopyAsset(packageLinkXmlPath, destLinkXmlPath);
        if (success)
        {
            Debug.Log("Copied FB link file from Packages to Assets/Plugins/FacebookSDK folder.");
        }
        else
        {
            StopBuildWithMessage("Failed to copy link.xml");
        }
    }

    private void StopBuildWithMessage(string message)
    {
        var prefix = "[FB] ";
        throw new BuildPlayerWindow.BuildMethodException(prefix + message);
    }
}
