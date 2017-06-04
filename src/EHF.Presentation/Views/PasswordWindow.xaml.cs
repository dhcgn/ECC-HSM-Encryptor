using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EHF.Presentation.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace EHF.Presentation.Views
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