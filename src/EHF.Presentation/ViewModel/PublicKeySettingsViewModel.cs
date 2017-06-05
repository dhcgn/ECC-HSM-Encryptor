using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace EHF.Presentation.ViewModel
{
    public class PublicKeySettingsViewModel : ViewModelBase
    {
        public PublicKeySettingsViewModel()
        {
            if (this.IsInDesignMode || IsInDesignModeStatic)
            {
            }
            else
            {
                this.LoadPublicKeysFromHSMCommand = new RelayCommand(this.LoadPublicKeysFromHSMCommandHandling);
                this.SaveCommand = new RelayCommand(this.SaveCommandHandling);
            }
        }

        private void SaveCommandHandling()
        {
            new Storage.LocalStorageManager().RemoveAll<EcKeyPairInfoViewModel>();

            new Storage.LocalStorageManager().AddRange(SimpleIoc.Default.GetInstance<MainViewModel>().PublicKeys);
        }

        private void LoadPublicKeysFromHSMCommandHandling()
        {
            var keys = Encryption.NitroKey.EllipticCurveCryptographer.GetEcKeyPairInfos();

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

        public RelayCommand LoadPublicKeysFromHSMCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
    }
}