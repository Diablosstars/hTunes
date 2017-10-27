using System;
using System.Collections.Generic;
using System.Data;
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

        private void Delete_Song(object sender, RoutedEventArgs e)
        {
            if (playListBox.SelectedItem.ToString() == "All Music")
            {
                DataRowView currentItem = songGrid.SelectedItem as DataRowView;
                int songID = (int)currentItem["id"];

                //https://social.msdn.microsoft.com/Forums/vstudio/en-US/d3f223ac-7fca-486e-8939-adb46e9bf6c9/how-can-i-get-yesno-from-a-messagebox-in-wpf?forum=wpf
                if (MessageBox.Show("Are you sure you wish to delete this song?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                { }
                else
                   if (currentItem != null)
                    musicLibrary.DeleteSong(songID);
            }
            else
            {
                DataRowView currentItem = songGrid.SelectedItem as DataRowView;

                ////Crashes when getting songID from a song in a playlist via context menu works for removing songs from All Music
                ////Not yet sure how to access the song_ID when accessing the listBox via playlist
                int songID = (int)currentItem["song_id"];
                int position = (int)currentItem["position"];
                musicLibrary.RemoveSongFromPlaylist(position, songID, playListBox.SelectedItem.ToString());
            }
        }

        private void Play_Song(object sender, RoutedEventArgs e)
        {
            
        }

        private void New_Playlist(object sender, RoutedEventArgs e)
        {

        }

        private void Stop_Song(object sender, RoutedEventArgs e)
        {

        }
    }
}
