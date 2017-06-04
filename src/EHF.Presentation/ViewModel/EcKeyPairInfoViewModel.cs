using Contract;
using GalaSoft.MvvmLight;

namespace EHF.Presentation.ViewModel
{
    public class EcKeyPairInfoViewModel : ViewModelBase
    {
        private bool isSelected;

        public bool IsSelected
        {
            get => this.isSelected;
            set => base.Set(ref this.isSelected, value);
        }

        public EcKeyPairInfo KeyPairInfos { get; set; }
    }
}