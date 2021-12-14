using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiPlayerBuild
{
    #if UNITY_EDITOR
    // % (Ctrl), # (Shift), &(Alt)
    [MenuItem("MultiPlayer/2Player %&t")]
    public static void Build2Player()
    {
        PerformWin64Build(2);
    }

    static string GetProjectName()
    {
        string[] names = Application.dataPath.Split('/');
        return names[names.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; ++i)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    public static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        for (int i = 1; i <= playerCount; ++i)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
            $"Builds/Win64/{GetProjectName()}{i}/{GetProjectName()}{i}.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.AutoRunPlayer);
        }
    }

    #endif
}
