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

namespace NewWpfImageViewer.Forms.Albums
{
    /// <summary>
    /// Логика взаимодействия для NewAlbumWindow.xaml
    /// </summary>
    public partial class NewAlbumWindow : Window
    {
        private AlbumClassLibrary.IAlbumManager AlbumManager;
        public bool AlbumAdded = false;

        public NewAlbumWindow(AlbumClassLibrary.IAlbumManager albumManager)
        {
            AlbumManager = albumManager;
            InitializeComponent();

            TypeComboBox.ItemsSource = albumManager.AvailableAlbums;
            TypeComboBox.DisplayMemberPath = "AlbumName";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var item = TypeComboBox.SelectedItem as AlbumClassLibrary.IAlbum;
            item.DisplayName = NameTextBox.Text;

            AlbumManager.AddAlbum(item);

            AlbumAdded = true;
            Close();
        }
    }
}
