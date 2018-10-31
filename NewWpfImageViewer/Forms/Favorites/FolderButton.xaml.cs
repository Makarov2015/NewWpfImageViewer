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

        public FolderButton(FolderEntity entity)
        {
            InitializeComponent();

            folder = entity;
            NameLabel.Content = entity.ShownName;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse_Click(folder, e);
        }
    }
}
