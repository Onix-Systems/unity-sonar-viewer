
using UnityEngine;
using App.Core.Contexts;
using App.Core.States;
using App.Helpers;

namespace App.Core
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            InitializeAppContext();
            Boot();   
        }

        private void InitializeAppContext()
        {
            GameObject gameObject = new GameObject("App Context");
            gameObject.AddComponent<AppContext>();
        }

        private void Boot()
        {
            AppStateNavigator.GoTo<BootState>();
        }
    }
}