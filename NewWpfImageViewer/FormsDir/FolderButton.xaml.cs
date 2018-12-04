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

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Логика взаимодействия для FolderButton.xaml
    /// </summary>
    public partial class FolderButton : UserControl
    {
        public delegate void Mouse_ClickHandler(object sender, RoutedEventArgs e);
        public event Mouse_ClickHandler Mouse_Click;

        public FolderButton(string name)
        {
            InitializeComponent();
            NameLabel.Content = name;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse_Click(sender, e);
        }
    }
}
