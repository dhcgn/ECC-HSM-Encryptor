using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace EHF.Presentation.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<PasswordViewModel>();
            SimpleIoc.Default.Register<PublicKeySettingsViewModel>();
        }


        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();
        public PasswordViewModel PasswordViewModel => ServiceLocator.Current.GetInstance<PasswordViewModel>();
        public PublicKeySettingsViewModel PublicKeySettingsViewModel => ServiceLocator.Current.GetInstance<PublicKeySettingsViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }

    public class Messages
    {
        public class PasswordWindowMessage
        {
            public bool? DialogResult { get; set; }
        }
    }

}
