using AlbumClassLibrary;
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

namespace NewWpfImageViewer.Forms.Favorites
{
    /// <summary>
    /// Логика взаимодействия для AddFolderButton.xaml
    /// </summary>
    public partial class AddFolderButton : UserControl
    {
        private IAlbum Album;

        public AddFolderButton(IAlbum ialbum)
        {
            Album = ialbum;
            InitializeComponent();

            this.ColorGrid.Background = MainWindow.WinColor;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Album.AddFolder();
        }
    }
}
