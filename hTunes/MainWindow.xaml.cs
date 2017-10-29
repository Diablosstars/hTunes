using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using Microsoft.Win32;

namespace hTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MusicLib musicLibrary = new MusicLib();
        private MediaPlayer _mediaPlayer;

        private Point startPoint;
        public MainWindow()
        {
            _mediaPlayer = new MediaPlayer();
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
            if (playListBox.SelectedItem != null)
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
                var current = currentItem.Row.ItemArray;

                //Same method of retrieving the songID above wouldn't work for playlist songs...
                int songID = Convert.ToInt32(current[0]);
                int position = Convert.ToInt32(currentItem["position"]);

                musicLibrary.RemoveSongFromPlaylist(position, songID, playListBox.SelectedItem.ToString());

                var selectedPlaylist = playListBox.SelectedItem.ToString();
                var songs = musicLibrary.SongsForPlaylist(selectedPlaylist);

                songGrid.ItemsSource = songs.DefaultView;
            }
        }

        private void Play_Song(object sender, RoutedEventArgs e)
        {
            DataRowView currentItem = songGrid.SelectedItem as DataRowView;
            string songFilePath = (string)currentItem["filename"];
            _mediaPlayer.Stop();
            _mediaPlayer.Open(new Uri(songFilePath));
            _mediaPlayer.Play();
        }

        private void New_Playlist(object sender, RoutedEventArgs e)
        {
            //create new empty playlist
            musicLibrary.AddPlaylist("New Playlist");
            LoadList(sender, e);
        }

        private void Stop_Song(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
        }

        //https://stackoverflow.com/questions/10238694/example-using-hyperlink-in-wpf
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void RenamePlaylist_MenuItem(object sender, RoutedEventArgs e)
        {
            if (playListBox.SelectedItem != null && playListBox.SelectedItem.ToString() != "All Music")
            {
                RenameWindow popup = new RenameWindow(playListBox.SelectedItem.ToString());
                popup.ShowDialog();
                popup.PlaylistName = popup.PlaylistName.Trim();
                if (popup.PlaylistName != "All Music" 
                    && playListBox.SelectedItem.ToString() != popup.PlaylistName)
                {
                    musicLibrary.RenamePlaylist(playListBox.SelectedItem.ToString(), popup.PlaylistName);
                    LoadList(sender, e);
                }
            }
            
        }

        private void songGrid_MouseMove(object sender, MouseEventArgs e)
        {
            //Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            //Start the drag-drop if mouse has moved far enough
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                //Initiate dragging the text from the textbox
                DataRowView currentItem = songGrid.SelectedItem as DataRowView;
                if (currentItem != null)
                {
                    var current = currentItem.Row.ItemArray;
                    DragDrop.DoDragDrop(songGrid, current[0], DragDropEffects.Copy);
                }
            }
        }

        private void songGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Store the mouse position
            startPoint = e.GetPosition(null);
        }

        private void playListBox_Drop(object sender, DragEventArgs e)
        {
            string dataString = (string)e.Data.GetData(DataFormats.StringFormat);
            int songId = Convert.ToInt32(dataString);

            Song song = musicLibrary.GetSong(songId);

            //Add this song to current selected playlist

        }

        //Check to see if current DragOver item is All Music or not
        private void playListBox_DragOver(object sender, DragEventArgs e)
        {

        }

        private void Open_File_Explorer(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Music Files ( *.mp3, *.m4a, *.wma, *.wav )|*.mp3;*.m4a;*.wma;*.wav;|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                Song s = musicLibrary.AddSong(filePath);
                playListBox.SelectedItem = playListBox.Items[0];
                songGrid.SelectedItem = songGrid.Items[0];
                DataRowView currentItem = songGrid.SelectedItem as DataRowView;
                int id = (int)currentItem["id"];

                int index = 0;
                while (id != s.Id)
                {
                    index++;
                    songGrid.SelectedItem = songGrid.Items[index];
                    currentItem = songGrid.SelectedItem as DataRowView;
                    id = (int)currentItem["id"];
                }
            }

            
            
        }
    }
}
