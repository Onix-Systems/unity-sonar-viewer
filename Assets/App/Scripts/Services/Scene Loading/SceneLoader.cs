
using System;
using UnityEngine.SceneManagement;

namespace App.Services.SceneLoading
{
    public class SceneLoader
    {
        public void Load(Scene scene, Action onLoaded=null)
        {
            int buildIndex = GetSceneBuildIdex(scene);
            
            SceneManager.sceneLoaded += (scene, sceneLoadMode) => {
                if (scene.buildIndex == buildIndex)
                    onLoaded?.Invoke();
            };

            SceneManager.LoadScene(buildIndex);
        }

        private int GetSceneBuildIdex(Scene scene) => (int) scene;
    }
}