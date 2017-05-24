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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;

namespace NAM.Presentation
{
    /// <summary>
    /// Interaktionslogik für MenuButton.xaml
    /// </summary>
    public partial class MenuButton : UserControl
    {
        public MenuButton()
        {
            InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic)
            {
                this.Text = "TEST";
                
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(MenuButton), new PropertyMetadata(default(string)));

        public string Text
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                  return 
                       "TEST";

                }
                return (string) GetValue(TextProperty);
            }
            set { SetValue(TextProperty, value); }
        }
    }
}
