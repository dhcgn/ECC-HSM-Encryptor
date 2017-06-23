using System.Windows;
using EccHsmEncryptor.Presentation.ViewModel;

namespace EccHsmEncryptor.Presentation.Views
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
            var viewModel = (MainViewModel) this.DataContext;
            // viewModel.ShowDropPanel = true;
        }

        private void MainWindow_OnDragLeave(object sender, DragEventArgs e)
        {
            var viewModel = (MainViewModel) this.DataContext;
            // viewModel.ShowDropPanel = false;
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                var viewModel = (MainViewModel) this.DataContext;
                viewModel.SetFilenamesToView(files);
                // viewModel.ShowDropPanel = false;
            }
        }
    }
}