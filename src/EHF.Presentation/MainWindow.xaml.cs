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
using EHF.Presentation.ViewModel;

namespace EHF.Presentation
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void MainWindow_OnDragEnter(object sender, DragEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;
           // viewModel.ShowDropPanel = true;
        }

        private void MainWindow_OnDragLeave(object sender, DragEventArgs e)
        {
            var viewModel = (MainViewModel)DataContext;
           // viewModel.ShowDropPanel = false;
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var viewModel = (MainViewModel)DataContext;
                viewModel.DropFiles(files);
               // viewModel.ShowDropPanel = false;
            }
        }
    }
}
