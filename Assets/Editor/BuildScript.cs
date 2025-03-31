using UnityEditor;
using UnityEngine;
using System.IO;

namespace BuildSystem
{
    public class BuildScript
    {
        public static void PerformBuild()
        {
            Debug.Log("🚀 Build Unity commencée !");
            string buildPath = "Builds/";
            string targetFile = buildPath + "game.exe";

            if (!Directory.Exists(buildPath))
                Directory.CreateDirectory(buildPath);

            string[] scenes = { "Assets/Scenes/MainScene.unity" };

            BuildPipeline.BuildPlayer(scenes, targetFile, BuildTarget.StandaloneWindows64, BuildOptions.None);

            if (File.Exists(targetFile))
                Debug.Log("✅ Build réussie : " + targetFile);
            else
                Debug.LogError("❌ La build n'a pas été générée !");
        }
    }
}
