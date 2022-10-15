using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;

public class PostBuildProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            ProcessForiOS(path);
        }
    }

    private static void ProcessForiOS(string path)
    {
        string pjPath = PBXProject.GetPBXProjectPath(path);
        PBXProject pj = new PBXProject();
        pj.ReadFromString(File.ReadAllText(pjPath));
        string target = pj.TargetGuidByName("Unity-iPhone");

        // Enable BitCode -> NO
        pj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

        File.WriteAllText(pjPath, pj.WriteToString());
    }

}
