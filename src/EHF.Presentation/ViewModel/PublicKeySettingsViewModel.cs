using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EccHsmEncryptor.Presentation.DesignData;
using EccHsmEncryptor.Presentation.Views;
using EncryptionSuite.Contract;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Storage;

namespace EccHsmEncryptor.Presentation.ViewModel
{
    public class PublicKeySettingsViewModel : ViewModelBase
    {
        #region .ctor

        public PublicKeySettingsViewModel()
        {
            if (this.IsInDesignMode || IsInDesignModeStatic)
            {
                this.PublicKeys = new ObservableCollection<EcKeyPairInfoViewModel>()
                {
                    new EcKeyPairInfoViewModel()
                    {
                        KeyPairInfos = DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #1", "Token White", "DENK0100123"),
                        IsSelected = true,
                        Description = "My buisness token"
                    },
                    new EcKeyPairInfoViewModel()
                    {
                        KeyPairInfos = DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #2", "Token White", "DENK0100123"),
                        IsSelected = false,
                        Description = "My private token"
                    },
                    new EcKeyPairInfoViewModel()
                    {
                        KeyPairInfos = DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #3", "Token Black", "DENK0100321"),
                        IsSelected = false,
                        Description = "My private backup token"
                    },
                };
            }
            else
            {
                this.LoadPublicKeysFromHSMCommand = new RelayCommand(this.LoadPublicKeysFromHSMCommandHandling);
                this.SaveCommand = new RelayCommand(this.SaveCommandHandling);
                this.CloseCommand = new RelayCommand(this.CloseCommandHandling);

                this.ExportCommand = new RelayCommand<ExportType>(this.ExportCommandHandling);
                this.LoadedCommand = new RelayCommand(this.LoadedCommandHandling);

                this.RemoveCommand = new RelayCommand(this.RemoveCommandHandling);
            }
        }

        private void RemoveCommandHandling()
        {
            if (this.SelectedPublicKey != null)
            {
                this.PublicKeys.Remove(this.SelectedPublicKey);
            }
        }

        private void LoadedCommandHandling()
        {
            this.RefreshPublicKeys();
        }

        public RelayCommand LoadedCommand { get; set; }

        #endregion

        #region Commands

        public RelayCommand LoadPublicKeysFromHSMCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        public RelayCommand<ExportType> ExportCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand RemoveAllCommand { get; set; }

        public enum ExportType
        {
            Ans1,
            Der,
            Json
        }

        #endregion

        #region Command Handling

        private void CloseCommandHandling()
        {
            WindowInvoker.CloseWindows(WindowInvoker.Windows.PublicKeySettings);
        }

        private void ExportCommandHandling(ExportType exportType)
        {
        }

        private void RefreshPublicKeys()
        {
            List<EcKeyPairInfoViewModel> loadedKeys;
            try
            {
                loadedKeys = new LocalStorageManager().GetAll<EcKeyPairInfoViewModel>(StorageNames.PublicKeys.ToString()).ToList();
            }
            catch (Exception e)
            {
                loadedKeys = Enumerable.Empty<EcKeyPairInfoViewModel>().ToList();
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.PublicKeys = new ObservableCollection<EcKeyPairInfoViewModel>(loadedKeys); });
        }


        private void SaveCommandHandling()
        {
            new LocalStorageManager().RemoveAll(StorageNames.PublicKeys.ToString());
            new LocalStorageManager().AddRange(this.PublicKeys, StorageNames.PublicKeys.ToString());

            this.MessengerInstance.Send(new Messages.StorageChange() {StorageName = StorageNames.PublicKeys});
        }

        private void LoadPublicKeysFromHSMCommandHandling()
        {
            var keys = EncryptionSuite.Encryption.NitroKey.EllipticCurveCryptographer.GetEcKeyPairInfos();

            foreach (var ecKeyPairInfo in keys)
            {
                this.PublicKeys.Add(new EcKeyPairInfoViewModel()
                {
                    IsSelected = false,
                    Description = "Added: " + DateTime.Now,
                    KeyPairInfos = ecKeyPairInfo
                });
            }
        }

        #endregion

        private ObservableCollection<EcKeyPairInfoViewModel> publicKeys;
        public ObservableCollection<EcKeyPairInfoViewModel> PublicKeys
        {
            get => this.publicKeys;
            set => this.Set(ref this.publicKeys, value);
        }

        private EcKeyPairInfoViewModel selectedPublicKey;
        public EcKeyPairInfoViewModel SelectedPublicKey
        {
            get => this.selectedPublicKey;
            set => this.Set(ref this.selectedPublicKey, value);
        }
    }
}