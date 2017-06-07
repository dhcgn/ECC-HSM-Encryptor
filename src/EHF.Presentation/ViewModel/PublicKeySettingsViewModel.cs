using System;
using EccHsmEncryptor.Presentation.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace EccHsmEncryptor.Presentation.ViewModel
{
    public class PublicKeySettingsViewModel : ViewModelBase
    {
        #region .ctor

        public PublicKeySettingsViewModel()
        {
            if (this.IsInDesignMode || IsInDesignModeStatic)
            {
            }
            else
            {
                this.LoadPublicKeysFromHSMCommand = new RelayCommand(this.LoadPublicKeysFromHSMCommandHandling);
                this.SaveCommand = new RelayCommand(this.SaveCommandHandling);
                this.CloseCommand = new RelayCommand(this.CloseCommandHandling);
            }
        }

        #endregion

        #region Commands

        public RelayCommand LoadPublicKeysFromHSMCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }

        #endregion

        #region Command Handling

        private void CloseCommandHandling()
        {
            WindowInvoker.ClosePublicKeySettingsWindows();
        }

        private void SaveCommandHandling()
        {
            new Storage.LocalStorageManager().RemoveAll<EcKeyPairInfoViewModel>();

            new Storage.LocalStorageManager().AddRange(SimpleIoc.Default.GetInstance<MainViewModel>().PublicKeys);
        }

        private void LoadPublicKeysFromHSMCommandHandling()
        {
            var keys = EncryptionSuite.Encryption.NitroKey.EllipticCurveCryptographer.GetEcKeyPairInfos();

            foreach (var ecKeyPairInfo in keys)
            {
                SimpleIoc.Default.GetInstance<MainViewModel>().PublicKeys.Add(new EcKeyPairInfoViewModel()
                {
                    IsSelected = false,
                    Description = "Added: " + DateTime.Now,
                    KeyPairInfos = ecKeyPairInfo
                });
            }
        }

        #endregion
    }
}