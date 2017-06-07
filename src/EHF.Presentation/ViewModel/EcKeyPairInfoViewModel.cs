using EncryptionSuite.Contract;
using GalaSoft.MvvmLight;

namespace EccHsmEncryptor.Presentation.ViewModel
{
    public class EcKeyPairInfoViewModel : ViewModelBase
    {
        #region Properties

        private bool isSelected;
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                base.Set(ref this.isSelected, value);
                base.MessengerInstance.Send(new Messages.PropertyChanged(this.GetType()));
            }
        }

        private string description;
        public string Description
        {
            get => this.description;
            set => base.Set(ref this.description, value);
        }

        public EcKeyPairInfo KeyPairInfos { get; set; }

        #endregion
    }
}