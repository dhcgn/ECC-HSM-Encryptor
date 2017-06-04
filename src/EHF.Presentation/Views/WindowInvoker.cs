using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHF.Presentation.Views
{
    public static class WindowInvoker
    {
        public static void ShowPublicKeySettingsWindows()
        {
            var publicKeySettingsWindows = new PublicKeySettingsWindows();
            publicKeySettingsWindows.Show();
        }
    }
}
