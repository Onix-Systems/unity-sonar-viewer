
using Utils.UnityCoroutineHelpers;
using App.Infrastructure.Contexts;
using App.Services;
using App.UI.Screens;
using App.Helpers;
using App.Services.SceneLoading;
using App.UI;
using App.Services.AppFSM;

namespace App.Core.States
{
    public class BootState : IAppState
    {
        public void Enter()
        {
            IContext mainContext = MainContext.Instance;

            SceneLoader sceneLoader = mainContext.Get<SceneLoader>();
            
            sceneLoader.Load(Scene.Main, () =>
            {
                AppUI ui = mainContext.Get<AppUI>();
                UIConfig uiConfig = mainContext.Get<AppConfig>().UIConfig;

                ui.ShowScreen<PreviewScreenView>();

                Coroutines.InvokeAfter(uiConfig.PreviewScreenVisibleTime, () => 
                {
                    AppStateNavigator.GoTo<AuthState>();
                });
            });
        }
    }
}
