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

namespace NewWpfImageViewer.Forms.Albums
{
    /// <summary>
    /// Логика взаимодействия для AlbumButton.xaml
    /// </summary>
    public partial class AlbumButton : UserControl
    {
        public AlbumClassLibrary.IAlbum Album;
        public event EventHandler AlbumButtonClicked;

        public AlbumButton(AlbumClassLibrary.IAlbum album)
        {
            InitializeComponent();

            this.Album = album;
            this.MainButton.Content = album.DisplayName;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            AlbumButtonClicked(Album, new EventArgs());
        }
    }
}
