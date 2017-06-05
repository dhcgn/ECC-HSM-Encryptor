using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHF.Presentation.Views
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
