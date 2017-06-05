using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Contract;
using EHF.Presentation.DesignData;
using EHF.Presentation.Views;
using Encryption;
using Encryption.Hybrid;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using Storage;

namespace EHF.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if (this.IsInDesignMode || IsInDesignModeStatic)
            {
                #region Design Data

                this.FileLength = 10324313;
                this.FilePath = @"C:\temp\document.docx";
                this.PublicKeys = new ObservableCollection<EcKeyPairInfoViewModel>()
                {
                    new EcKeyPairInfoViewModel()
                    {
                        KeyPairInfos = DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #1", "Token White", "DENK0100123"),
                        IsSelected = true,
                    },
                    new EcKeyPairInfoViewModel()
                    {
                        KeyPairInfos = DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #2", "Token White", "DENK0100123"),
                        IsSelected = false,
                    },
                    new EcKeyPairInfoViewModel()
                    {
                        KeyPairInfos = DesignDataFactory.CreateDesignData<EcKeyPairInfo>("My Key #3", "Token Black", "DENK0100321"),
                        IsSelected = false,
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
                    this.EncryptCommand.RaiseCanExecuteChanged();
                    this.DecryptCommand.RaiseCanExecuteChanged();
                };
            }
        }

        private void PublicKeySettingsCommandHandling()
        {
            WindowInvoker.ShowPublicKeySettingsWindows();
        }

        private bool DecryptCommandCanExecute()
        {
            if (!File.Exists(this.FilePath))
                return false;

            if (this.SelectedAvailableHardwareToken == null)
                return false;

            return true;
        }

        private bool EncryptCommandCanExecute()
        {
            if (!File.Exists(this.FilePath))
                return false;

            if (!this.PublicKeys.Any(model => model.IsSelected))
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

        private void EncryptCommandHandling()
        {
            using (var input = File.OpenRead(this.FilePath))
            using (var output = File.Create(this.FilePath + ".enc"))
            {
                var publicKeys = this.PublicKeys.Where(model => model.IsSelected).Select(model => model.KeyPairInfos.PublicKey.ExportPublicKey());
                HybridEncryption.Encrypt(input, output, publicKeys.ToArray());
            }
        }

        public RelayCommand DecryptCommand { get; set; }

        public RelayCommand EncryptCommand { get; set; }
        public RelayCommand PublicKeySettingsCommand { get; set; }

        private async void LoadedCommandHandling()
        {
            var loadedKeys = await Task.Run(() =>
            {
                try
                {
                    return new LocalStorageManager().GetAll<EcKeyPairInfoViewModel>().ToArray();
                }
                catch (Exception e)
                {
                    return Enumerable.Empty<EcKeyPairInfoViewModel>().ToArray();
                }
            });
            var nitroKeys = await Task.Run(() => Encryption.NitroKey.EllipticCurveCryptographer.GetEcKeyPairInfos());

            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.PublicKeys = new ObservableCollection<EcKeyPairInfoViewModel>(loadedKeys); });
            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.AvailableHardwareTokens = new List<EcKeyPairInfo>(nitroKeys); });
            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.SelectedAvailableHardwareToken = this.AvailableHardwareTokens.FirstOrDefault(); });
        }

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

        public RelayCommand StartCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        private ObservableCollection<EcKeyPairInfoViewModel> publicKeys;

        public ObservableCollection<EcKeyPairInfoViewModel> PublicKeys
        {
            get => this.publicKeys;
            set
            {
                if (this.publicKeys != null)
                    this.publicKeys.CollectionChanged -= this.PublicKeysCollectionChanged;

                this.Set(ref this.publicKeys, value);

                if (this.publicKeys != null)
                    this.publicKeys.CollectionChanged += this.PublicKeysCollectionChanged;
            }
        }

        private void PublicKeysCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.EncryptCommand.RaiseCanExecuteChanged();
            this.DecryptCommand.RaiseCanExecuteChanged();
        }

        private long fileLength;

        public long FileLength
        {
            get => this.fileLength;
            set => this.Set(ref this.fileLength, value);
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

        public void DropFiles(string[] files)
        {
            this.FilePath = files.FirstOrDefault();
        }
    }
}