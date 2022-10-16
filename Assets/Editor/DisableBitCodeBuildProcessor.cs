using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class DisableBitCodeBuildProcessor
{
    [PostProcessBuild]
    private static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS) return;

        var projectPath = PBXProject.GetPBXProjectPath(path);
        var content = File.ReadAllText(@projectPath);
        content = content.Replace("ENABLE_BITCODE = YES", "ENABLE_BITCODE = NO");
        File.WriteAllText(projectPath, content);
    }
}
