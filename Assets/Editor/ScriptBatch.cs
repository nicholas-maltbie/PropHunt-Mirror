using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

public class ScriptBatch 
{
    public static string[] GetScenes()
    {
        return new string[] {"Assets/Scenes/BasicHouse.unity"};
    }

    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        MacOSBuild();
        LinuxBuild();
        WindowsBuild();
    }

    [MenuItem("Build/MacOS Build")]
    public static void MacOSBuild ()
    {
        // Get filename.
        string path = "Builds/MacOS";
        string[] levels = GetScenes();

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/PropHunt.app", BuildTarget.StandaloneOSX, BuildOptions.Development);
    }

    [MenuItem("Build/Linux Build")]
    public static void LinuxBuild ()
    {
        // Get filename.
        string path = "Builds/Linux";
        string[] levels = GetScenes();

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/PropHunt.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.Development);
    }

    [MenuItem("Build/Windows64 Build")]
    public static void WindowsBuild ()
    {
        // Get filename.
        string path = "Builds/Wins64";
        string[] levels = GetScenes();

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/PropHunt.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
    }

    // [MenuItem("Build/Windows Build")]
    // public static void WindowsBuild ()
    // {
    //     // Get filename.
    //     string path = "Builds/Win";
    //     string[] levels = GetScenes();

    //     // Build player.
    //     BuildPipeline.BuildPlayer(levels, path + "/PropHunt.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    // }
}