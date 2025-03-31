using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    public static void PerformBuild()
    {
        string buildPath = "Builds/";
        string targetFile = buildPath + "game.exe";

        if (!Directory.Exists(buildPath))
            Directory.CreateDirectory(buildPath);

        BuildPipeline.BuildPlayer(
            new[] { "Assets/Scenes/MainScene.unity" }, // Ajoute tes scènes ici
            targetFile,
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );

        Debug.Log("Build terminée avec succès !");
    }
}
