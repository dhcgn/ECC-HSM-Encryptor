using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EccHsmEncryptor.Presentation.DesignData;
using EccHsmEncryptor.Presentation.Views;
using EncryptionSuite.Contract;
using EncryptionSuite.Encryption.Hybrid;
using EncryptionSuite.Encryption.NitroKey;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Storage;

namespace EccHsmEncryptor.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region .ctor

        public MainViewModel()
        {
            if (this.IsInDesignMode || IsInDesignModeStatic)
            {
                #region Design Data

                this.FilePath = @"C:\temp\document.docx";
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
                this.AvailableHardwareTokens = new List<EcKeyPairInfo>()
                {
                    DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #1", "Token White", "DENK0100123"),
                    DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #3", "Token Black", "DENK0100321"),
                };
                this.SelectedAvailableHardwareToken = this.AvailableHardwareTokens.First();

                #endregion
            }
            else
            {
                this.LoadedCommand = new RelayCommand(this.LoadedCommandHandling);
                this.PublicKeySettingsCommand = new RelayCommand(this.PublicKeySettingsCommandHandling);
                this.EncryptCommand = new RelayCommand(this.EncryptCommandHandling, this.EncryptCommandCanExecute);
                this.DecryptCommand = new RelayCommand(this.DecryptCommandHandling, this.DecryptCommandCanExecute);

                this.PropertyChanged += (sender, args) =>
                {
                    this.EncryptCommand?.RaiseCanExecuteChanged();
                    this.DecryptCommand?.RaiseCanExecuteChanged();
                };

                // Todo: fix this workaround
                base.MessengerInstance.Register<Messages.PropertyChanged>(this, changed =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        if (changed.TypeName == typeof(EcKeyPairInfoViewModel).Name)
                        {
                            this.EncryptCommand?.RaiseCanExecuteChanged();
                            this.DecryptCommand?.RaiseCanExecuteChanged();
                        }
                    });
                });

                this.AvailableHardwareTokensIsBusy = true;
                this.PublicKeysIsBusy = true;
            }
        }

        #endregion

        #region Commands

        public RelayCommand DecryptCommand { get; set; }
        public RelayCommand EncryptCommand { get; set; }
        public RelayCommand PublicKeySettingsCommand { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        #endregion

        #region Command Handling

        private void PublicKeySettingsCommandHandling()
        {
            WindowInvoker.ShowPublicKeySettingsWindows();
        }

        private bool EncryptCommandCanExecute()
        {
            if (!File.Exists(this.FilePath))
                return false;

            if (!this.PublicKeys.Any(model => model.IsSelected))
                return false;

            return true;
        }

        private void EncryptCommandHandling()
        {
            using (var input = File.OpenRead(this.FilePath))
            using (var output = File.Create(this.FilePath + ".enc"))
            {
                var publicKeys = this.PublicKeys.Where(model => model.IsSelected).Select(model => model.KeyPairInfos.PublicKey.ExportPublicKey());
                HybridEncryption.Encrypt(input, output, publicKeys.ToArray());
            }
        }

        private bool DecryptCommandCanExecute()
        {
            if (!File.Exists(this.FilePath))
                return false;

            if (this.SelectedAvailableHardwareToken == null)
                return false;

            return true;
        }

        private void DecryptCommandHandling()
        {
            var result = new PasswordWindow().ShowDialog();

            if (result == null || !(bool) result)
                return;

            var password = SimpleIoc.Default.GetInstance<PasswordViewModel>().Password;

            using (var input = File.OpenRead(this.FilePath))
            using (var output = File.Create(this.FilePath + ".dec"))
            {
                HybridEncryption.Decrypt(input, output, password);
            }
        }

        private async void LoadedCommandHandling()
        {
            var tasks = new List<Task>
            {
                Task.Run(() => this.RefreshPublicKeys()),
                Task.Run(() => this.RefreshAvailableHardwareToken())
            };

            await Task.WhenAll(tasks);
        }

        #endregion

        private void RefreshPublicKeys()
        {
            List<EcKeyPairInfoViewModel> loadedKeys;
            try
            {
                loadedKeys = new LocalStorageManager().GetAll<EcKeyPairInfoViewModel>().ToList();
            }
            catch (Exception e)
            {
                loadedKeys = Enumerable.Empty<EcKeyPairInfoViewModel>().ToList();
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.PublicKeys = new ObservableCollection<EcKeyPairInfoViewModel>(loadedKeys);

                if (!loadedKeys.Any())
                    this.PublicKeysNotAvailable = true;

                this.PublicKeysIsBusy = false;
            });
        }

        private void RefreshAvailableHardwareToken()
        {
            EcKeyPairInfo[] nitroKeys;
            try
            {
                nitroKeys = EllipticCurveCryptographer.GetEcKeyPairInfos();
            }
            catch (Exception e)
            {
                nitroKeys = Enumerable.Empty<EcKeyPairInfo>().ToArray();
            }


            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.AvailableHardwareTokens = new List<EcKeyPairInfo>(nitroKeys);
                this.SelectedAvailableHardwareToken = this.AvailableHardwareTokens.FirstOrDefault();
                this.AvailableHardwareTokensIsBusy = false;
            });
        }

        public void DropFiles(string[] files)
        {
            this.FilePath = files.FirstOrDefault();
        }

        #region Properties

        private List<EcKeyPairInfo> availableHardwareTokens;

        public List<EcKeyPairInfo> AvailableHardwareTokens
        {
            get => this.availableHardwareTokens;
            set => this.Set(ref this.availableHardwareTokens, value);
        }

        private EcKeyPairInfo selectedAvailableHardwareToken;

        public EcKeyPairInfo SelectedAvailableHardwareToken
        {
            get => this.selectedAvailableHardwareToken;
            set => this.Set(ref this.selectedAvailableHardwareToken, value);
        }

        private ObservableCollection<EcKeyPairInfoViewModel> publicKeys;

        public ObservableCollection<EcKeyPairInfoViewModel> PublicKeys
        {
            get => this.publicKeys;
            set => this.Set(ref this.publicKeys, value);
        }

        private string filePath;

        public string FilePath
        {
            get => this.filePath;
            set => this.Set(ref this.filePath, value);
        }

        private bool showDropPanel;

        public bool ShowDropPanel
        {
            get => this.showDropPanel;
            set => this.Set(ref this.showDropPanel, value);
        }

        private bool availableHardwareTokensIsBusy;


        public bool AvailableHardwareTokensIsBusy
        {
            get => this.availableHardwareTokensIsBusy;
            set => this.Set(ref this.availableHardwareTokensIsBusy, value);
        }

        private bool publicKeysIsBusy;

        public bool PublicKeysIsBusy
        {
            get => this.publicKeysIsBusy;
            set => this.Set(ref this.publicKeysIsBusy, value);
        }

        private bool publicKeysNotAvailable;

        public bool PublicKeysNotAvailable
        {
            get => this.publicKeysNotAvailable;
            set => this.Set(ref this.publicKeysNotAvailable, value);
        }

        #endregion
    }
}