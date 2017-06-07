using System.Windows;
using EccHsmEncryptor.Presentation.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace EccHsmEncryptor.Presentation.Views
{
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            this.InitializeComponent();
        }

        private void PasswordWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.PasswordBox.Focus();
            Messenger.Default.Register<Messages.PasswordWindowMessage>(this, message =>
            {
                this.DialogResult = message.DialogResult;
            });
        }

        private void PasswordWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<Messages.PasswordWindowMessage>(this);
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((PasswordViewModel)this.DataContext).Password = this.PasswordBox.Password;
        }
    }
}