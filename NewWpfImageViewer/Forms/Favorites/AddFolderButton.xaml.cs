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
        public delegate void Mouse_ClickHandler(object sender, RoutedEventArgs e);
        public event Mouse_ClickHandler Mouse_Click;

        public AddFolderButton()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse_Click(sender, e);
            MessageBox.Show("Что-то добавляем!");
        }
    }
}
