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
    /// Логика взаимодействия для AddNewAlbum.xaml
    /// </summary>
    public partial class NewAlbumButton : UserControl
    {
        AlbumClassLibrary.IAlbumManager AlbumManager;
        public event EventHandler AlbumAdded;

        public NewAlbumButton(AlbumClassLibrary.IAlbumManager manager)
        {
            AlbumManager = manager;
            InitializeComponent();
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            NewAlbumWindow newAlbum = new NewAlbumWindow(AlbumManager);
            newAlbum.ShowDialog();

            if (newAlbum.AlbumAdded)
                AlbumAdded(null, new EventArgs());
        }
    }
}
