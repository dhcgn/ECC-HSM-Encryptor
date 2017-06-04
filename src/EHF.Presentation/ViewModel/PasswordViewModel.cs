using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EHF.Presentation.ViewModel
{
    public class PasswordViewModel : ViewModelBase
    {
        private string password;

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

        public RelayCommand StartCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }

        public string Password
        {
            get => this.password;
            set => base.Set(ref this.password, value);
        }
    }
}