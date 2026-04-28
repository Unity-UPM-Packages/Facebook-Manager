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

        const string packageLinkXmlPath = "Packages/com.thelegends.facebook.sdk/Runtime/FacebookSDK/link.xml";

        const string destLinkXmlPath = "Assets/FacebookSDK/link.xml";

        if (!AssetDatabase.IsValidFolder("Assets/FacebookSDK"))
        {
            AssetDatabase.CreateFolder("Assets", "FacebookSDK");
        }

        try
        {
            File.Copy(packageLinkXmlPath, destLinkXmlPath, overwrite: true);
            AssetDatabase.ImportAsset(destLinkXmlPath, ImportAssetOptions.ForceUpdate);
            Debug.Log("Copied & Overwritten FB link file from Packages to Assets/FacebookSDK folder.");
        }
        catch (System.Exception e)
        {
            StopBuildWithMessage("Failed to copy link.xml: " + e.Message);
        }
    }

    private void StopBuildWithMessage(string message)
    {
        var prefix = "[FB] ";
        throw new BuildPlayerWindow.BuildMethodException(prefix + message);
    }
}
