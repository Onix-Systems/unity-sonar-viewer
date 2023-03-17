
#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using App.Core;
using App.Services;
using UnityEditor.SceneManagement;

namespace ModelViewer.Assets.App.Core.Editor
{
    public class AppInitializer
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            SetupStartupScene();
            CreateAppConfig();
        }

        [MenuItem("Model Viewer/Create App Config")]
        private static void CheckAndCreateAppConfig()
        {
            string[] foundGuids = AssetDatabase.FindAssets("t:" + typeof(AppConfig).Name);

            if (foundGuids.Length == 1)
            {
                Debug.Log($"AppConfig already exists, check directory: {Constants.ConfigsResourcesDirectoryPath}");
                AppConfig appConfig = ScriptableObject.CreateInstance<AppConfig>();
                Selection.activeObject = appConfig;
                return;
            }

            if (foundGuids.Length > 1)
            {
                Debug.LogError($"Multiple AppConfig scriptable objects where found. Only one AppConfig can be used.");
                return;
            }

            if (foundGuids.Length == 0)
            {
                CreateAppConfig();
            }
        }

        private static void CreateAppConfig()
        {
            string[] foundGuids = AssetDatabase.FindAssets("t:" + typeof(AppConfig).Name);

            if (foundGuids.Length == 0)
            {
                AppConfig appConfig = ScriptableObject.CreateInstance<AppConfig>();
                string fullPath = Path.GetFullPath(Constants.ConfigsResourcesDirectoryPath);
                Directory.CreateDirectory(fullPath);

                string appConfigFilelPath = Path.Combine(
                    Constants.ConfigsResourcesDirectoryPath, 
                    Constants.AppConfigFileName + ".asset");

                AssetDatabase.CreateAsset(appConfig, appConfigFilelPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = appConfig;

                return;
            }

            if (foundGuids.Length == 1)
            {
                return;
            }

            if (foundGuids.Length > 1)
            {
                Debug.LogError($"More than one AppConfig scriptable object where found. Only one AppConfig can be used.");
                return;
            }
        }

        private static void SetupStartupScene()
        {
            EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;

            if (buildSettingsScenes.Length > 0)
            {
                EditorBuildSettingsScene firstScene = buildSettingsScenes[0];
                SceneAsset startupScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScene.path);
                EditorSceneManager.playModeStartScene = startupScene;
            }
        }
    }
}
#endif