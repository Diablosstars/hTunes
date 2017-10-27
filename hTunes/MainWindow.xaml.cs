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

namespace hTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MusicLib musicLibrary = new MusicLib();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }

        private void LoadList(object sender, RoutedEventArgs e)
        {
            var playlist = musicLibrary.Playlists;
            List<String> list = new List<string>();
            list.Add("All Music");
            list.AddRange(playlist);
            playListBox.ItemsSource = list;
        }

        private void LoadGrid(object sender, RoutedEventArgs e)
        {
            var songs = musicLibrary.Songs;
 
            songGrid.ItemsSource = songs.DefaultView;
        }

        private void ListItemSelect(object sender, SelectionChangedEventArgs e)
        {
            if (playListBox.SelectedItem.ToString() == "All Music")
            {
                var AllMusic = musicLibrary.Songs;
                songGrid.ItemsSource = AllMusic.DefaultView;
            }
            else
            {
                var selectedPlaylist = playListBox.SelectedItem.ToString();
                var songs = musicLibrary.SongsForPlaylist(selectedPlaylist);

                songGrid.ItemsSource = songs.DefaultView;
            }
        }
    }
}
