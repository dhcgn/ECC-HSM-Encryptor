using System;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace EccHsmEncryptor.Presentation.ViewModel
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

    public enum StorageNames
    {
        PublicKeys
    }

    public class Messages
    {
        public class PasswordWindowMessage
        {
            public bool? DialogResult { get; set; }
        }

        public class StorageChange
        {
            public StorageNames StorageName { get; set; }
        }
    }

}
