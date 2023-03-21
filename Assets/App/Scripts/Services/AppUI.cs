
using System;
using UnityEngine;
using App.Infrastructure.Contexts;
using App.Infrastructure.UI;
using App.UI.Contexts;

namespace App.Services
{
    public class AppUI
    {
        public ScreenView CurrentScreen { get; private set; }

        public TScreen GetScreen<TScreen>() where TScreen: ScreenView
        {
            IContext mainContext = MainContext.Instance;
            
            if (mainContext.TryGet(out ScreensContext screensContext))
            {
                return screensContext.Get<TScreen>();
            }

            return default;
        }

        public TPopup GetPopup<TPopup>() where TPopup: PopupView
        {
            IContext mainContext = MainContext.Instance;

            if (mainContext.TryGet(out PopupsContext popupsContext))
            {
                return popupsContext.Get<TPopup>();
            }

            return default;
        }

        public ScreenView ShowScreen<TScreen>() where TScreen : ScreenView
        {
            Type screenType = typeof(TScreen);
            IContext mainContext = MainContext.Instance;
            
            if (mainContext.TryGet(out ScreensContext screensContext))
            {
                TScreen screen = screensContext.Get<TScreen>();
                return ShowScreen(screen);
            }
            else
            {
                Debug.LogError($"Unable to show screen \"{screenType.Name}\", cuz screens context was not found");
            }

            return default;
        }

        public ScreenView ShowScreen(ScreenView screen)
        {
            if (screen == CurrentScreen)
            {
                return CurrentScreen;
            }

            CurrentScreen?.Hide();
            CurrentScreen = screen;
            CurrentScreen.Show();

            return CurrentScreen;
        }

        public void ShowPopup(PopupView popup)
        {
            popup.Show();
        }

        public void HidePopup<TPopup>() where TPopup : PopupView
        {
            IContext mainContext = MainContext.Instance;

            if (mainContext.TryGet(out PopupsContext popupsContext))
            {
                TPopup screen = popupsContext.Get<TPopup>();
                HidePopup(screen);
            }
            else
            {
                Debug.LogError($"Unable to hide popup \"{typeof(TPopup).Name}\", cuz popups context was not found");
            }
        }

        public void HidePopup(PopupView popup)
        {
            popup.Hide();
        }

        public void HideAllPopups()
        {
            IContext mainContext = MainContext.Instance;

            if (mainContext.TryGet(out PopupsContext popupsContext))
            {
                foreach(View popupView in popupsContext.Views)
                {
                    popupView.Hide();
                }
            }
        }

        public bool IsCurrentScreen<TScreen>() where TScreen : ScreenView
        {
            Type screenType = typeof(TScreen);

            return CurrentScreen.GetType() == screenType;
        }
    }
}
