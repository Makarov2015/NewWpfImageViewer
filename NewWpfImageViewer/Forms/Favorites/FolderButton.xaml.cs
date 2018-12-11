using AlbumClassLibrary;
using NewWpfImageViewer.ClassDir;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace NewWpfImageViewer.Forms.Favorites
{
    /// <summary>
    /// Логика взаимодействия для FolderButton.xaml
    /// </summary>
    public partial class FolderButton : UserControl
    {
        public delegate void Mouse_ClickHandler(object sender, RoutedEventArgs e);
        public event Mouse_ClickHandler Mouse_Click;
        public event Mouse_ClickHandler Delete_Click;

        private IFolder folder = null;

        public FolderButton(IFolder entity)
        {
            InitializeComponent();

            NamePlate.Background = MainWindow.WinColor;

            folder = entity;
            NameLabel.Content = entity.Name;

            this.PreviewImage.Source = AlbumClassLibrary.Extensions.BitmapSourceExtension.GetSource(entity.PreviewImage);
        }

        public void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse_Click(folder, e);
        }

        private void FolderButtonGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            DeledeFolderButton.Visibility = Visibility.Visible;
            DeledeFolderButton.Background = this.Resources["TransparentWhite"] as System.Windows.Media.Brush;
        }

        private void FolderButtonGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            DeledeFolderButton.Visibility = Visibility.Hidden;
            DeledeFolderButton.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void DeledeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Delete_Click(folder, e);
        }
    }
}
