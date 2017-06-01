using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EHF.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
      
        public RelayCommand StartCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        private bool showDropPanel;
        public bool ShowDropPanel
        {
            get => this.showDropPanel;
            set => base.Set(ref value, this.showDropPanel);
        }

        public void DropFiles(string[] files)
        {
            throw new System.NotImplementedException();
        }
    }
}