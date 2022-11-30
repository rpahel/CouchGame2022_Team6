using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class MyCustomBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        //Debug.Log("MyCustomBuildProcessor.OnPostprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
    }
}

public class BuildPlayerExample : MonoBehaviour
{
    [MenuItem("Build/Build PC")]
    public static void MyBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new()
        {
            scenes = new[] { "Assets/_Scenes/Game.unity" },
            locationPathName = $"Builds/Build_{System.DateTime.Now:yyyy.MM.dd_hh.mm.ss}/Team6_Glouton.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.CleanBuildCache
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize / 1000000f + " Megabytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}