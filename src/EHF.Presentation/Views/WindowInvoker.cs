namespace EccHsmEncryptor.Presentation.Views
{
    public static class WindowInvoker
    {
        private static PublicKeySettingsWindows publicKeySettingsWindows;

        public static void ShowPublicKeySettingsWindows()
        {
            publicKeySettingsWindows = new PublicKeySettingsWindows();
            publicKeySettingsWindows.Show();
        }

        public static void ClosePublicKeySettingsWindows()
        {
            publicKeySettingsWindows.Close();
        }
    }
}
