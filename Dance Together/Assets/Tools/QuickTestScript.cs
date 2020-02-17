using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Provides some quick testing buttons for local machine LAN testing
/// </summary>
public class NetworkEditorTools : MonoBehaviour
{
    /// <summary>
    /// Stores whether the 'use editor testing' menu checkbox is checked
    /// </summary>
    public static bool useEditorTesting
    {
        get
        {
            return EditorPrefs.GetBool("useEditorPlayer");
        }
        set
        {
            EditorPrefs.SetBool("useEditorPlayer", value);
        }
    }

    /// <summary>
    /// Whether the current in-editor play session is part of an editor test
    /// </summary>
    public static bool isRunningEditorTest
    {
        get
        {
            return EditorPrefs.GetBool("isRunningEditorTest");
        }
        set
        {
            EditorPrefs.SetBool("isRunningEditorTest", value);
        }
    }

    public static string testBuildFolder
    {
        get
        {
            return $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'))}/TestBuilds";
        }
    }

    /// <summary>
    /// Path to the test build .exe
    /// </summary>
    public static string testBuildPath
    {
        get
        {
            return $"{testBuildFolder}/{PlayerSettings.productName}.exe";
        }
    }

    [MenuItem("QuickTest/Build and run LAN multiplayer")]
    public static void BuildAndTest()
    {
        List<string> levels = new List<string>();
        string originalScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;

        UnityEditor.Build.Reporting.BuildReport buildReport = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, testBuildPath, BuildTarget.StandaloneWindows64, BuildOptions.Development);

        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(originalScene);

        if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            EditorUtility.DisplayDialog("Someone goofed!", $"Build failed ({buildReport.summary.totalErrors} errors)", "OK");
            return;
        }
        else
        {
            TestPunkey();
        }
    }

    [MenuItem("QuickTest/Run LAN multiplayer")]
    public static void TestPunkey()
    {
        if (!useEditorTesting)
        {
            // run another EXE
            RunTestBuild();
        }
        else
        {
            // play an instance in the editor
            isRunningEditorTest = true;

            EditorApplication.isPlaying = true;
        }

        RunTestBuild();
    }

    [MenuItem("QuickTest/Run instance in-editor")]
    private static void UseEditorTesting()
    {
        useEditorTesting = !useEditorTesting;
    }

    [MenuItem("QuickTest/Run instance in-editor", true)]
    private static bool UseEditorTestingValidate()
    {
        Menu.SetChecked("QuickTest/Run instance in-editor", useEditorTesting);
        return true;
    }

    private static void RunTestBuild()
    {
        // Run another instance of the game
        System.Diagnostics.Process process = new System.Diagnostics.Process();

        process.StartInfo.FileName = testBuildPath;
        process.StartInfo.WorkingDirectory = testBuildFolder;
        process.StartInfo.Arguments = $"AutoConnect"; // TODO: implement in a local manager on game start

        process.Start();
    }
}

#endif