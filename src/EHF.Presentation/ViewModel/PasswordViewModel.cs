using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EHF.Presentation.ViewModel
{
    public class PasswordViewModel : ViewModelBase
    {
        #region .ctor

        public PasswordViewModel()
        {
            this.ExitCommand=new RelayCommand(() =>
            {
                this.MessengerInstance.Send<Messages.PasswordWindowMessage>(new Messages.PasswordWindowMessage()
                {
                    DialogResult = false
                });
            });
            this.StartCommand = new RelayCommand(() =>
            {
                this.MessengerInstance.Send<Messages.PasswordWindowMessage>(new Messages.PasswordWindowMessage()
                {
                    DialogResult = true
                });
            });
        }

        #endregion

        #region Commands

        public RelayCommand StartCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }

        #endregion

        #region Properties

        private string password;

        public string Password
        {
            get => this.password;
            set => base.Set(ref this.password, value);
        }

        #endregion
    }
}