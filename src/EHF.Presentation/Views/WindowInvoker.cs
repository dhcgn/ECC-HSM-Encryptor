using System;

namespace EccHsmEncryptor.Presentation.Views
{
    public static class WindowInvoker
    {
        private static PublicKeySettingsWindows publicKeySettingsWindows;
        private static SettingsWindows settingsWindows;

        public enum Windows
        {
            PublicKeySettings,
            Settings
        }

        public static void ShowWindow(Windows window)
        {
            switch (window)
            {
                case Windows.PublicKeySettings:
                    publicKeySettingsWindows = new PublicKeySettingsWindows();
                    publicKeySettingsWindows.Show();
                    break;
                case Windows.Settings:
                    settingsWindows = new SettingsWindows();
                    settingsWindows.Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(window), window, null);
            }
        }

        public static void CloseWindows(Windows window)
        {
            switch (window)
            {
                case Windows.PublicKeySettings:
                    publicKeySettingsWindows.Close();
                    break;
                case Windows.Settings:
                    settingsWindows.Close();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(window), window, null);
            }
          
        }
    }
}