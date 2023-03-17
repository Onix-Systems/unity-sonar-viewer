
using UnityEngine;
using App.Core.States;
using App.Helpers;
using App.Contexts;

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