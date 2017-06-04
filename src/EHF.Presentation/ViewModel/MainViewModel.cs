using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;
using EHF.Presentation.DesignData;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;

namespace EHF.Presentation.ViewModel
{
    public class EcKeyPairInfoViewModel
    {
        public bool IsSelected { get; set; }
        public EcKeyPairInfo KeyPairInfos { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if (this.IsInDesignMode || IsInDesignModeStatic)
            {
                #region Design Data

                this.FileLength = 10324313;
                this.FilePath = @"C:\temp\document.docx";
                this.EcKeyPairInfoViewModels = new List<EcKeyPairInfoViewModel>()
                {
                    new EcKeyPairInfoViewModel()
                    {
                        KeyPairInfos = DesignDataFactory.CreateDesignData<EcKeyPairInfo>()
                    }
                };
                this.EcKeyPairInfos = new List<EcKeyPairInfo>()
                {
                    DesignDataFactory.CreateDesignData<EcKeyPairInfo>()
                };

                #endregion
            }
            else
            {
                this.LoadedCommand = new RelayCommand(this.LoadedCommandHandling);
            }
        }

        private async void LoadedCommandHandling()
        {
            var loadedKeys = await Task.Run(() => new Storage.LocalStorageManager().GetAll<EcKeyPairInfoViewModel>().ToArray());
            var nitroKeys = await Task.Run(() => Encryption.NitroKey.EllipticCurveCryptographer.GetEcKeyPairInfos());

            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.EcKeyPairInfoViewModels = new List<EcKeyPairInfoViewModel>(loadedKeys); });
            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.EcKeyPairInfos = new List<EcKeyPairInfo>(nitroKeys); });
        }

        private List<EcKeyPairInfo> ecKeyPairInfos;

        public List<EcKeyPairInfo> EcKeyPairInfos
        {
            get => this.ecKeyPairInfos;
            set => this.Set(ref this.ecKeyPairInfos, value);
        }

        public RelayCommand StartCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        private List<EcKeyPairInfoViewModel> ecKeyPairInfoViewModels;

        public List<EcKeyPairInfoViewModel> EcKeyPairInfoViewModels
        {
            get => this.ecKeyPairInfoViewModels;
            set => this.Set(ref this.ecKeyPairInfoViewModels, value);
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
            set => base.Set(ref this.filePath, value);
        }

        private bool showDropPanel;
        public bool ShowDropPanel
        {
            get => this.showDropPanel;
            set => this.Set(ref this.showDropPanel, value);
        }

        public void DropFiles(string[] files)
        {
            
        }
    }
}