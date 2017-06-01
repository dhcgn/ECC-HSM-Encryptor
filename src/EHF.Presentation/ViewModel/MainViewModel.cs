using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EHF.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if (this.IsInDesignMode)
            {
                this.FileLength = 10324313;
            }
        }

        public RelayCommand StartCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        private long fileLength;
        public long FileLength
        {
            get => this.fileLength;
            set => this.Set(ref value, this.fileLength);
        }

        private bool showDropPanel;
        

        public bool ShowDropPanel
        {
            get => this.showDropPanel;
            set => this.Set(ref value, this.showDropPanel);
        }

        public void DropFiles(string[] files)
        {
            throw new System.NotImplementedException();
        }
    }
}