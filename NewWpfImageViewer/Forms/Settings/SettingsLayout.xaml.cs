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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewWpfImageViewer.Forms.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsLayout.xaml
    /// </summary>
    public partial class SettingsLayout : UserControl
    {
        public event EventHandler Close;
        public bool Edited = false;

        public SettingsLayout()
        {
            InitializeComponent();
            CacheFilePathTextBox.Text = Properties.Settings.Default.CacheFilePath + Properties.Settings.Default.CacheFileName;

            using (AlbumClassLibrary.CacheManager.CacheManager manager = new AlbumClassLibrary.CacheManager.CacheManager(CacheFilePathTextBox.Text))
            {
                CacheSizeLabel.Content = String.Format(CacheSizeLabel.Content.ToString(), manager.CacheSize);
            }

            MaxFilesTextbox.Text = Properties.Settings.Default.MaxFilesToLoad.ToString();
            this.Loaded += SettingsLayout_Loaded;
        }

        private void SettingsLayout_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation showAnimation = new DoubleAnimation();
            showAnimation.From = 0;
            showAnimation.To = this.ActualWidth;
            showAnimation.Duration = TimeSpan.FromMilliseconds(100);
            this.BeginAnimation(Button.WidthProperty, showAnimation);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation showAnimation = new DoubleAnimation();
            showAnimation.From = this.ActualWidth;
            showAnimation.To = 0;
            showAnimation.Duration = TimeSpan.FromMilliseconds(100);
            this.BeginAnimation(Button.WidthProperty, showAnimation);

            this.Close(this, new EventArgs());
        }

        private void ChangePath_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string pat = dialog.SelectedPath;
                    CacheFilePathTextBox.Text = pat + Properties.Settings.Default.CacheFileName;
                    ClassDir.SettingsManager.ChangeCacheFileFolder(pat);
                    Edited = true;
                }
            }
        }

        private void MaxFilesTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int newInt = 0;

            if(Int32.TryParse(MaxFilesTextbox.Text, out newInt))
            {
                if (newInt == Properties.Settings.Default.MaxFilesToLoad)
                    return;
                else
                {
                    Properties.Settings.Default.MaxFilesToLoad = newInt;
                    Properties.Settings.Default.Save();
                    Edited = true;
                }
            }
        }
    }
}
