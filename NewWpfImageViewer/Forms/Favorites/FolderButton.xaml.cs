using NewWpfImageViewer.ClassDir;
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

namespace NewWpfImageViewer.Forms
{
    /// <summary>
    /// Логика взаимодействия для FolderButton.xaml
    /// </summary>
    public partial class FolderButton : UserControl
    {
        public delegate void Mouse_ClickHandler(object sender, RoutedEventArgs e);
        public event Mouse_ClickHandler Mouse_Click;

        private FolderEntity folder = null;

        public FolderButton(FolderEntity entity, string previeFilePath, bool IsSelected)
        {
            InitializeComponent();

            folder = entity;
            NameLabel.Content = entity.ShownName;

            if (System.IO.File.Exists(previeFilePath))
                PreviewImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap((System.Drawing.Bitmap.FromFile(previeFilePath) as System.Drawing.Bitmap).GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, null);

            if(IsSelected)
                NamePlate.Background = Brushes.Coral;
            else
                NamePlate.Background = Brushes.CadetBlue;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
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
            MessageBox.Show("Удаляшки!");
        }
    }
}
